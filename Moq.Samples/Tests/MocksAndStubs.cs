using System.Net.Mail;
using Moq;
using NUnit.Framework;

namespace MoqSamples
{
    /// <summary>
    /// Samples for creating fakes that behaves like mocks and stubs simultaneously.
    /// </summary>
    [TestFixture]
    public class MocksAndStubs
    {
        [Test]
        public void Test_SmartLogger_WriteLine_Calls_LogMailer_CreateMessage()
        {
            // In some cases we should combine stubs and mocks.
            // For example, ILogMailer.CreateMessage requires state initialization 
            // required by SmartLogger (stub) and behavior verification (that
            // SmartLogger calls CreateMessage with specified argument).

            // Arrange
            var logMailer = Mock.Of<ILogMailer>(
                lm => lm.CreateMessage(It.IsAny<string>()) == new MailMessage("from@g.com", "to@g.com"));
            var logWriter = Mock.Of<ILogWriter>();
            var smartLogger = new SmartLogger(logWriter, logMailer);

            // Act
            smartLogger.WriteLine("42");

            // Assert
            var logMailerMock = Mock.Get(logMailer);
            logMailerMock.Verify(lm => lm.CreateMessage("42"));
        }
    }
}