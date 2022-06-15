

namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount.Services
{
    [TestFixture]
    public class EmailClientServiceTests
    {
        [Test]
        public void CtorMissingValue()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var emailClientService = new EmailClientService(null);

                Assert.Null(emailClientService);
            });
        }

        [Test]
        public void SendEmailAsyncMissingApiKey()
        {
            var mockOptions = new Mock<INotifyServiceSettings>();

            var sut = new EmailClientService(mockOptions.Object);

            Assert.ThrowsAsync<NullReferenceException>(() => sut.SendEmailAsync("fake@domain.org", string.Empty, new Dictionary<string, object>()));
        }

        [Test]
        public void SendEmailAsyncBadApiKey()
        {
            var mockOptions = new Mock<INotifyServiceSettings>();
            mockOptions
                .Setup(_ => _.NotifyApiKey)
                .Returns("fake key");

            var sut = new EmailClientService(mockOptions.Object);

            Assert.ThrowsAsync<NotifyAuthException>(() => sut.SendEmailAsync("fake@domain.org", string.Empty, new Dictionary<string, object>()));
        }
    }

    [TestFixture]
    public class CookieServiceTests
    {
        private CookieService _sut;
        private CookieNamesConfiguration _cookieNamesConfiguration;
        private Mock<IOptions<CookieNamesConfiguration>> _mockOptions;

        [SetUp]
        public void Setup()
        {
            _mockOptions = new Mock<IOptions<CookieNamesConfiguration>>();
            _cookieNamesConfiguration = new CookieNamesConfiguration
            {
                NonEssential = new Dictionary<string, string>
                {
                    {"HtGAnalyticsCookie", "fake analytics cookie"},
                    {"HtGMarketingCookie", "fake marketing cookie"}
                },
                Essential = new Dictionary<string, string>
                {
                    {"HtGcookieAcceptedCookie", "fake accepted cookie"}
                }
            };
            _mockOptions
                .Setup(_ => _.Value)
                .Returns(_cookieNamesConfiguration);

            _sut = new CookieService(_mockOptions.Object);
        }

        [Test]
        public void SyncCookieSelection()
        {
            var mockHttpRequest = new Mock<HttpRequest>();
            var mockCookies = new RequestCookieCollectionMock();

            mockHttpRequest
                .Setup(_ => _.Cookies)
                .Returns(() => mockCookies);

            var cookieBannerViewModel = new CookieBannerViewModel
            {
                GoogleAnalyticsCookieAccepted = "fake response"
            };

            _sut.SyncCookieSelection(mockHttpRequest.Object, cookieBannerViewModel);
        }

        [Test]
        public async Task ProcessCookieNotAccepted()
        {
            var mockHttpResponse = new Mock<HttpResponse>();
            var result = await _sut.ProcessCookie("fake cookie type", null, mockHttpResponse.Object);

            Assert.True(result);
        }

        [Test]
        public async Task ProcessCookieNotActType()
        {
            var mockHttpResponse = new Mock<HttpResponse>();
            var result = await _sut.ProcessCookie("fake cookie type", true, mockHttpResponse.Object);

            Assert.True(result);
        }

        [Test]
        public async Task ProcessCookie()
        {
            var mockHttpResponse = new Mock<HttpResponse>();

            var mockResponseCookies = new Mock<IResponseCookies>();

            mockHttpResponse
                .Setup(_ => _.Cookies)
                .Returns(mockResponseCookies.Object);

            var result = await _sut.ProcessCookie("act", true, mockHttpResponse.Object);

            Assert.True(result);

            mockResponseCookies.Verify(_ => _.Append("fake analytics cookie", "true", It.IsAny<CookieOptions>()), Times.Once);
            mockResponseCookies.Verify(_ => _.Append("fake marketing cookie", "true", It.IsAny<CookieOptions>()), Times.Once);
            mockResponseCookies.Verify(_ => _.Append("fake accepted cookie", "true", It.IsAny<CookieOptions>()), Times.Once);
        }

        [Test]
        public async Task SaveCookiesPreferences()
        {
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();
            var mockHttpResponse = new Mock<HttpResponse>();
            var mockResponseCookies = new Mock<IResponseCookies>();

            var fields = new Dictionary<string, StringValues>
            {
                {"GoogleAnalyticsCookieAccepted", new StringValues(new []{ "Gx" })},
                {"MarketingCookieAccepted", new StringValues(new []{ "Mx" })},
            };

            var fakeFormCollection = new FormCollection(fields);

            mockHttpRequest
                .Setup(_ => _.Form)
                .Returns(fakeFormCollection);

            mockHttpContext
                .Setup(_ => _.Request)
                .Returns(mockHttpRequest.Object);

            mockHttpContext
                .Setup(_ => _.Response)
                .Returns(mockHttpResponse.Object);

            mockHttpResponse
                .Setup(_ => _.Cookies)
                .Returns(() => mockResponseCookies.Object);

            var result = await _sut.SaveCookiesPreferences(mockHttpContext.Object);

            Assert.True(result);

            mockResponseCookies.Verify(_ => _.Append("fake analytics cookie", "Gx", It.IsAny<CookieOptions>()), Times.Once);
            mockResponseCookies.Verify(_ => _.Append("fake marketing cookie", "Mx", It.IsAny<CookieOptions>()), Times.Once);
            mockResponseCookies.Verify(_ => _.Append("fake accepted cookie", "true", It.IsAny<CookieOptions>()), Times.Once);
        }
    }
}