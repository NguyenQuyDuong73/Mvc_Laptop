using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MvcLaptop.Controllers;
using MvcLaptop.Data;
using MvcLaptop.Models;
using System.Threading.Tasks;
using Xunit;

namespace MvcLaptop.Tests.Controllers
{
    public class Login
    {
        private MvcLaptopContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<MvcLaptopContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new MvcLaptopContext(options);

            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    UserName = "katesi1",
                    Password = "123",
                    Email = "katesi1@example.com"
                });

                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public async Task Login_ShouldRedirectToIndex_WhenCredentialsAreValid()
        {
            // Arrange
            var context = GetInMemoryContext("LoginTestDatabase_Valid");
            context.Users.Add(new User
            {
                UserName = "katesi1",
                Password = "123",
                Email = "katesi1@example.com"
            });
            context.SaveChanges();

            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(context, mockLogger.Object);

            // Mock session
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockHttpSession();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var userName = "katesi1";
            var password = "123";

            // Act
            var result = await controller.Login(userName, password);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }


        [Fact]
        public async Task Login_ShouldReturnViewWithError_WhenCredentialsAreInvalid()
        {
            // Arrange
            var context = GetInMemoryContext("LoginTestDatabase_Invalid");
            var controller = new HomeController(context, Mock.Of<ILogger<HomeController>>());

            // Act
            var result = await controller.Login("invalidUser", "wrongPassword");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

    }
}
