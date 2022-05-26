using System;
using System.Threading.Tasks;
using Beis.HelpToGrow.Core.Repositories.Interface;
using BEIS.HelpToGrow.Voucher.Web.Config;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using BEIS.HelpToGrow.Voucher.Web.Models.Applicant;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;
using BEIS.HelpToGrow.Voucher.Web.Services.FCAServices;
using Beis.Htg.VendorSme.Database.Models;
using Microsoft.Extensions.Options;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class ConfirmApplicantControllerTest : BaseControllerTest
    {
        private ConfirmApplicantController _sut;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IProductRepository> _productRepository;
        private ControllerContext _controllerContext;
        private Mock<IFCASocietyService> _mockFCASocietyService;
        private Mock<IProductPriceService> _mockProductPriceService;

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _productRepository = new Mock<IProductRepository>();
            _controllerContext = SetupControllerContext(_controllerContext);
            _mockFCASocietyService = new Mock<IFCASocietyService>();
            _mockProductPriceService = new Mock<IProductPriceService>();

            _sut = new ConfirmApplicantController(
                _mockSessionService.Object,
                _mockFCASocietyService.Object,
                _mockProductPriceService.Object,
                Options.Create(new UrlOptions { LearningPlatformUrl = "https://test-fake-webapp.azurewebsites.net/" }));

            _sut.ControllerContext = _controllerContext;

            SetupProductRepository(_productRepository);
        }

        [Test]
        public async Task GetIndex()
        {
            var userVoucherDto = new UserVoucherDto
            {
                HasFCANumber = "yes",
                FCANumber = "fake FCA number",
                SelectedProduct = new product(),
                ApplicantDto = new ApplicantDto
                {
                    HasAcceptedTermsAndConditions = true,
                    HasAcceptedPrivacyPolicy = true,
                    HasAcceptedSubsidyControl = true
                }
            };
            
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(userVoucherDto);

            var fcaSociety = new FCASociety
            {
                SocietyName = "fake society name",
                FullRegistrationNumber = "fake registration number"
            };

            _mockFCASocietyService
                .Setup(_ => _.GetSociety(It.IsAny<string>()))
                .Returns(Task.FromResult(fcaSociety));

            _mockProductPriceService
                .Setup(_ => _.GetProductPrice(It.IsAny<long>()))
                .Returns(Task.FromResult("fake product price"));

            var index = await _sut.Index();
            var viewResult = (ViewResult) index;

            if (viewResult.Model is ConfirmApplicantViewModel viewModel)
            {
                Assert.AreEqual("fake society name", viewModel.CompanyName);
                Assert.AreEqual("fake registration number", viewModel.CompanyNumber);
                return;
            }

            Assert.Fail($"View model is not {nameof(ConfirmApplicantViewModel)}");
        }

        [Test]
        public async Task GetIndexReturnsConfirmApplicantViewModelWithFullName()
        {
            var expectedModel =
                await SetupSelection(_productRepository,1,1,"", "", "", "", "", "", "", "", 
                    new ApplicantDto 
                    {
                        FullName = "Full Name",
                        HasAcceptedPrivacyPolicy = true,
                        HasAcceptedSubsidyControl = true,
                        HasAcceptedTermsAndConditions = true
                    });

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            _mockProductPriceService
                .Setup(_ => _.GetProductPrice(It.IsAny<long>()))
                .Returns(Task.FromResult("fake product price"));

            var index = await _sut.Index();
            var controllerResult = (ViewResult)index;
            
            Assert.That(((ConfirmApplicantViewModel) controllerResult.Model).FullName == "Full Name");
        }

        [Test]
        public async Task GetIndexReturnsConfirmApplicantViewModelWithRole()
        {
            var expectedModel =
                await SetupSelection(_productRepository, 1, 1, "", "", "", "", "", "", "", "",
                    new ApplicantDto 
                    { 
                        Role = "Role Name",
                        HasAcceptedPrivacyPolicy = true,
                        HasAcceptedSubsidyControl = true,
                        HasAcceptedTermsAndConditions = true
                    });

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            _mockProductPriceService
                .Setup(_ => _.GetProductPrice(It.IsAny<long>()))
                .Returns(Task.FromResult("fake product price"));

            var index = await _sut.Index();
            var controllerResult = (ViewResult)index;

            Assert.That(((ConfirmApplicantViewModel)controllerResult.Model).Role == "Role Name");
        }

        [Test]
        public async Task GetIndexReturnsConfirmApplicantViewModelWithEmailAddress()
        {
            var expectedModel =
                await SetupSelection(_productRepository, 1, 1, "", "", "", "", "", "", "", "",
                    new ApplicantDto 
                    { 
                        EmailAddress = "Email@Address.com", 
                        HasAcceptedPrivacyPolicy = true, 
                        HasAcceptedSubsidyControl = true, 
                        HasAcceptedTermsAndConditions = true 
                    });

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            _mockProductPriceService
                .Setup(_ => _.GetProductPrice(It.IsAny<long>()))
                .Returns(Task.FromResult("fake price description"));

            var index = await _sut.Index();
            var controllerResult = (ViewResult)index;

            Assert.That(((ConfirmApplicantViewModel)controllerResult.Model).EmailAddress == "Email@Address.com");
        }

        [Test]
        public async Task GetIndexReturnsConfirmApplicantViewModelWithHasTermsAndConditionsAsFalse()
        {
            var expectedModel =
                await SetupSelection(_productRepository, 0, 0, "", "", "", "", "", "", "", "",
                    new ApplicantDto 
                    { 
               
                        HasAcceptedPrivacyPolicy = true,
                        HasAcceptedSubsidyControl = true,
                        HasAcceptedTermsAndConditions = false
                    });

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            var controllerResult = await _sut.Index();

            Assert.That(controllerResult is RedirectToActionResult);
            Assert.That((controllerResult as RedirectToActionResult).ControllerName == "TermsAndConditions");
        }

        [Test]
        public async Task GetIndexReturnsConfirmApplicantViewModelWithHasPrivacyPolicyAsFalse()
        {
            var expectedModel =
                await SetupSelection(_productRepository, 0, 0, "", "", "", "", "", "", "", "",
                    new ApplicantDto 
                    {
                        HasAcceptedPrivacyPolicy = false,
                        HasAcceptedSubsidyControl = true,
                        HasAcceptedTermsAndConditions = true
                    });

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            var controllerResult = await _sut.Index();

            Assert.That(controllerResult is RedirectToActionResult);
            Assert.That((controllerResult as RedirectToActionResult).ControllerName == "TermsAndConditions");
        }

        [Test]
        public async Task GetIndexReturnsConfirmApplicantViewModelWithHasTSubsidiaryControlAsFalse()
        {
            var expectedModel =
                await SetupSelection(_productRepository, 0, 0, "", "", "", "", "", "", "", "",
                    new ApplicantDto 
                    {
                        HasAcceptedPrivacyPolicy = true,
                        HasAcceptedSubsidyControl = false,
                        HasAcceptedTermsAndConditions = true
                    });

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            var controllerResult = await _sut.Index();

            Assert.That(controllerResult is RedirectToActionResult);
            Assert.That((controllerResult as RedirectToActionResult).ControllerName == "TermsAndConditions");
        }

        [Test]
        public async Task GetIndexReturnsConfirmApplicantViewModelWithHasTSubsidiaryControlTermsAndConditionsAndPrivacyPolicyAsTrue()
        {
            var expectedModel =
                await SetupSelection(_productRepository, 1, 1, "", "", "", "", "", "", "", "",
                    new ApplicantDto
                    {
                        HasAcceptedPrivacyPolicy = true,
                        HasAcceptedSubsidyControl = true,
                        HasAcceptedTermsAndConditions = true
                    });

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            _mockProductPriceService
                .Setup(_ => _.GetProductPrice(It.IsAny<long>()))
                .Returns(Task.FromResult("fake price description"));

            var index = await _sut.Index();
            var controllerResult = (ViewResult) index;

            Assert.That(((ConfirmApplicantViewModel)controllerResult.Model).HasAcceptedSubsidyControl);
        }
    }
}