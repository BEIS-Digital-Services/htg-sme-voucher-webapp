using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using BEIS.HelpToGrow.Voucher.Web.Models.CompaniesHouse;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility;


namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class CompaniesHouseNumberControllerTest : BaseControllerTest
    {
        private CompaniesHouseNumberController _sut;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IVendorService> _mockVendorService;
        private Mock<IEnterpriseService> _mockEnterpriseService;
        private Mock<IProductRepository> _productRepository;
        private Mock<ICompanyHouseHttpConnection<CompanyHouseResponse>> _apiHttpConnection;
        private Mock<ICompanyHouseResultService> _companyHouseResultService;
        private ControllerContext _controllerContext;
        private Mock<IApplicationStatusService> _mockApplicationStatusService;
        private ApplicationStatus _applicationStatus = ApplicationStatus.NewApplication;

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _mockVendorService = new Mock<IVendorService>();
            _mockEnterpriseService = new Mock<IEnterpriseService>();
            _productRepository = new Mock<IProductRepository>();
            _controllerContext = SetupControllerContext(_controllerContext);
            _apiHttpConnection = new Mock<ICompanyHouseHttpConnection<CompanyHouseResponse>>();
            _companyHouseResultService = new Mock<ICompanyHouseResultService>();
            SetupProductRepository(_productRepository);
            _mockApplicationStatusService = new Mock<IApplicationStatusService>();
            _mockApplicationStatusService.Setup(x => x.GetApplicationStatus(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string s1, string s2) => _applicationStatus);
            _sut = NewCompaniesHouseNumberController();
            _sut.ControllerContext = _controllerContext;
            
        }

        [Test]
        public void Index()
        {
            var userVoucherDto = new UserVoucherDto
            {
                HasCompanyHouseNumber = "fake response #1",
                CompanySize = "fake response #2"
            };

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(userVoucherDto);

            var viewResult = (ViewResult)_sut.Index();

            if (viewResult.Model is CompaniesHouseViewModel viewModel)
            {
                Assert.AreEqual("fake response #1", viewModel.HasCompaniesHouseNumber);
                Assert.AreEqual("fake response #2", viewModel.CompanySize);
                return;
            }

            Assert.Fail($"Expected {nameof(CompaniesHouseViewModel)} view model");
        }

        [Test]
        public async Task CheckCompanyDetailsInvalidModel()
        {
            var result = await _sut.CheckCompanyDetails(new CompaniesHouseNumberViewModel());

            var viewResult = (ViewResult) result;

            Assert.That(viewResult.Model is CompaniesHouseNumberViewModel);
            Assert.AreEqual("CompaniesHouseNumber", viewResult.ViewName);
        }

        [Test]
        public async Task CheckCompanyDetailsGivenExistingCompany()
        {
            _applicationStatus = ApplicationStatus.EmailVerified;
            var voucherDto = await SetupSelection(_productRepository, 1, 1, "Yes", "", "", "Yes");

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(voucherDto);
            _mockEnterpriseService.Setup(x => x.CompanyNumberIsUnique(It.IsAny<string>())).Returns(Task.FromResult(false));

            var companiesHouseNumberViewModel = new CompaniesHouseNumberViewModel { Number = "123" };
            var viewResult = (ViewResult) await _sut.CheckCompanyDetails(companiesHouseNumberViewModel);

            Assert.AreEqual("CompanyAlreadyExists", viewResult.ViewName);
        }

        [Test]
        public async Task CheckCompanyDetailsGivenVendorFound()
        {
            await SetupSelection(_productRepository, 1, 1, "Yes", "", "", "Yes");

            _mockEnterpriseService.Setup(_ => _.CompanyNumberIsUnique(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockVendorService.Setup(_ => _.IsRegisteredVendor(It.IsAny<string>())).Returns(Task.FromResult(true));

            var actionResult = (RedirectToActionResult) await _sut.CheckCompanyDetails(new CompaniesHouseNumberViewModel { Number = "123" });

            Assert.AreEqual("InEligible", actionResult.ControllerName);
            Assert.AreEqual("Vendor", actionResult.ActionName);
        }

        [Test]
        public async Task CheckCompanyDetailInvalidCompaniesHouseNumber()
        {
            await SetupSelection(_productRepository, 1, 1, "Yes", "", "", "Yes");

            _mockEnterpriseService.Setup(_ => _.CompanyNumberIsUnique(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockVendorService.Setup(_ => _.IsRegisteredVendor(It.IsAny<string>())).Returns(Task.FromResult(false));
            _apiHttpConnection.Setup(_ => _.ProcessRequest(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(new CompanyHouseResponse { HttpStatusCode = HttpStatusCode.OK });

            var viewResult = (ViewResult) await _sut.CheckCompanyDetails(new CompaniesHouseNumberViewModel { Number = "123" });

            Assert.AreEqual("CompaniesHouseNumber", viewResult.ViewName);
        }

        [Test]
        public async Task CheckCompanyDetailCompaniesHouseNumberNotFound()
        {
            await SetupSelection(_productRepository, 1, 1, "Yes", "", "", "Yes");

            _mockEnterpriseService.Setup(_ => _.CompanyNumberIsUnique(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockVendorService.Setup(_ => _.IsRegisteredVendor(It.IsAny<string>())).Returns(Task.FromResult(false));
            _apiHttpConnection.Setup(_ => _.ProcessRequest(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(new CompanyHouseResponse { HttpStatusCode = HttpStatusCode.NotFound });

            var viewResult = (ViewResult)_sut.CheckCompanyDetails(new CompaniesHouseNumberViewModel { Number = "123" }).Result;

            Assert.AreEqual("CompaniesHouseNotFound", viewResult.ViewName);
        }

        [Test]
        public void CheckCompanyDetailServiceUnavailable()
        {
            _mockEnterpriseService.Setup(_ => _.CompanyNumberIsUnique(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockVendorService.Setup(_ => _.IsRegisteredVendor(It.IsAny<string>())).Returns(Task.FromResult(false));
            _apiHttpConnection.Setup(_ => _.ProcessRequest(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(new CompanyHouseResponse { HttpStatusCode = HttpStatusCode.ServiceUnavailable });

            var viewResult = (ViewResult)_sut.CheckCompanyDetails(new CompaniesHouseNumberViewModel { Number = "123" }).Result;

            Assert.AreEqual("ServiceUnavailable", viewResult.ViewName);
        }

        [Test]
        public async Task CheckCompanyDetailSetsCompaniesHouseResponse()
        {
            var voucherDto = await SetupSelection(_productRepository, 1, 1, "Yes", "", "", "Yes");

            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(voucherDto);
            _mockEnterpriseService.Setup(_ => _.CompanyNumberIsUnique(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockVendorService.Setup(_ => _.IsRegisteredVendor(It.IsAny<string>())).Returns(Task.FromResult(false));
            _apiHttpConnection.Setup(_ => _.ProcessRequest(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(new CompanyHouseResponse { HttpStatusCode = HttpStatusCode.OK, CompanyNumber = "XYZ" });

            var viewResult = (ViewResult)await _sut.CheckCompanyDetails(new CompaniesHouseNumberViewModel { Number = "XYZ" });

            _mockSessionService.Verify(_ => _.Set("userVoucherDto", It.IsAny<UserVoucherDto>(), It.IsAny<HttpContext>()));

            Assert.AreEqual("XYZ", ((CompaniesHouseNumberViewModel)viewResult.Model).CompanyHouseResponse.CompanyNumber);
        }

        [Test]
        public async Task CheckCompanyDetailHandlesErrorSavingResponse()
        {
            var voucherDto = await SetupSelection(_productRepository, 1, 1, "Yes", "", "", "Yes");

            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(voucherDto);
            _mockEnterpriseService.Setup(_ => _.CompanyNumberIsUnique(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockVendorService.Setup(_ => _.IsRegisteredVendor(It.IsAny<string>())).Returns(Task.FromResult(false));
            _apiHttpConnection.Setup(_ => _.ProcessRequest(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(new CompanyHouseResponse { HttpStatusCode = HttpStatusCode.OK, CompanyNumber = "XYZ" });
            _companyHouseResultService.Setup(_ => _.SaveAsync(It.IsAny<CompanyHouseResponse>())).Throws(new Exception("fake error message"));

            Assert.ThrowsAsync<Exception>(async () => await _sut.CheckCompanyDetails(new CompaniesHouseNumberViewModel { Number = "XYZ" }));

            _mockSessionService.Verify(_ => _.Set("userVoucherDto", It.IsAny<UserVoucherDto>(), It.IsAny<HttpContext>()));
        }

        [Test]

        public async Task CheckCompanyDetailsGivenExistingCancelledCannotReapplyCompany()
        {
            _applicationStatus = ApplicationStatus.CancelledCannotReApply;
            var voucherDto = await SetupSelection(_productRepository, 1, 1, "Yes", "", "", "Yes");

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(voucherDto);
            _mockEnterpriseService.Setup(x => x.CompanyNumberIsUnique(It.IsAny<string>())).Returns(Task.FromResult(false));

            var companiesHouseNumberViewModel = new CompaniesHouseNumberViewModel { Number = "123" };
            var viewResult = await _sut.CheckCompanyDetails(companiesHouseNumberViewModel);
            Assert.IsTrue(viewResult is RedirectToActionResult);
            Assert.AreEqual("Vendor", (viewResult as RedirectToActionResult).ActionName);
            Assert.AreEqual("InEligible", (viewResult as RedirectToActionResult).ControllerName);
        }

        private CompaniesHouseNumberController NewCompaniesHouseNumberController() =>
            new(_mockSessionService.Object,
                _apiHttpConnection.Object,
                _companyHouseResultService.Object,
                _mockVendorService.Object,
                _mockEnterpriseService.Object,
                _mockApplicationStatusService.Object);
    }
}