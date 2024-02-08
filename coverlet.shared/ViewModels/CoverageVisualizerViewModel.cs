using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace coverlet.shared
{
    public class CoverageVisualizerViewModel : ViewModelBase, ICoverageVisualizerViewModel
    {
        private const string DOTNETFILENAME = "dotnet";
        private const string CPATH = @"C:\\coveragevisualizer\";
        private readonly string defaultText = @"
        MANDATORY STEPS BEFORE CLICK
        1. https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=windows#generate-reports
        2. dotnet tool install -g dotnet-reportgenerator-globaltool
        3. Solution has Unit Test projects
        4. Unit Test Projects has coverlet NUGET package
        5. CLICK TO GENERATE COVERAGE REPORT!";
        private string reportFormat = nameof(ReportFormats.Html);
        private bool showLoader;
        private string displayOutput;
        private bool isGenerateEnabled = true;
        private string folderPath = CPATH;

        public string FolderPath
        {
            get { return folderPath; }
            set { folderPath = value; OnPropertyChanged(nameof(FolderPath)); }
        }
        public Dispatcher Dispatcher { get; set; }
        public CoverageVisualizerViewModel()
        {
            DisplayOutput = defaultText;
            ReportTypes = Enum.GetNames(typeof(ReportFormats)).ToList();
        }
        public bool IsGenerateEnabled
        {
            get { return isGenerateEnabled; }
            set { isGenerateEnabled = value; OnPropertyChanged(nameof(IsGenerateEnabled)); }
        }
        public bool ShowLoader
        {
            get { return showLoader; }
            set { showLoader = value; OnPropertyChanged(nameof(ShowLoader)); }
        }
        public string DisplayOutput
        {
            get { return displayOutput; }
            set { displayOutput = value; OnPropertyChanged(nameof(DisplayOutput)); }
        }
        public string ReportFormat
        {
            get { return reportFormat; }
            set
            {
                reportFormat = value;
                OnPropertyChanged(nameof(ReportFormat));
            }
        }
        public List<string> ReportTypes { get; set; }

        async Task ICoverageVisualizerViewModel.ResetAsync()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                IsGenerateEnabled = true;
                ShowLoader = false;
                DisplayOutput = defaultText;
            });
        }

        async Task ICoverageVisualizerViewModel.StartCoverageReportGenerationAsync()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    IsGenerateEnabled = false;
                    ShowLoader = true;
                    DisplayOutput += "\n INSTALLING dotnet-reportgenerator-globaltool... \n";
                });
                await ProcessHelper.ExecuteAsync(new ProcessParameter
                {
                    FileName = DOTNETFILENAME,
                    Arguments = "tool install -g dotnet-reportgenerator-globaltool",
                    ProcessComplete = GenerateCodeCoverageXmlAsync,
                    PrintOutputMessage = PrintOutputMessage,
                    PrintErrorMessage = PrintErrorMessage
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    IsGenerateEnabled = true;
                    ShowLoader = false;
                    DisplayOutput += $"\n {ex.ToString()} \n";
                });
            }

        }

        private async Task PrintErrorMessage(string message)
        {
            await Dispatcher.InvokeAsync(() => DisplayOutput += message);
        }

        private async Task PrintOutputMessage(string message)
        {
            await Dispatcher.InvokeAsync(() => DisplayOutput += message);
        }

        private async Task GenerateCodeCoverageXmlAsync()
        {
            await ResetOutputFolders();
            Dispatcher.Invoke(() =>
            {
                DisplayOutput += "COLLECTING UNIT TEST CODE COVERAGE... \n";
            });
            await ProcessHelper.ExecuteAsync(new ProcessParameter
            {
                FileName = DOTNETFILENAME,
                Arguments = $"test --collect:\"XPlat Code Coverage\" --no-restore",
                ProcessComplete = GenerateReportAsync,
                PrintOutputMessage = PrintOutputMessage,
                PrintErrorMessage = PrintErrorMessage
            });
        }

        private async Task GenerateReportAsync()
        {

            await Dispatcher.InvokeAsync(() =>
            {
                DisplayOutput += $"GENERATING REPORTS BASED ON coverage.cobertura.xml \n";
            });
            await ProcessHelper.ExecuteAsync(new ProcessParameter
            {
                FileName = "powershell.exe",
                Arguments = $@"reportgenerator -reports:'**/coverage.cobertura.xml' -targetdir:{FolderPath}\CoverageReports -reporttypes:{ReportFormat}",
                ProcessComplete = OpenOutputAsync,
                PrintOutputMessage = PrintOutputMessage,
                PrintErrorMessage = PrintErrorMessage
            });
        }

        private async Task OpenOutputAsync()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                DisplayOutput += $"LOADING GENERATED REPORTS.. \n";
                ShowLoader = false;
                DisplayOutput += $@"OPEN {FolderPath}/CoverageReports/index.html IN A BROWSER, IF NOT OPENED!";
                var extension = ReportFormatHelper.GetFileExtension(ReportFormat);
                if (extension == "html")
                {
                    System.Diagnostics.Process.Start($@"chrome.exe", $"{FolderPath}\\CoverageReports\\index.html");
                }
                else
                {
                    System.Diagnostics.Process.Start($"{FolderPath}\\CoverageReports\\");
                }

                IsGenerateEnabled = true;
            });

        }

        private async Task ResetOutputFolders()
        {
            await Task.Run(() =>
            {
                ClearOldReports();
                ClearOldCoverageXml();

            });
        }

        private void ClearOldCoverageXml()
        {
            try
            {
                var directory = Directory.GetDirectories($"{Directory.GetCurrentDirectory()}", "TestResults", SearchOption.AllDirectories).FirstOrDefault();
                Dispatcher.InvokeAsync(() => DisplayOutput += $"CHECKING {directory} FOLDER CODE COVERAGE XML... \n");

                if (!string.IsNullOrEmpty(directory))
                {
                    Dispatcher.InvokeAsync(() => DisplayOutput += $"DELETING {directory} FOLDER CODE COVERAGE XML... \n");
                    Directory.Delete($"{directory}", true);
                }
                Dispatcher.InvokeAsync(() => DisplayOutput += $"GENERATING XPLAT CODE COVERAGE FOR ALL TESTS IN THE PROJECT... \n");
            }
            catch (Exception ex)
            {
                Dispatcher.InvokeAsync(() => DisplayOutput += $"{ex.ToString()}... \n");
            }
        }

        private void ClearOldReports()
        {
            try
            {
                var priorReportPath = $"{FolderPath}/CoverageReports";
                Dispatcher.InvokeAsync(() =>
                {
                    DisplayOutput += $"CHECKING {priorReportPath} FOLDER FOR OLD REPORTS... \n";
                });
                if (Directory.Exists(priorReportPath))
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        DisplayOutput += $"DELETING {priorReportPath} FOLDER OLD REPORTS... \n";
                    });

                    Directory.Delete(priorReportPath, true);
                }
            }
            catch (Exception ex)
            {

                Dispatcher.InvokeAsync(() => DisplayOutput += $"{ex.ToString()}... \n");
            }
        }
    }
}
