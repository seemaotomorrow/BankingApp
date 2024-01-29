using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.Extensions.Logging;
using AdminWebsite.Controllers;
using AdminWebsite.Models;

namespace AdminWebsiteTest
{
    public class CustomersControllerTests
    {
        private readonly Mock<IHttpClientFactory> _clientFactoryMock;
        private readonly Mock<ILogger<CustomersController>> _loggerMock;
        private readonly HttpClient _clientMock;
        private readonly CustomersController _controller;
        private readonly FakeHttpMessageHandler _fakeHandler;

        public CustomersControllerTests()
        {
            // Mock the logger
            _loggerMock = new Mock<ILogger<CustomersController>>();

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
            _controller = new CustomersController(_clientFactoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResultWithCustomers_WhenApiResponseSuccessful()
        {
            // Arrange
            var expectedResponseContent = "[{\"CustomerId\":1,\"Name\":\"Test Customer\"}]";
            _fakeHandler.SetFakeResponse(HttpStatusCode.OK, expectedResponseContent);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var customers = Assert.IsAssignableFrom<List<CustomerDto>>(viewResult.Model);
            Assert.Single(customers);
        }
        
        [Fact]
        public async Task Edit_Get_ReturnsViewResultWithCustomer_WhenCustomerExists()
        {
            // Arrange
            int customerId = 1;
            var expectedResponseContent = "{\"CustomerId\":1,\"Name\":\"Test Customer\"}";
            _fakeHandler.SetFakeResponse(HttpStatusCode.OK, expectedResponseContent);

            // Act
            var result = await _controller.Edit(customerId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var customer = Assert.IsAssignableFrom<CustomerDto>(viewResult.Model);
            Assert.Equal(customerId, customer.CustomerID);
        }

        [Fact]
        public async Task Edit_Post_RedirectsToIndexOnSuccess()
        {
            // Arrange
            var customerToUpdate = new CustomerDto { CustomerID = 1, Name = "Updated Customer" };
            _fakeHandler.SetFakeResponse(HttpStatusCode.NoContent); // Assuming API returns NoContent on successful update

            // Act
            var result = await _controller.Edit(customerToUpdate.CustomerID, customerToUpdate);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        // handles a failed update.
        
        [Fact]
        public async Task Edit_Post_ReturnsViewResultWithCustomer_WhenUpdateFails()
        {
            // Arrange
            var customerToUpdate = new CustomerDto { CustomerID = 1, Name = "Updated Customer" };
            _fakeHandler.SetFakeResponse(HttpStatusCode.InternalServerError); // Assuming API returns an error

            // Act
            var result = await _controller.Edit(customerToUpdate.CustomerID, customerToUpdate);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<CustomerDto>(viewResult.Model);
        }

        //LockUnlockCustomer
        [Fact]
        public async Task LockUnlockCustomer_RedirectsToIndexOnSuccess()
        {
            // Arrange
            int customerId = 1;
            _fakeHandler.SetFakeResponse(HttpStatusCode.OK); // Assuming API returns OK on successful operation

            // Act
            var result = await _controller.LockUnlockCustomer(customerId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        
        // LockUnlockCustomer action always redirects to Index.
        [Fact]
        public async Task LockUnlockCustomer_AlwaysRedirectsToIndex()
        {
            // Arrange
            int customerId = 1;
            // Set a generic response, as we're not testing the API's behavior here
            _fakeHandler.SetFakeResponse(HttpStatusCode.OK);

            // Act
            var result = await _controller.LockUnlockCustomer(customerId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }


        
        
    }

   
}
