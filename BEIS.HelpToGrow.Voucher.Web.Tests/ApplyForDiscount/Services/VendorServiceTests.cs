using System.Collections.Generic;
using System.Threading.Tasks;
using Beis.HelpToGrow.Core.Repositories.Interface;
using BEIS.HelpToGrow.Voucher.Web.Services;
using Beis.Htg.VendorSme.Database.Models;
using Moq;
using NUnit.Framework;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount.Services
{
    [TestFixture]
    public class VendorServiceTests
    {
        private VendorService _sut;
        private Mock<IVendorCompanyRepository> _mockRepo;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IVendorCompanyRepository>();
            _sut = new VendorService(_mockRepo.Object);
        }

        [Test]
        public async Task NoRegisteredVendors()
        {
            _mockRepo
                .Setup(_ => _.GetVendorCompanies())
                .Returns(Task.FromResult(new List<vendor_company>()));
            
            var result = await _sut.IsRegisteredVendor("fake companies house number");

            Assert.False(result);
        }

        [Test]
        public async Task MatchesRegisteredVendor()
        {
            _mockRepo
                .Setup(_ => _.GetVendorCompanies())
                .Returns(Task.FromResult(new List<vendor_company>
                {
                    new() { vendor_company_house_reg_no = "100" }
                }));

            var result = await _sut.IsRegisteredVendor("0100");

            Assert.True(result);
        }
    }
}