
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class ContactUsControllerTests : BaseControllerTest
    {
        private ContactUsController _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new ContactUsController();
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult) _sut.Index();

            Assert.IsNotNull(viewResult);
        }
    }
}