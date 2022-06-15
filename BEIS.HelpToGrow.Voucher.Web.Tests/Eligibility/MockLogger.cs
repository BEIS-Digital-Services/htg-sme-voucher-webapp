using Microsoft.Extensions.Logging;
using Moq;

namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    public static class MockLogger
    {
        public static Mock<ILoggerFactory> Factory(FakeLogger fakeLogger)
        {
            var mockLoggerFactory = new Mock<ILoggerFactory>(MockBehavior.Default);

            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(fakeLogger);

            return mockLoggerFactory;
        }
    }
}