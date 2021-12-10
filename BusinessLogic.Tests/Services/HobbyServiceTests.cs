using System;
using Xunit;
using Moq;
using DataAccess.Context;
using BusinessLogic.Services;

namespace BusinessLogic.Tests.Services
{
    public class HobbyServiceTests
    {
        private readonly Mock<HobbyNetContext> _contextMock;
        private HobbyService _service;


        public HobbyServiceTests()
        {
            _contextMock = new Mock<HobbyNetContext>();
            _service = new HobbyService(_contextMock.Object);
        }

        [Fact]
        public void Test1()
        {

        }
    }
}
