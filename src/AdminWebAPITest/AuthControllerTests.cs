using Microsoft.AspNetCore.Mvc;
using AdminWebAPI.Controllers;
using AdminWebAPI.Models;

namespace AdminWebAPITests
{
    public class AuthControllerTests
    {
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            // Initialize controller
            _controller = new AuthController();
        }



        [Fact]
        public void Login_WithIncorrectCredentials_ReturnsUnauthorizedResult()
        {
            // Arrange
            var login = new AdminLogin { Username = "wrong", Password = "wrong" };

            // Act
            var result = _controller.Login(login);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
