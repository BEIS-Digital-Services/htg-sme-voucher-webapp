﻿using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class TaskListControllerTest : BaseControllerTest
    {
        private TaskListController _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new TaskListController();
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult)_sut.Index();

            Assert.AreEqual(0, viewResult.ViewData.Count);
        }
    }
}