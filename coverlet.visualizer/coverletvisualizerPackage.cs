using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace coverlet.visualizer
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("Coverage Tool", "Tool to run the steps for coverlet report generation", "1.0")]
    [ProvideToolWindow(typeof(CoverageToolWindow), Style = VsDockStyle.Tabbed, DockedWidth = 300, Window = "DocumentWell", Orientation = ToolWindowOrientation.Top)]
    [Guid("6e3b2e95-902b-4385-a966-30c06ab3c7a6")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class CoverageViewerPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            await ShowToolWindow.InitializeAsync(this);
        }

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType)
        {
            return toolWindowType.Equals(Guid.Parse(CoverageToolWindow.WindowGuidString)) ? this : null;
        }

        protected override string GetToolWindowTitle(Type toolWindowType, int id)
        {
            return toolWindowType == typeof(CoverageToolWindow) ? CoverageToolWindow.Title : base.GetToolWindowTitle(toolWindowType, id);
        }

        protected override async Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken)
        {
            // Perform as much work as possible in this method which is being run on a background thread.
            // The object returned from this method is passed into the constructor of the SampleToolWindow 
            var dte = await GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;

            return new CoverageToolWindowState
            {
                DTE = dte
            };
        }

    }
}
