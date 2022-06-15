using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility;

using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount.Services
{
    [TestFixture]
    public class EligibilityCheckResultServiceTests
    {
        private EligibilityCheckResultService _sut;
        private Mock<IEligibilityCheckResultRepository> _mockRepo;
        private Mock<ILogger<EligibilityCheckResultService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IEligibilityCheckResultRepository>();
            _mockLogger = new Mock<ILogger<EligibilityCheckResultService>>();
            _sut = new EligibilityCheckResultService(_mockRepo.Object, _mockLogger.Object);
        }

        [Test]
        public async Task SaveAsyncFailed()
        {
            var check = new Check(true, new List<IError>(), new List<IError>(), new List<IError>());
            var indesserCallSavedResult = new Result<long>();
            indesserCallSavedResult.WithError("fake error message");
            var result = await _sut.SaveAsync(check, indesserCallSavedResult);
            Assert.That(result.IsFailed);
        }

        [Test]
        public async Task SaveAsyncExceptionHandled()
        {
            _mockRepo
                .Setup(_ => _.AddCheckResult(It.IsAny<eligibility_check_result>()))
                .Throws(new Exception("fake error message"));

            var check = new Check(true, new List<IError>(), new List<IError>(), new List<IError>());
            var indesserCallSavedResult = Result.Ok(7L);
            var result = await _sut.SaveAsync(check, indesserCallSavedResult);

            Assert.That(result.IsFailed);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Equals($"Error saving eligibility check result for Indesser API call: {indesserCallSavedResult.Value}") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task SaveAsync()
        {
            var check = new Check(true, new List<IError>(), new List<IError>(), new List<IError>());
            var indesserCallSavedResult = new Result<long>();
            var result = await _sut.SaveAsync(check, indesserCallSavedResult);
            Assert.That(result.IsSuccess);
        }
    }
}