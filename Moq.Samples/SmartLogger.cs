namespace MoqSamples
{
    public class SmartLogger
    {
        private readonly ILogWriter _logWriter;
        private readonly ILogMailer _logMailer;

        public SmartLogger(ILogWriter logWriter, ILogMailer logMailer)
        {
            _logWriter = logWriter;
            _logMailer = logMailer;
        }

        public void WriteLine(string message)
        {
            var mailMessage = _logMailer.CreateMessage(message);
            _logMailer.Send(mailMessage);

            _logWriter.Write(message);
        }
    }
}