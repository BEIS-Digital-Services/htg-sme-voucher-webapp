
namespace Beis.HelpToGrow.Voucher.Web.Tests.Common
{
    [TestFixture]
    public class TokenIssuedUrlGenerationTest
    {
        [Test]
        [TestCase("https://fakedomain.com/fake-platform", "Xx1Bgg7yqpNfLQOBpi1NOQ")]
        public void ShouldGenerateAValidUrl(string url, string token)
        {
            var param = new Dictionary<string, string>() { { "grantToken", token } };
            var result = new Uri(QueryHelpers.AddQueryString(url, param));

            Assert.AreEqual("https://fakedomain.com/fake-platform?grantToken=Xx1Bgg7yqpNfLQOBpi1NOQ", result.ToString());
        }
    }
}