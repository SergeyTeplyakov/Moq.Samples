namespace MoqSamples
{
    public interface ILogWriter
    {
        string GetLogger();
        void SetLogger(string logger);
        void Write(string message);
    }

}