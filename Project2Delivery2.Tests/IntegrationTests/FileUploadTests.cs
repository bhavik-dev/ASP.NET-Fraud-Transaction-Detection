using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project2Delivery2.Controllers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Project2Delivery2.Tests.IntegrationTests
{
    [TestClass]
    [TestCategory("FileUpload")]
    public class FileUploadTests
    {
        private Mock<IWebHostEnvironment> _mockEnvironment;
        private FileUploadController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockEnvironment.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
            _controller = new FileUploadController(_mockEnvironment.Object);
        }

        [TestMethod]
        [Description("Verify Index action returns ViewResult")]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }


        [TestMethod]
        public void Cleanup()
        {
            _controller = null;
        }
    }
}