using System.Collections.Generic;
using System.Text;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using HttpContextMoq;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Services;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount.Services
{
    [TestFixture]
    public class SessionServiceTests
    {
        private SessionService _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SessionService();
        }

        [Test]
        public void Set()
        {
            var userVoucherDto = new UserVoucherDto();
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();

            mockHttpContext
                .Setup(_ => _.Session)
                .Returns(() => mockSession.Object);

            _sut.Set("fake key", userVoucherDto, mockHttpContext.Object);

            mockSession.Verify(_ => _.Set("fake key", It.IsAny<byte[]>()), Times.Once);
        }

        [Test]
        public void GetFails()
        {
            var fakeDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto {FullName = "fake full name"}
            };
            var fakeJson = JsonConvert.SerializeObject(fakeDto);
            var value = Encoding.ASCII.GetBytes(fakeJson);
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();

            mockHttpContext
                .Setup(_ => _.Session)
                .Returns(mockSession.Object);

            mockSession
                .Setup(_ => _.TryGetValue("fake key", out value))
                .Returns(false);

            var userVoucherDto = _sut.Get<UserVoucherDto>("fake key", mockHttpContext.Object);

            Assert.Null(userVoucherDto);
        }

        [Test]
        public void Get()
        {
            var fakeDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto {FullName = "fake full name"}
            };
            var fakeJson = JsonConvert.SerializeObject(fakeDto);
            var value = Encoding.ASCII.GetBytes(fakeJson);
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();

            mockHttpContext
                .Setup(_ => _.Session)
                .Returns(mockSession.Object);

            mockSession
                .Setup(_ => _.TryGetValue("fake key", out value))
                .Returns(true);

            var userVoucherDto = _sut.Get<UserVoucherDto>("fake key", mockHttpContext.Object);

            Assert.AreEqual("fake full name", userVoucherDto.ApplicantDto.FullName);
        }

        [Test]
        public void HasNoValidSession()
        {
            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .Setup(_ => _.Session)
                .Returns(new SessionMock());

            var result = _sut.HasValidSession(mockHttpContext.Object);

            Assert.False(result);
        }

        [Test]
        public void HasValidSession()
        {
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();

            mockSession
                .Setup(_ => _.IsAvailable)
                .Returns(true);

            mockSession
                .Setup(_ => _.Keys)
                .Returns(new List<string> {"fake key"});

            mockHttpContext
                .Setup(_ => _.Session)
                .Returns(mockSession.Object);

            var result = _sut.HasValidSession(mockHttpContext.Object);

            Assert.That(result);
        }

        [Test]
        public void Remove()
        {
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();

            mockHttpContext
                .Setup(_ => _.Session)
                .Returns(mockSession.Object);

            _sut.Remove("fake key", mockHttpContext.Object);
        }
    }
}