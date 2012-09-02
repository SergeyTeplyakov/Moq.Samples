using Moq;
using NUnit.Framework;

namespace MoqSamples
{
    /// <summary>
    /// Sample code for creating mocks with Moq
    /// </summary>
    /// <remarks>
    /// For more information about differences between mocks and stubs you can read following articles:
    /// 1. Mocks Aren't Stubs by Martin Fowler (http://martinfowler.com/articles/mocksArentStubs.html)
    /// 2. Mocks, Stubs and Fakes: it’s a continuum by Daniel Cazzulino
    ///    (http://blogs.clariusconsulting.net/kzu/mocks-stubs-and-fakes-its-a-continuum/)
    /// 3. Стабы и моки (http://sergeyteplyakov.blogspot.com/2011/12/blog-post.html) (in Russian)
    /// </remarks>
    [TestFixture]
    public class Mocks
    {
        [Test]
        public void Test_WriteLine_Calls_Write()
        {
            // Arrange
            var mock = new Mock<ILogWriter>();
            var logger = new Logger(mock.Object);

            // Act
            logger.WriteLine("Hello, logger!");

            // Assert
            // Checking that Write method of the ILogWriter was called
            mock.Verify(lw => lw.Write(It.IsAny<string>()));
        }
        
        [Test]
        public void Test_WriteLine_Calls_Write_With_Appropriate_Argument()
        {
            // Arrange
            var mock = new Mock<ILogWriter>();
            var logger = new Logger(mock.Object);

            // Act
            logger.WriteLine("Hello, logger!");

            // Assert
            // Checking that Write method was called with appropriate argument
            mock.Verify(lw => lw.Write("Hello, logger!"));
        }

        [Test]
        public void Test_WriteLine_Called_Exactly_Once()
        {
            // Arrange
            var mock = new Mock<ILogWriter>();
            var logger = new Logger(mock.Object);

            // Act
            logger.WriteLine("Hello, logger!");

            // Assert
            // We could check, that particular method calls specified number of times
            mock.Verify(lw => lw.Write(It.IsAny<string>()),
                Times.Once());
        }

        [Test]
        public void Test_WriteLine_Calls_Write_With_Setup_Method()
        {
            // Arrange
            var mock = new Mock<ILogWriter>();
            mock.Setup(lw => lw.Write(It.IsAny<string>()));

            var logger = new Logger(mock.Object);

            // Act
            logger.WriteLine("Hello, logger!");

            // Assert
            // We're not explicitly stated what we're expecting.
            // mock.Setup expectations would be use.
            mock.Verify();
        }

        [Test]
        public void Test_WriteLine_And_SetLogger_Calls()
        {
            // By default Moq uses loose verification model, so verify method will
            // passed even if we're not explicitly set up all required dependencies.
            // In strict mode Verify method will fail if we left some calls in setup method.

            // Arrange
            var mock = new Mock<ILogWriter>(MockBehavior.Strict);
            // Commenting one of the following lines will lead to exception in mock.Verify()
            mock.Setup(lw => lw.Write(It.IsAny<string>()));
            mock.Setup(lw => lw.SetLogger(It.IsAny<string>()));

            var logger = new Logger(mock.Object);

            // Act
            logger.WriteLine("Hello, logger!");

            // Assert
            mock.Verify();
        }

    }
}