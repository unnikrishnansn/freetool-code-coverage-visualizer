using System.Diagnostics;
using System.Threading.Tasks;

namespace coverlet.shared
{
    public static class ProcessHelper
    {
        public static async Task ExecuteAsync(ProcessParameter processParamter)
        {
            await Task.Run(() => {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo(processParamter.FileName, processParamter.Arguments);
                    psi.RedirectStandardError = true;
                    psi.RedirectStandardOutput = true;
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;
                    using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi))
                    {
                        while (!process.HasExited)
                        {
                            processParamter.PrintOutputMessage?.Invoke(process.StandardOutput.ReadToEnd());
                        }

                        processParamter.PrintOutputMessage?.Invoke(process.StandardOutput.ReadToEnd());
                        processParamter.PrintErrorMessage?.Invoke(process.StandardError.ReadToEnd());
                    }
                }
                catch (System.Exception ex)
                {

                    processParamter.PrintErrorMessage?.Invoke(ex.ToString());
                }
              
                processParamter.ProcessComplete?.Invoke();
            });
        }
    }
}
