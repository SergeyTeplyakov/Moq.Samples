namespace MoqSamples
{
    public class Logger
    {
        private readonly ILogWriter _logWriter;

        public Logger(ILogWriter logWriter)
        {
            _logWriter = logWriter;
        }

        public void WriteLine(string message)
        {
            _logWriter.Write(message);
        }
    }
}