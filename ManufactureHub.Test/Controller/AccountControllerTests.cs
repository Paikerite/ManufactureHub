using FakeItEasy;
using ManufactureHub.Controllers;
using ManufactureHub.Data;
using ManufactureHub.Data.Enums;
using ManufactureHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ManufactureHub.Test.Controller
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<RoleManager<ApplicationRole>> _roleManagerMock;
        private readonly Mock<ILogger<AccountController>> _loggerMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<ManufactureHubContext> _contextMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            // Setup UserManager mock
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Setup SignInManager mock
            var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
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


        //[Fact]
        //public void AccountControllerTests_Index_ReturnSuccess()
        //{

        //    //Arrange - What do i need to bring in?
        //    var userAccountViewModel = A.Fake<ApplicationUser>();
        //    A.CallTo(() => userManager.FindByIdAsync("5")).Return(userAccountViewModel);
        //    //Act

        //    //Assert
        //}

        //[Fact]
        //public async Task Index_ReturnsViewWithUsers_WhenUsersExist()
        //{
        //    // Arrange
        //    var users = new List<ApplicationUser>
        //        {
        //        new ApplicationUser { Id = 1, SurName = "Smith", Email = "smith@example.com" },
        //        new ApplicationUser { Id = 2, SurName = "Jones", Email = "jones@example.com" }
        //        }.AsQueryable();

        //    _userManagerMock.Setup(um => um.Users).Returns(users);
        //    _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>()))
        //    .ReturnsAsync(new List<string> { "Admin" });

        //    _roleManagerMock.Setup(rm => rm.FindByNameAsync("Admin"))
        //    .ReturnsAsync(new ApplicationRole { Name = "Admin", RoleName = "Адмін" });

        //    // Act
        //    var result = await _controller.Index();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsAssignableFrom<List<ApplicationUser>>(viewResult.Model);
        //    Assert.Equal(2, model.Count);
        //}

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
        public async Task Login_Post_ReturnsViewWithErrors_WhenLoginFails()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "WrongPassword",
                RememberMe = false
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(loginModel.Email))
            .ReturnsAsync(new ApplicationUser { Email = loginModel.Email });

            _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(
            loginModel.Email, loginModel.Password, loginModel.RememberMe, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _controller.Login(loginModel, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.Equal(loginModel, viewResult.Model);
        }

        [Fact]
        public async Task Create_Post_ReturnsRedirect_WhenUserCreationSucceeds()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Name = "John",
                SurName = "Doe",
                PatronymicName = "Smith",
                Email = "john.doe@example.com",
                Position = "Developer",
                Password = "Password123",
                ConfirmPassword = "Password123",
                Role = Roles.Worker
            };

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), registerModel.Password))
            .ReturnsAsync(IdentityResult.Success);

            _roleManagerMock.Setup(rm => rm.FindByNameAsync(Roles.Worker.ToString()))
            .ReturnsAsync(new ApplicationRole { Name = Roles.Worker.ToString() });

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Roles.Worker.ToString()))
            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Create(registerModel, null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Edit_Post_ReturnsRedirect_WhenUpdateSucceeds()
        {
            // Arrange
            var editModel = new EditUserModel
            {
                Id = 1,
                Name = "Jane",
                SurName = "Doe",
                PatronymicName = "Smith",
                Position = "Manager",
                Role = Roles.Admin
            };

            var user = new ApplicationUser
            {
                Id = 1,
                Name = "John",
                SurName = "Doe",
                PatronymicName = "Smith",
                Position = "Developer"
            };

            _userManagerMock.Setup(um => um.FindByIdAsync("1"))
            .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { Roles.Worker.ToString() });

            _roleManagerMock.Setup(rm => rm.FindByNameAsync(Roles.Worker.ToString()))
            .ReturnsAsync(new ApplicationRole { Name = Roles.Worker.ToString() });

            _roleManagerMock.Setup(rm => rm.FindByNameAsync(Roles.Admin.ToString()))
            .ReturnsAsync(new ApplicationRole { Name = Roles.Admin.ToString() });

            _userManagerMock.Setup(um => um.RemoveFromRoleAsync(user, Roles.Worker.ToString()))
            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(user, Roles.Admin.ToString()))
            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

            _signInManagerMock.Setup(sm => sm.RefreshSignInAsync(user))
            .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(1, editModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirect_WhenDeletionSucceeds()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = 1,
                Email = "test@example.com"
            };

            _userManagerMock.Setup(um => um.FindByIdAsync("1"))
            .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.GetUserIdAsync(user))
            .ReturnsAsync("1");

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Task", redirectResult.ControllerName);
        }

    }
}
