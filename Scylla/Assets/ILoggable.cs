namespace Scylla
{
    
    public enum LogLevel
    {
        None,
        Simple,
        Verbose
    }
    
    public interface ILoggable
    {
        void Log(string log);
    }
}

