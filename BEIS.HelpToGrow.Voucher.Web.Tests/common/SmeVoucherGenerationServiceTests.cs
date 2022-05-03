using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beis.HelpToGrow.Core.Repositories.Interface;
using BEIS.HelpToGrow.Voucher.Web.Common;
using BEIS.HelpToGrow.Voucher.Web.Services;
using BEIS.HelpToGrow.Voucher.Web.Services.Interfaces;
using Beis.Htg.VendorSme.Database.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.Common
{
    [TestFixture]
    public class SmeVoucherGenerationServiceTests
    {
        private Mock<ILogger<VoucherGenerationService>> _mockLogger;
        private IVoucherGenerationService _smeVoucherGenerationService;
        private IEncryptionService _encryptionService;
        private const string VoucherCodeLength = "9";

        [SetUp]
        public void Setup()
        {
            var repositoryTokens = new List<token>();
            
            var mockTokenRepository = new Mock<ITokenRepository>();
            mockTokenRepository.Setup(x => x.AddToken(It.IsAny<token>()));
            mockTokenRepository.Setup(x => x.AddToken(It.IsAny<token>())).Callback((token t) => repositoryTokens.Add(t));
            mockTokenRepository.Setup(x => x.GetToken(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync((long eid, long pid) => repositoryTokens.FirstOrDefault(x => x.enterprise_id == eid && x.product == pid));

            var mockConfiguration = new Mock<IConfiguration>();
         

            mockConfiguration.Setup(_ => _["VoucherSettings:voucherCodeLength"]).Returns(VoucherCodeLength);

            _mockLogger = new Mock<ILogger<VoucherGenerationService>>();
            _encryptionService = new EncryptionService("Dkjkjjkjii", 2, "AVRlp67e*baz44sR", 256);
            _smeVoucherGenerationService = new VoucherGenerationService(mockConfiguration.Object, _encryptionService, mockTokenRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task MustGenerateVoucher()
        {
            var vendorCompany = new vendor_company()
            {
                vendorid = 12345,
                registration_id = "V12345"
            };

            var enterprise = new enterprise()
            {
                enterprise_id = 345
            };

            var product = new product()
            {
                product_id = 123,
                price = "200"
            };

            var voucherCode = await _smeVoucherGenerationService.GenerateVoucher(vendorCompany, enterprise, product);
            var reconstitutedVoucherCode = $"{voucherCode}==";
            var decrypted = _encryptionService.Decrypt(reconstitutedVoucherCode, vendorCompany.registration_id + vendorCompany.vendorid);

            Console.WriteLine(voucherCode);
            Assert.AreEqual(VoucherCodeLength, "" + decrypted.Length);
        }

        [Test]
        public async Task MustNotGenerateDuplicateVoucher()
        {
            // if the voucher has already been created for the enterprise/product then return the existing code.

            var vendor_company = new vendor_company()
            {
                vendorid = 1,
                registration_id = "12345"
            };
            var enterprise = new enterprise()
            {
                enterprise_id = 345
            };
            var product = new product()
            {
                product_id = 123,
                price = "200"
            };

            var voucherCode1 = await _smeVoucherGenerationService.GenerateVoucher(vendor_company, enterprise, product);
            var voucherCode2 = await _smeVoucherGenerationService.GenerateVoucher(vendor_company, enterprise, product);            

            Assert.AreEqual(voucherCode1, voucherCode2);
        }

        [Test]
        public void MustGenerateSetCode()
        {
            var setCode = _smeVoucherGenerationService.GenerateSetCode(Int32.Parse(VoucherCodeLength));
            
            Assert.AreEqual(9, setCode.Length);
        }
    }
}