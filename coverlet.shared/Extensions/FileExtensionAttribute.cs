using System;

namespace coverlet.shared
{
    public class FileExtensionAttribute : Attribute
    {
        public string Extension { get; }

        public FileExtensionAttribute(string extension)
        {
            Extension = extension;
        }
    }
}
