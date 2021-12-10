using System;
using Xunit;
using Moq;
using DataAccess.Context;
using BusinessLogic.Services;
using System.Collections.Generic;
using Domain.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace BusinessLogic.Tests.Services
{
    public class HobbyServiceTests
    {
        private readonly Mock<HobbyNetContext> _contextMock;
        private readonly HobbyService _service;

        public HobbyServiceTests()
        {
            _contextMock = new Mock<HobbyNetContext>();
            _service = new HobbyService(_contextMock.Object);
        }

        [Fact]
        public void GetAllHobbies_List_HobbiesExists_OrderByHobbyName()
        {
            // Arrange
            var allHobbies = GetAllHobbies();
            var mockSetAllHobbies = GetQueryableMockDbSet<Hobby>(allHobbies);
            _contextMock.Setup(db => db.Hobbies).Returns(mockSetAllHobbies.Object);

            // Act
            var hobbiesActual = _service.GetAllHobbies();

            // Assert
            Assert.Equal(5, hobbiesActual.Count);
            Assert.Equal("Aerobics", hobbiesActual[0].Name);
            Assert.Equal("Diving", hobbiesActual[1].Name);
            Assert.Equal("Drawing", hobbiesActual[2].Name);
            Assert.Equal("Football", hobbiesActual[3].Name);
            Assert.Equal("Tennis", hobbiesActual[4].Name);
        }

        [Fact]
        public void GetUserHobbiesList_List_UserAndHobbiesExists_OrderByHobbyName()
        {
            // Arrange
            var allHobbies = GetAllHobbies();
            var mockSetAllHobbies = GetQueryableMockDbSet<Hobby>(allHobbies);

            var allUsers = GetAllUsers(allHobbies);
            var mockSetAllUsers = GetQueryableMockDbSet<User>(allUsers);

            _contextMock.Setup(db => db.Hobbies).Returns(mockSetAllHobbies.Object);
            _contextMock.Setup(db => db.Users).Returns(mockSetAllUsers.Object);

            // Act
            var userHobbiesActual = _service.GetUserHobbiesList("id1");

            // Assert
            Assert.Equal(3, userHobbiesActual.Count);
            Assert.Equal("Aerobics", userHobbiesActual[0].Name);
            Assert.Equal("Football", userHobbiesActual[1].Name);
            Assert.Equal("Tennis", userHobbiesActual[2].Name);
        }

        [Fact]
        public void RemoveHobbyFromList_True_UserAndHobbyExists()
        {
            // Arrange
            var allHobbies = GetAllHobbies();
            var mockSetAllHobbies = GetQueryableMockDbSet<Hobby>(allHobbies);

            var allUsers = GetAllUsers(allHobbies);
            var mockSetAllUsers = GetQueryableMockDbSet<User>(allUsers);

            _contextMock.Setup(db => db.Hobbies).Returns(mockSetAllHobbies.Object);
            _contextMock.Setup(db => db.Users).Returns(mockSetAllUsers.Object);

            // Act
            var resultActual = _service.RemoveHobbyFromList("id3", 4);

            // Assert
            Assert.True(resultActual);
        }

        [Fact]
        public void RemoveHobbyFromList_False_HobbyNotExist()
        {
            // Arrange
            var allHobbies = GetAllHobbies();
            var mockSetAllHobbies = GetQueryableMockDbSet<Hobby>(allHobbies);

            var allUsers = GetAllUsers(allHobbies);
            var mockSetAllUsers = GetQueryableMockDbSet<User>(allUsers);

            _contextMock.Setup(db => db.Hobbies).Returns(mockSetAllHobbies.Object);
            _contextMock.Setup(db => db.Users).Returns(mockSetAllUsers.Object);

            // Act
            var resultActual = _service.RemoveHobbyFromList("id3", 6);

            // Assert
            Assert.False(resultActual);
        }

        [Fact]
        public void RemoveHobbyFromList_False_UserNotExist()
        {
            // Arrange
            var allHobbies = GetAllHobbies();
            var mockSetAllHobbies = GetQueryableMockDbSet<Hobby>(allHobbies);

            var allUsers = GetAllUsers(allHobbies);
            var mockSetAllUsers = GetQueryableMockDbSet<User>(allUsers);

            _contextMock.Setup(db => db.Hobbies).Returns(mockSetAllHobbies.Object);
            _contextMock.Setup(db => db.Users).Returns(mockSetAllUsers.Object);

            // Act
            var resultActual = _service.RemoveHobbyFromList("id4", 2);

            // Assert
            Assert.False(resultActual);     
        }

        [Fact]
        public void AddHobiesToUser_UserAndHobbiesExists()
        {
            // Arrange
            var allHobbies = GetAllHobbies();
            var mockSetAllHobbies = GetQueryableMockDbSet<Hobby>(allHobbies);

            var allUsers = GetAllUsers(allHobbies);
            var mockSetAllUsers = GetQueryableMockDbSet<User>(allUsers);

            var hobbiesToAdd = new List<Hobby>(allHobbies.Take(2));

            _contextMock.Setup(db => db.Hobbies).Returns(mockSetAllHobbies.Object);
            _contextMock.Setup(db => db.Users).Returns(mockSetAllUsers.Object);

            // Act
            _service.AddHobbiesToUser("id2", hobbiesToAdd);

            // Assert
            Assert.Equal(2, _service.GetUserHobbiesList("id2").Count);
        }

        [Fact]
        public void AddHobiesToUser_HobbiesNotExists()
        {
            // Arrange
            var allHobbies = GetAllHobbies();
            var mockSetAllHobbies = GetQueryableMockDbSet<Hobby>(allHobbies);

            var allUsers = GetAllUsers(allHobbies);
            var mockSetAllUsers = GetQueryableMockDbSet<User>(allUsers);

            var hobbiesToAdd = new List<Hobby>();

            _contextMock.Setup(db => db.Hobbies).Returns(mockSetAllHobbies.Object);
            _contextMock.Setup(db => db.Users).Returns(mockSetAllUsers.Object);

            // Act
            _service.AddHobbiesToUser("id2", hobbiesToAdd);

            // Assert
            Assert.Empty(_service.GetUserHobbiesList("id2"));
        }

        private static Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> soucreList) where T : class
        {
            var queryable = soucreList.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mockSet;
        }

        private static List<Hobby> GetAllHobbies()
        {
            var hobbies = new List<Hobby>
            {
                 new Hobby() 
                 {
                     Id = 1, 
                     Name = "Football"
                 },
                 new Hobby() 
                 {
                     Id = 2,
                     Name = "Tennis"
                 },
                 new Hobby() 
                 {
                     Id = 3,
                     Name = "Aerobics"
                 },
                 new Hobby() 
                 {
                     Id = 4,
                     Name = "Diving"
                 },
                 new Hobby() 
                 {
                     Id = 5,
                     Name = "Drawing"
                 },
            };
            return hobbies;
        }

        private static List<User> GetAllUsers(List<Hobby> allHobbies)
        {
            var users = new List<User>
            {
                 new User()
                 {
                     Id = "id1",
                     UserName = "User1",
                     Hobbies = new List<Hobby>(allHobbies.Take(3))
                 },
                 new User()
                 {
                     Id = "id2",
                     UserName = "User2",
                     Hobbies = new List<Hobby>()
                 },
                 new User()
                 {
                     Id = "id3",
                     UserName = "User3",
                     Hobbies = new List<Hobby>(allHobbies.Take(4))
                 }
            };
            return users;
        }
    }
}