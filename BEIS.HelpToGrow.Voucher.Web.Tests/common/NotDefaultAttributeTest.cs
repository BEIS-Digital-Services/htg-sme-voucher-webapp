using System.Threading.Tasks;
using BEIS.HelpToGrow.Voucher.Web.Common;
using BEIS.HelpToGrow.Voucher.Web.Models.CompaniesHouse;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.Common
{
    internal class NoOpResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.CompletedTask;
        }
    }

    [TestFixture]
    public class NotDefaultAttributeTest
    {
        private NotDefaultAttribute _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new NotDefaultAttribute();
        }

        [Test]
        public void IsNotValid()
        {
            const int zero = 0;

            Assert.False(_sut.IsValid(zero));
        }

        [Test]
        public void IsValidNull()
        {
            Assert.That(_sut.IsValid(null));
        }

        [Test]
        public void IsValidInt()
        {
            Assert.That(_sut.IsValid(3));
        }

        [Test]
        public void IsValidViewModel()
        {
            Assert.That(_sut.IsValid(new CompaniesHouseNumberViewModel()));
        }
    }
}