using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace coverlet.visualizer
{
    [Guid(WindowGuidString)]
    public class CoverageToolWindow : ToolWindowPane
    {
        public const string WindowGuidString = "e4e2ba26-a455-4c53-adb3-8225fb696f8b"; // Replace with new GUID in your own code
        public const string Title = "Coverage Visualizer Window";

        // "state" parameter is the object returned from MyPackage.InitializeToolWindowAsync
        public CoverageToolWindow(CoverageToolWindowState state) : base()
        {
            Caption = Title;
            BitmapImageMoniker = KnownMonikers.ImageIcon;

            Content = new CoverageToolWindowControl(state);
        }
    }
}
