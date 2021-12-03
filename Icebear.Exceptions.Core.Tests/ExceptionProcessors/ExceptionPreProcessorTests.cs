using System;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.ErrorMessageHandlers;
using yourLogs.Exceptions.Core.ExceptionProcessors;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NUnit.Framework;

namespace yourLogs.Exceptions.Core.Tests.ExceptionProcessors
{
    [TestFixture]
    public class ExceptionPreProcessorTests
    {
        [Test]
        public async Task EnsureOrders()
        {
            var processor = ExceptionProcessorFactory.Default();
            var processor1 = Substitute.For<IExceptionPreProcessor>();
            processor1.PreProcessException(Arg.Any<Exception>())
                .Returns(new Exception("Processor1"));
                
            var processor2 = Substitute.For<IExceptionPreProcessor>();
            processor2.PreProcessException(Arg.Any<Exception>())
                .Returns(new Exception("Processor2"));
            
            processor.RegisterPreHandlers(
                processor1, 
                processor2
            );

            var result = await processor.HandleException(new Exception("testException"));
            
            processor1.Received(1).PreProcessException(Arg.Any<Exception>());
            processor2.Received(1).PreProcessException(Arg.Any<Exception>());

            Assert.IsNotNull(result);
            Assert.AreEqual("Processor2", result.Text);
        }
    }
}