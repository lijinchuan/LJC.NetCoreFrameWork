using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LJC.NetCoreFrameWork.Comm
{
    public static class ShellHelper
    {
        public static string Exec(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    RedirectStandardError = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.WaitForExit();

            string result = process.StandardOutput.ReadToEnd();

            result += process.StandardError.ReadToEnd();

            return result;
        }
    }
}
