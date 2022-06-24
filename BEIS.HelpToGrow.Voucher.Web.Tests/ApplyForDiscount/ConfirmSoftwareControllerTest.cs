
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class ConfirmSoftwareControllerTest : BaseControllerTest
    {
        private ConfirmSoftwareController _sut;
        private Mock<ISessionService> _mockSessionService;
        private ControllerContext _controllerContext;

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _controllerContext = SetupControllerContext(_controllerContext);

            _sut = new ConfirmSoftwareController(_mockSessionService.Object);
            _sut.ControllerContext = _controllerContext;
        }

        [Test]
        public void GetIndexReturnsUserVoucherDtoModelWithSelectedProductIdAsOne()
        {
          
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(new UserVoucherDto
                {
                    SelectedProduct = new product { product_id = 3 }
                });

            var controllerResult = (ViewResult) _sut.Index();
            
            Assert.That(controllerResult.Model is UserVoucherDto);
            Assert.AreEqual(3, ((UserVoucherDto)controllerResult.Model).SelectedProduct.product_id);
        }

        [Test]
        public void Error()
        {
            var viewResult = (ViewResult) _sut.Error();

            Assert.That(viewResult.Model is ErrorViewModel);
        }
    }
}