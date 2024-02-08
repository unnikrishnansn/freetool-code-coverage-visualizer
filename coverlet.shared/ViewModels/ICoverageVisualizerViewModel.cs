using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace coverlet.shared
{
    public interface ICoverageVisualizerViewModel
    {
        string DisplayOutput { get; set; }
        string ReportFormat { get; set; }
        bool IsGenerateEnabled { get; set; }
        bool ShowLoader { get; set; }
        Dispatcher Dispatcher { get; set; }
        List<string> ReportTypes { get; set; }
        string FolderPath { get; set; }

        Task StartCoverageReportGenerationAsync();
        Task ResetAsync();
    }
}
