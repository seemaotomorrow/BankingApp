using Microsoft.AspNetCore.Mvc;
using Moq;
using AdminWebAPI.Repositories;
using AdminWebAPI.Models;


namespace AdminWebAPITests
{
    public class LoginControllerTests
    {
        private readonly Mock<ILoginRepository> _loginRepositoryMock;
        private readonly LoginController _controller;

        public LoginControllerTests()
        {
            // Mock the login repository
            _loginRepositoryMock = new Mock<ILoginRepository>();
            _controller = new LoginController(_loginRepositoryMock.Object);
        }

        [Fact]
        public async Task GetLoginById_ExistingId_ReturnsLogin()
        {
            // Arrange
            var loginId = "existingId";
            var expectedLogin = new Login { LoginID = loginId, PasswordHash = "hash" };
            _loginRepositoryMock.Setup(repo => repo.GetByLoginIDAsync(loginId)).ReturnsAsync(expectedLogin);

            // Act
            var result = await _controller.GetLoginById(loginId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedLogin, actionResult.Value);
        }

        [Fact]
        public async Task GetLoginById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var loginId = "nonExistingId";
            _loginRepositoryMock.Setup(repo => repo.GetByLoginIDAsync(loginId)).ReturnsAsync((Login)null);

            // Act
            var result = await _controller.GetLoginById(loginId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateLogin_ValidLogin_ReturnsCreatedAtAction()
        {
            // Arrange
            var newLogin = new Login { LoginID = "newLogin", PasswordHash = "newHash" };
            _loginRepositoryMock.Setup(repo => repo.CreateAsync(newLogin)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateLogin(newLogin);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(newLogin, createdAtActionResult.Value);
            Assert.Equal("GetLoginById", createdAtActionResult.ActionName);
        }

        [Fact]
        public async Task UpdateLogin_ExistingId_UpdatesPassword()
        {
            // Arrange
            var loginId = "existingId";
            var newPasswordHash = "newHash";
            var login = new Login { LoginID = loginId, PasswordHash = "oldHash" };
            _loginRepositoryMock.Setup(repo => repo.GetByLoginIDAsync(loginId)).ReturnsAsync(login);
            _loginRepositoryMock.Setup(repo => repo.UpdatePasswordAsync(loginId, newPasswordHash)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateLogin(loginId, newPasswordHash);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _loginRepositoryMock.Verify(repo => repo.UpdatePasswordAsync(loginId, newPasswordHash), Times.Once);
        }

        [Fact]
        public async Task DeleteLogin_ExistingId_DeletesLogin()
        {
            // Arrange
            var loginId = "existingId";
            var login = new Login { LoginID = loginId, PasswordHash = "hash" };
            _loginRepositoryMock.Setup(repo => repo.GetByLoginIDAsync(loginId)).ReturnsAsync(login);
            _loginRepositoryMock.Setup(repo => repo.DeleteAsync(loginId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteLogin(loginId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _loginRepositoryMock.Verify(repo => repo.DeleteAsync(loginId), Times.Once);
        }

        [Fact]
        public async Task CreateLogin_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var newLogin = new Login { LoginID = "newLogin", PasswordHash = "" }; // Assume empty password is invalid
            _controller.ModelState.AddModelError("PasswordHash", "PasswordHash is required");

            // Act
            var result = await _controller.CreateLogin(newLogin);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }
        
        [Fact]
        public async Task UpdateLogin_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var loginId = "nonExistingId";
            var newPasswordHash = "newHash";
            _loginRepositoryMock.Setup(repo => repo.GetByLoginIDAsync(loginId)).ReturnsAsync((Login)null);

            // Act
            var result = await _controller.UpdateLogin(loginId, newPasswordHash);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }


    }
}
