using System;
using Moq;
using NUnit.Framework;

namespace MoqSamples
{
    public interface ILoggerDependency
    {
        string GetCurrentDirectory();
        string GetDirectoryByLoggerName(string loggerName);
        string DefaultLogger { get; }
    }

    /// <summary>
    /// Contains sample code for creating stubs with Moq.
    /// </summary>
    /// <remarks>
    /// For more information about differences between mocks and stubs you can read following articles:
    /// 1. Mocks Aren't Stubs by Martin Fowler (http://martinfowler.com/articles/mocksArentStubs.html)
    /// 2. Mocks, Stubs and Fakes: it’s a continuum by Daniel Cazzulino
    ///    (http://blogs.clariusconsulting.net/kzu/mocks-stubs-and-fakes-its-a-continuum/)
    /// 3. Стабы и моки (http://sergeyteplyakov.blogspot.com/2011/12/blog-post.html) (in Russian)
    /// </remarks>
    [TestFixture]
    public class Stubs
    {
        [Test]
        public void Test_GetCurrentDirrectory_Simple_Stub()
        {
            // Arrange
            // Mock.Of returns dependency itself (auto generated proxy) but not the mock object.
            // This code means that GetCurrentDirectory() will return "D:\\Temp"
            ILoggerDependency loggerDependency =
                Mock.Of<ILoggerDependency>(d => d.GetCurrentDirectory() == "D:\\Temp");
            var currentDirectory = loggerDependency.GetCurrentDirectory();

            Console.WriteLine("Current directory is {0}", currentDirectory);

            // Assert
            Assert.That(currentDirectory, Is.EqualTo("D:\\Temp"));
        }

        [TestCase("Foo")]
        [TestCase("Boo")]
        public void Test_GetDirectoryByLoggerName_Stub_Always_Returns_The_Same_Value(string loggerName)
        {
            // Arrange
            // For any parameter in the GetDirectoryByLoggerName Stub should return "C:\\Foo".
            ILoggerDependency loggerDependency = Mock.Of<ILoggerDependency>(
                ld => ld.GetDirectoryByLoggerName(It.IsAny<string>()) == "C:\\Foo");

            string directory = loggerDependency.GetDirectoryByLoggerName(loggerName);
            Console.WriteLine("Directory for the logger '{0}' is '{1}'", loggerName, directory);

            // Assert
            Assert.That(directory, Is.EqualTo("C:\\Foo"));
        }

        [TestCase("Foo")]
        [TestCase("Boo")]
        public void Test_GetDirectoryByLoggerName_Stub_Returns_Different_Value_Based_On_The_Arguments(string loggerName)
        {
            // Arrange
            // Unfortunately I don't know how to create stub that will return different value based on the arguments
            // using v.4 functional specification style
            Mock<ILoggerDependency> stub = new Mock<ILoggerDependency>();

            // Setting up our stub to return different values based on the argument.
            // This code is similar to following implementation:
            // public string GetDirectoryByLoggername(string s) { return "C:\\" + s; }
            stub
                .Setup(ld => ld.GetDirectoryByLoggerName(It.IsAny<string>()))
                .Returns<string>(name => "C:\\" + name);

            ILoggerDependency logger = stub.Object;
            string directory = logger.GetDirectoryByLoggerName(loggerName);

            Console.WriteLine("Directory for the logger '{0}' is '{1}'", loggerName, directory);

            // Assert
            Assert.That(directory, Is.EqualTo("C:\\" + loggerName));

        }

        [Test]
        public void Test_DefaultLogger_Simple_Stub()
        {
            // Arrange
            ILoggerDependency logger = Mock.Of<ILoggerDependency>(
                d => d.DefaultLogger == "DefaultLogger");

            string defaultLogger = logger.DefaultLogger;

            Console.WriteLine("Default logger is '{0}'", defaultLogger);

            // Assert
            Assert.That(defaultLogger, Is.EqualTo("DefaultLogger"));
        }

        [Test]
        public void Test_Initialize_Multiple_Stubs_Using_v4_Syntax()
        {
            // Arrange
            // Moq v.4 introduces new feature called "moq functional specification".
            // Using this new syntax we're able to setup several stubs in one expression
            ILoggerDependency logger = 
                Mock.Of<ILoggerDependency>(
                    d => d.GetCurrentDirectory() == "D:\\Temp" &&
                         d.DefaultLogger == "DefaultLogger" &&
                         d.GetDirectoryByLoggerName(It.IsAny<string>()) == "C:\\Temp");

            // Assert
            Assert.That(logger.GetCurrentDirectory(), Is.EqualTo("D:\\Temp"));
            Assert.That(logger.DefaultLogger, Is.EqualTo("DefaultLogger"));
            Assert.That(logger.GetDirectoryByLoggerName("CustomLogger"), Is.EqualTo("C:\\Temp"));
        }
        
        [Test]
        public void Test_Initialize_Multiple_Stubs_Using_Setup_Calls()
        {
            // Arrange
            // Moq v.4 introduces new feature called "moq functional specification".
            // Using this new syntax we're able to setup several stubs in one expression
            Mock<ILoggerDependency> stub = new Mock<ILoggerDependency>();
            stub.Setup(ld => ld.GetCurrentDirectory()).Returns("D:\\Temp");
            stub.Setup(ld => ld.GetDirectoryByLoggerName(It.IsAny<string>())).Returns("C:\\Temp");
            stub.SetupGet(ld => ld.DefaultLogger).Returns("DefaultLogger");

            ILoggerDependency logger = stub.Object;

            // Assert
            Assert.That(logger.GetCurrentDirectory(), Is.EqualTo("D:\\Temp"));
            Assert.That(logger.DefaultLogger, Is.EqualTo("DefaultLogger"));
            Assert.That(logger.GetDirectoryByLoggerName("CustomLogger"), Is.EqualTo("C:\\Temp"));
        }

        [Test]
        public void Test_Getting_Mock_By_The_Mocked_Interface()
        {
            // In some cases we need to obtain mock-object by mocked interface.
            // For example, we can use "moq functional specification" syntax for several
            // members and use old imperative syntax for some other members
            ILoggerDependency logger = Mock.Of<ILoggerDependency>(
                ld => ld.GetCurrentDirectory() == "D:\\Temp"
                   && ld.DefaultLogger == "DefaultLogger");

            // Getting mock-object itself by a mocked interface
            Mock<ILoggerDependency> stub = Mock.Get(logger);

            stub.Setup(ld => ld.GetDirectoryByLoggerName(It.IsAny<string>()))
                .Returns<string>(loggerName => "C:\\" + loggerName);

            // Assert
            Assert.That(logger.GetCurrentDirectory(), Is.EqualTo("D:\\Temp"));
            Assert.That(logger.DefaultLogger, Is.EqualTo("DefaultLogger"));
            Assert.That(logger.GetDirectoryByLoggerName("Foo"), Is.EqualTo("C:\\Foo"));
            Assert.That(logger.GetDirectoryByLoggerName("Boo"), Is.EqualTo("C:\\Boo"));
        }
    }
}