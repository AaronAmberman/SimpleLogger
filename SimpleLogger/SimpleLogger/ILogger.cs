namespace SimpleLogger
{
    /// <summary>Describes a logger object.</summary>
    public interface ILogger
    {
        string LogFile { get; set; }
        long LogRollSize { get; set; }
        LogLevel LogLevel { get; set; }

        void Debug(string message);
        void Error(string message);
        void Fatal(string message);
        void Info(string message);
        bool RollLogFile();
        void Trace(string message);
        void Warning(string message);
    }
}