using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using Microsoft.Extensions.Caching.Memory;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class IndesserTokenResetControllerTest : BaseControllerTest
    {
        private IndesserTokenResetController _sut;
        private Mock<IMemoryCache> _mockCacheService;
        private Mock<ILogger<IndesserTokenResetController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockCacheService = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<IndesserTokenResetController>>();
            _sut = new IndesserTokenResetController(_mockCacheService.Object, _mockLogger.Object);
        }

        [Test]
        public void IndexHandlesSessionException()
        {
            _mockCacheService
                .Setup(_ => _.Remove("connectionToken"))
                .Throws(new Exception("fake error message"));

            _sut.Index();

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Error clearing Indesser connection token") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public void Index()
        {
            _sut.Index();

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Clearing Indesser connection token") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Cleared Indesser connection token") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}