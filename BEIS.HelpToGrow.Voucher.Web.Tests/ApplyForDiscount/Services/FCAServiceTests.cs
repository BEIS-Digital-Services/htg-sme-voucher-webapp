using System.Collections.Generic;
using System.Threading.Tasks;
using Beis.HelpToGrow.Core.Repositories.Interface;
using BEIS.HelpToGrow.Voucher.Web.Services.FCAServices;
using Beis.Htg.VendorSme.Database.Models;
using Moq;
using NUnit.Framework;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount.Services
{
    [TestFixture]
    public class FCAServiceTests
    {
        private Mock<IFCASocietyRepository> _mockRepo;
        private FCASocietyService _sut;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IFCASocietyRepository>();
            _sut = new FCASocietyService(_mockRepo.Object);
        }

        [Test]
        public async Task LoadFCASocieties()
        {
            await _sut.LoadFCASocieties();

            _mockRepo.Verify(_ => _.AddSocieties(It.IsAny<List<fcasociety>>()));
        }

        [Test]
        public async Task GetSocietyMissing()
        {
            _mockRepo
                .Setup(_ => _.GetFCASocietyByNumber(It.IsAny<string>()))
                .Returns(Task.FromResult((fcasociety)null));

            var fcaSociety = await _sut.GetSociety("fake society number");

            Assert.Null(fcaSociety);
        }

        [Test]
        public async Task GetSociety()
        {
            var fakeSociety = new fcasociety
            {
                society_number = 123,
                society_suffix = "fake suffix",
                full_registration_number = "fake registration number",
                society_name = "fake society name",
                registered_as = "fake registered as",
                society_address = "fake society address",
                registration_date = "fake registration date",
                registration_act = "fake registration act",
                society_status = "fake society status"
            };

            _mockRepo
                .Setup(_ => _.GetFCASocietyByNumber(It.IsAny<string>()))
                .Returns(Task.FromResult(fakeSociety));

            var result = await _sut.GetSociety("fake society number");

            Assert.AreEqual(fakeSociety.society_number, result.SocietyNumber);
            Assert.AreEqual(fakeSociety.society_suffix, result.SocietySuffix);
            Assert.AreEqual(fakeSociety.full_registration_number, result.FullRegistrationNumber);
            Assert.AreEqual(fakeSociety.society_name, result.SocietyName);
            Assert.AreEqual(fakeSociety.registered_as, result.RegisteredAs);
            Assert.AreEqual(fakeSociety.society_address, result.SocietyAddress);
            Assert.AreEqual(fakeSociety.registration_date, result.RegistrationDate);
            Assert.AreEqual(fakeSociety.deregistration_date, result.DeregistrationDate);
            Assert.AreEqual(fakeSociety.registration_act, result.RegistrationAct);
            Assert.AreEqual(fakeSociety.society_status, result.SocietyStatus);
        }
    }
}