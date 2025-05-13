using ManufactureHub.Controllers;
using ManufactureHub.Data;
using ManufactureHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManufactureHub.Test.Controller
{
    public class AccountControllerAdditionalTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<RoleManager<ApplicationRole>> _roleManagerMock;
        private readonly Mock<ILogger<AccountController>> _loggerMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<ManufactureHubContext> _contextMock;
        private readonly AccountController _controller;

        public AccountControllerAdditionalTests()
        {
            // Setup UserManager mock
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Setup SignInManager mock
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object, contextAccessorMock.Object, claimsFactoryMock.Object, null, null, null, null);

            // Setup RoleManager mock
            var roleStoreMock = new Mock<IRoleStore<ApplicationRole>>();
            _roleManagerMock = new Mock<RoleManager<ApplicationRole>>(
            roleStoreMock.Object, null, null, null, null);

            _loggerMock = new Mock<ILogger<AccountController>>();
            _emailSenderMock = new Mock<IEmailSender>();
            _contextMock = new Mock<ManufactureHubContext>(new DbContextOptions<ManufactureHubContext>());

            _controller = new AccountController(
            _contextMock.Object,
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _loggerMock.Object,
            _roleManagerMock.Object,
            _emailSenderMock.Object);
        }

        [Fact]
        public async Task Login_Get_ReturnsView()
        {
            // Act
            var result = await _controller.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Returns default view
        }

        [Fact]
        public async Task Login_Post_ReturnsRedirect_WhenLoginSucceeds()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "Password123",
                RememberMe = false
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(loginModel.Email))
            .ReturnsAsync(new ApplicationUser { Email = loginModel.Email });

            _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(
            loginModel.Email, loginModel.Password, loginModel.RememberMe, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _controller.Login(loginModel, null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Task", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Login_Post_ReturnsViewWithErrors_WhenUserNotFound()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "Password123",
                RememberMe = false
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(loginModel.Email))
            .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Login(loginModel, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.Equal(loginModel, viewResult.Model);
            Assert.Contains("Невдала спроба ввійти, неправильний пароль або пошта",
            viewResult.ViewData.ModelState[""].Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public async Task Logout_ReturnsRedirect()
        {
            // Arrange
            _signInManagerMock.Setup(sm => sm.SignOutAsync())
            .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Task", redirectResult.ControllerName);
        }

        [Fact]
        public async Task UploadFile_ReturnsRelativePath_WhenFileIsValid()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "test image content";
            var fileName = "test.jpg";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            // Act
            var result = await _controller.UploadFile(fileMock.Object);

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("AvatarsImages", result);
            Assert.EndsWith(".jpg", result);
        }

        [Fact]
        public async Task UploadFile_ThrowsException_WhenFileSizeExceedsLimit()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            fileMock.Setup(f => f.Length).Returns(11 * 1024 * 1024); // 11MB, exceeds 10MB limit

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.UploadFile(fileMock.Object));
            Assert.Equal("Файл занадто великого розміру, максимум 10 mb", exception.Message);
        }

        [Fact]
        public async Task UploadFile_ThrowsException_WhenFileExtensionIsInvalid()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "test content";
            var fileName = "test.txt";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.UploadFile(fileMock.Object));
            Assert.Equal("Данний формат файла не підтримується, дозволяється: .png,.jpg,.jpeg", exception.Message);
        }

        //[Fact]
        //public async Task ForgotPassword_Post_RedirectsToConfirmation_WhenUserExistsAndEmailConfirmed()
        //{
        //    // Arrange
        //    var forgotPasswordModel = new ForgotPasswordModel
        //    {
        //        Email = "test@example.com"
        //    };

        //    var user = new ApplicationUser { Email = forgotPasswordModel.Email };
        //    _userManagerMock.Setup(um => um.FindByEmailAsync(forgotPasswordModel.Email))
        //    .ReturnsAsync(user);
        //    _userManagerMock.Setup(um => um.IsEmailConfirmedAsync(user))
        //    .ReturnsAsync(true);
        //    _userManagerMock.Setup(um => um.GeneratePasswordResetTokenAsync(user))
        //    .ReturnsAsync("reset-token");

        //    _emailSenderMock.Setup(es => es.SendEmailAsync(
        //    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //    .Returns(Task.CompletedTask);

        //    // Act
        //    var result = await _controller.ForgotPassword(forgotPasswordModel);

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("ForgotPasswordConfirmation", redirectResult.ActionName);
        //}

        [Fact]
        public async Task ForgotPassword_Post_RedirectsToConfirmation_WhenUserDoesNotExist()
        {
            // Arrange
            var forgotPasswordModel = new ForgotPasswordModel
            {
                Email = "test@example.com"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(forgotPasswordModel.Email))
            .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.ForgotPassword(forgotPasswordModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ForgotPasswordConfirmation", redirectResult.ActionName);
        }

        [Fact]
        public void NotHaveRights_ReturnsView()
        {
            // Act
            var result = _controller.NotHaveRights();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Returns default view
        }
    }
}
