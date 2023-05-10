using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkRestApi.Controllers;
using VkRestApi.Data;
using VkRestApi.Models;

namespace ApiTest
{
    // I think mocking EF isnt the best way of testing
    // Deadline is coming, so there are few examples of my unit tests
    // The better way is to add another logic layer in API and test it instead
    // but i think its too late for me to change structure :(
    public class ApiTests
    {
        [Fact]
        public async Task Get_TakeActiveUser_ReturnsJsonResult()
        {
            // Arrange
            User user = new();
            var mockUsers = new Mock<DbSet<User>>();
            mockUsers.Setup(_ => _.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
            var mockGroups = new Mock<DbSet<UserGroup>>();
            mockGroups.Setup(_ => _.FindAsync(It.IsAny<int>())).ReturnsAsync(user.UserGroup);
            var mockStates = new Mock<DbSet<UserState>>();
            mockStates.Setup(_ => _.FindAsync(It.IsAny<int>())).ReturnsAsync(user.UserState);
            var controller = GetContollerWithMockedParameters(mockUsers, mockGroups, mockStates);

            // Act
            var result = await controller.Get(0);

            // Assert
            var response = Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task Get_TakeBlockedUser_ReturnsNoContentResult()
        {
            // Arrange
            User user = new();
            user.UserState.Code = UserStateEnum.BLOCKED;
            var mockUsers = new Mock<DbSet<User>>();
            mockUsers.Setup(_ => _.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
            var mockGroups = new Mock<DbSet<UserGroup>>();
            mockGroups.Setup(_ => _.FindAsync(It.IsAny<int>())).ReturnsAsync(user.UserGroup);
            var mockStates = new Mock<DbSet<UserState>>();
            mockStates.Setup(_ => _.FindAsync(It.IsAny<int>())).ReturnsAsync(user.UserState);
            var controller = GetContollerWithMockedParameters(mockUsers, mockGroups, mockStates);

            // Act
            var result = await controller.Get(0);

            // Assert
            var response = Assert.IsType<NoContentResult>(result);
        }


        private UsersController GetContollerWithMockedParameters(
            Mock<DbSet<User>> mockUsers, Mock<DbSet<UserGroup>> mockGroups, Mock<DbSet<UserState>> mockStates)
        {
            var mockContext = new Mock<ApiContext>();
            mockContext.Setup(_ => _.Users).Returns(mockUsers.Object);
            mockContext.Setup(_ => _.UserGroups).Returns(mockGroups.Object);
            mockContext.Setup(_ => _.UserStates).Returns(mockStates.Object);
            var mockCache = new Mock<IMemoryCache>();
            var controller = new UsersController(mockContext.Object, mockCache.Object);
            return controller;
        }
    }
}
