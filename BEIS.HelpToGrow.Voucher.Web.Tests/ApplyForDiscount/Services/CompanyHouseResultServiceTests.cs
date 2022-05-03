using System;
using System.Threading.Tasks;
using Beis.HelpToGrow.Core.Repositories.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount.Services
{
    [TestFixture]
    public class CompanyHouseResultServiceTests
    {
        private CompanyHouseResultService _sut;
        private Mock<ICompanyHouseResultRepository> _mockRepo;
        private Mock<ILogger<CompanyHouseResultService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<ICompanyHouseResultRepository>();
            _mockLogger = new Mock<ILogger<CompanyHouseResultService>>();
            _sut = new CompanyHouseResultService(_mockRepo.Object, _mockLogger.Object);
        }

        [Test]
        public async Task SaveAsyncCompany()
        {
            _mockRepo
                .Setup(_ => _.Exists(It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            var companyHouseResponse = new CompanyHouseResponse { SicCodes = new string[]{ }};
            var result = await _sut.SaveAsync(companyHouseResponse);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public async Task SaveAsyncCompanyHandlesException()
        {
            _mockRepo
                .Setup(_ => _.Exists(It.IsAny<string>()))
                .Throws(new Exception("fake error message"));

            var result = await _sut.SaveAsync(new CompanyHouseResponse());

            Assert.That(result.IsFailed);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Error saving companies house response") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task SaveAsyncCompanyExists()
        {
            _mockRepo
                .Setup(_ => _.Exists(It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var result = await _sut.SaveAsync(new CompanyHouseResponse());

            Assert.That(result.IsSuccess);
        }
    }
}