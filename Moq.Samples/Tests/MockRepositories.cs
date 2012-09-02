using System.Net.Mail;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace MoqSamples
{
    /// <summary>
    /// In some cases convenient to use different mocks in atomic way and verify them in one call,
    /// for example.
    /// </summary>
    [TestFixture]
    public class MockRepositories
    {
        [Test]
        public void Test_Initialize_Multiple_Stubs_Using_Repository_Of_Method()
        {
            // There is another way to initialize multiple stubs inside one
            // expression using LINQ-style syntax (using moq functional specification)
            // using MockRepository class

            // Arrange
            var repository = new MockRepository(MockBehavior.Default);
            ILoggerDependency logger = repository.Of<ILoggerDependency>()
                .Where(ld => ld.DefaultLogger == "DefaultLogger")
                .Where(ld => ld.GetCurrentDirectory() == "D:\\Temp")
                .Where(ld => ld.GetDirectoryByLoggerName(It.IsAny<string>()) == "C:\\Temp")
                .First();

            // Assert
            Assert.That(logger.GetCurrentDirectory(), Is.EqualTo("D:\\Temp"));
            Assert.That(logger.DefaultLogger, Is.EqualTo("DefaultLogger"));
            Assert.That(logger.GetDirectoryByLoggerName("CustomLogger"), Is.EqualTo("C:\\Temp"));
        }

        [Test]
        public void Test_Initialize_Multiple_Stubs_Using_Repository_OneOf_Method()
        {
            // MockRepository supports two ways for building subs.
            // Instead of calling "Of" method we can call OneOf and
            // use the same syntax as for creating stubs via Mock.Of

            // Arrange
            var repository = new MockRepository(MockBehavior.Default);
            ILoggerDependency logger =
                repository.OneOf<ILoggerDependency>(ld => ld.DefaultLogger == "DefaultLogger"
                                                    && ld.GetCurrentDirectory() == "D:\\Temp"
                                                    && ld.GetDirectoryByLoggerName(It.IsAny<string>()) == "C:\\Temp");

            // Assert
            Assert.That(logger.GetCurrentDirectory(), Is.EqualTo("D:\\Temp"));
            Assert.That(logger.DefaultLogger, Is.EqualTo("DefaultLogger"));
            Assert.That(logger.GetDirectoryByLoggerName("CustomLogger"), Is.EqualTo("C:\\Temp"));
        }

        [Test]
        public void Test_WriteLine_Calls_LogWriter_And_LogMailer()
        {
            // Creating different mocks from MockRepository simplifies
            // future verification by calling one verification method
            // on mock repository instead of calling different methods
            // on different mock objects

            // Arrange
            var repo = new MockRepository(MockBehavior.Default);
            var logWriterMock = repo.Create<ILogWriter>();
            logWriterMock.Setup(lw => lw.Write(It.IsAny<string>()));

            var logMailerMock = repo.Create<ILogMailer>();
            logMailerMock.Setup(lm => lm.Send(It.IsAny<MailMessage>()));

            var smartLogger = new SmartLogger(logWriterMock.Object, logMailerMock.Object);

            // Act
            smartLogger.WriteLine("Hello, Logger");

            // Assert
            repo.Verify();

        }
    }
}