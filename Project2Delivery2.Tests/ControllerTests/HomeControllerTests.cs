using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project2Delivery2.Controllers;

namespace Project2Delivery2.Tests.ControllerTests
{
    [TestClass]
    [TestCategory("Controller")]
    public class HomeControllerTests
    {
        private HomeController _controller;

        [TestInitialize]
        public void Setup()
        {
            _controller = new HomeController();
        }

        [TestMethod]
        [Description("Verify Index action returns ViewResult")]
        [TestCategory("UnitTest")]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _controller = null;
        }
    }
}
