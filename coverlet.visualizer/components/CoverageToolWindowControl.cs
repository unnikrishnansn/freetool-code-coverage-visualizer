using coverlet.shared;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;

namespace coverlet.visualizer
{
    /// <summary>
    /// Interaction logic for CoverletCoverageWindowControl.
    /// </summary>
    public partial class CoverageToolWindowControl : UserControl
    {
        private CoverageToolWindowState _state;
        public ICoverageVisualizerViewModel VM { get; set; }
        public CoverageToolWindowControl(CoverageToolWindowState state)
        {
            _state = state;
            VM = new CoverageVisualizerViewModel();
            VM.Dispatcher = Dispatcher;
            InitializeComponent();
            this.DataContext = VM;

        }

        private void generateReport_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            VM.StartCoverageReportGenerationAsync();
        }

        private void reset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            VM.ResetAsync();
        }

        private void file_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var folderDialog = new OpenFileDialog
            {
                Title = "Select a Folder",
                Filter = "Folder|*.dummy",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "SelectFolder"
            };             // Show dialog and get selected folder path
            var result = folderDialog.ShowDialog();
            if (result.GetValueOrDefault())
            {                 // Update textbox with selected folder path
                VM.FolderPath = Path.GetDirectoryName(folderDialog.FileName);
            }
        }
    }
}