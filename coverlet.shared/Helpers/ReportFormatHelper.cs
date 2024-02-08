using System;

namespace coverlet.shared
{
    public static class ReportFormatHelper
    {
        public static string GetFileExtension(string reportFormat)
        {
            var attribute = (FileExtensionAttribute)Attribute.GetCustomAttribute(
                typeof(ReportFormats).GetField(reportFormat),
                typeof(FileExtensionAttribute));

            return attribute?.Extension;
        }
    }
}
