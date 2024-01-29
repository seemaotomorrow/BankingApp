using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Moq;
using Microsoft.Extensions.Logging;
using AdminWebsite.Controllers;
using AdminWebsite.Models;

namespace AdminWebsiteTest
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly Mock<IHttpClientFactory> _clientFactoryMock;
        private readonly HttpClient _clientMock;
        private readonly HomeController _controller;
        private readonly FakeHttpMessageHandler _fakeHandler;

        public HomeControllerTests()
        {
            // Mock the logger
            _loggerMock = new Mock<ILogger<HomeController>>();

            // Mock the HttpClientFactory
            _clientFactoryMock = new Mock<IHttpClientFactory>();

            // Create a mock HttpClient with FakeHttpMessageHandler
            _fakeHandler = new FakeHttpMessageHandler();
            _clientMock = new HttpClient(_fakeHandler)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            _clientMock.DefaultRequestHeaders.Accept.Clear();
            _clientMock.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _clientFactoryMock.Setup(factory => factory.CreateClient("apiEndpoint")).Returns(_clientMock);

            // Create controller
            _controller = new HomeController(_loggerMock.Object, _clientFactoryMock.Object);
        }

        [Fact]
        public void Index_Get_ReturnsViewWithModel()
        {
            // Arrange - Nothing to arrange for this test

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AdminLoginModel>(viewResult.Model);
        }

        [Fact]
        public async Task Index_Post_ValidModel_ReturnsRedirectOnSuccess()
        {
            // Arrange
            var model = new AdminLoginModel { Username = "admin", Password = "password" };
            _fakeHandler.SetFakeResponse(HttpStatusCode.OK); // Use FakeHttpMessageHandler to set the fake response

            // Act
            var result = await _controller.Index(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Customers", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Index_Post_InvalidModel_ReturnsViewWithModelErrorOnFailure()
        {
            // Arrange
            var model = new AdminLoginModel { Username = "admin", Password = "password" };
            _fakeHandler.SetFakeResponse(HttpStatusCode.Unauthorized); // Use FakeHttpMessageHandler to set the fake response

            // Act
            var result = await _controller.Index(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.IsType<AdminLoginModel>(viewResult.Model);
        }

        [Fact]
        public void Privacy_ReturnsView()
        {
            // Arrange - Nothing to arrange for this test

            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }



       
    }
}



