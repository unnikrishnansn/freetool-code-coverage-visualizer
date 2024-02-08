using System;
using System.Threading.Tasks;

namespace coverlet.shared
{
    public class ProcessParameter
    {
        public string FileName { get; set; }
        public string Arguments { get; set; }
        public Func<string, Task> PrintOutputMessage { get; set; }
        public Func<string, Task> PrintErrorMessage { get; set; }
        public Func<Task> ProcessComplete { get; set; }
    }
}
