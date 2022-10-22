namespace SimpleLogger
{
    /// <summary>A simple logger object that has rolling log files.</summary>
    public class Logger : ILogger
    {
        #region Properties

        /// <summary>Gets or sets the log file.</summary>
        public string LogFile { get; set; }

        /// <summary>Gets or sets the file size to roll the log file at. If less than 0 then the log file will not be rolled. Default is 102400 (100KB).</summary>
        public long LogRollSize { get; set; } = 102400; // 100KB default

        /// <summary>Gets or sets the log level for the logger. Default is Trace.</summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;

        #endregion

        #region Methods

        /// <summary>Writes a debug level message to the log file.</summary>
        /// <param name="message">The message to write.</param>
        public void Debug(string message)
        {
            if (LogLevel >= LogLevel.Debug)
                WriteToLogFile(LogLevel.Debug, message);
        }

        private void EnsureLogFile()
        {
            if (string.IsNullOrWhiteSpace(LogFile))
            {
                throw new ArgumentNullException(nameof(LogFile), "The LogFile cannot be null, empty or consist of whitespace characters only.");
            }
        }

        /// <summary>Writes a error level message to the log file.</summary>
        /// <param name="message">The message to write.</param>
        public void Error(string message)
        {
            if (LogLevel >= LogLevel.Error)
                WriteToLogFile(LogLevel.Error, message);
        }

        /// <summary>Writes a fatal level message to the log file.</summary>
        /// <param name="message">The message to write.</param>
        public void Fatal(string message)
        {
            if (LogLevel >= LogLevel.Fatal)
                WriteToLogFile(LogLevel.Fatal, message);
        }

        /// <summary>Writes a info level message to the log file.</summary>
        /// <param name="message">The message to write.</param>
        public void Info(string message)
        {
            if (LogLevel >= LogLevel.Info)
                WriteToLogFile(LogLevel.Info, message);
        }

        /// <summary>Rolls the log file if needed.</summary>
        /// <returns>True if rolled, otherwise false.</returns>
        public bool RollLogFile()
        {
            if (LogRollSize < 0) return false; //  no rolling desired

            if (File.Exists(LogFile))
            {
                try
                {
                    FileInfo file = new FileInfo(LogFile);

                    if (file.Length > LogRollSize)
                    {
                        string nameAddition = DateTime.Now.ToString("+MM-dd-yyyy+hh.mm.ss");
                        string extension = Path.GetExtension(file.Name);
                        string newName = Path.GetFileNameWithoutExtension(file.Name) + nameAddition + extension;
                        string path = file.DirectoryName;
                        string absolutePath = Path.Combine(path, newName);

                        file.CopyTo(absolutePath);

                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unable to roll the log file.{Environment.NewLine}{ex}");

                    return false;
                }
            }

            return false;
        }

        /// <summary>Writes a trace level message to the log file.</summary>
        /// <param name="message">The message to write.</param>
        public void Trace(string message)
        {
            if (LogLevel >= LogLevel.Trace)
                WriteToLogFile(LogLevel.Trace, message);
        }

        /// <summary>Writes a warning level message to the log file.</summary>
        /// <param name="message">The message to write.</param>
        public void Warning(string message)
        {
            if (LogLevel >= LogLevel.Warning)
                WriteToLogFile(LogLevel.Warning, message);
        }

        private void WriteToLogFile(LogLevel level, string message)
        {
            EnsureLogFile();

            try
            {
                FileStream fs = null;

                if (RollLogFile())
                {
                    fs = new FileStream(LogFile, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(LogFile, FileMode.Append, FileAccess.Write);
                }

                StreamWriter writer = new StreamWriter(fs);

                string date = DateTime.Now.ToString("MM/dd/yyyy-hh:mm:ss");

                writer.WriteLine($"{date}|{level}|{message}");

                writer.Close();
                fs.Close();

                writer.Dispose();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to write to the log file.{Environment.NewLine}{ex}");
            }
        }

        #endregion
    }
}
