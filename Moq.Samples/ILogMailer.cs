using System.Net.Mail;

namespace MoqSamples
{
    public interface ILogMailer
    {
        string GetDefaultUserName();
        MailMessage CreateMessage(string body);
        void Send(MailMessage message);
    }
}