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
//using System.Data.Entity;
using System.Collections.ObjectModel;

namespace BusinessLogic.Tests.Services
{
    public class HobbyServiceTests
    {
        private readonly HobbyService _sut;
        private readonly Mock<HobbyNetContext> _contextMock = new Mock<HobbyNetContext>();
      


        public HobbyServiceTests()
        {
            _sut = new HobbyService(_contextMock.Object);
        }


        [Fact]
        public void GetAllHobbies_ShouldReturnList_WhenHobbiesExists()
        {  
            var data = new List<Hobby>
            {
                 new Hobby() { Name = "Football"},
                 new Hobby() { Name = "Tennis"},
                 new Hobby() { Name = "Aerobics"}

            }.AsQueryable();

            var mockSet = new Mock<DbSet<Hobby>>();
            mockSet.As<IQueryable<Hobby>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Hobby>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Hobby>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Hobby>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<HobbyNetContext>();
            mockContext.Setup(c => c.Hobbies).Returns(mockSet.Object);

            var service = new HobbyService(mockContext.Object);
            var hobbies = service.GetAllHobbies();

            Assert.Equal(3, hobbies.Count);
            Assert.Equal("Aerobics", hobbies[0].Name);
            Assert.Equal("Football", hobbies[1].Name);
            Assert.Equal("Tennis", hobbies[2].Name);
        }



        //Assert.AreEqual("AAA", blogs[0].Name);
        //Assert.AreEqual("BBB", blogs[1].Name);
        //Assert.AreEqual("ZZZ", blogs[2].Name);
        //[Fact]
        //public void TestCreateNewDocument()
        //{
        //    var mockContext = new Mock<HobbyNetContext>();

        //    var mockDocumentDbSet = GetQueryableMockDocumentDbSet();

        //    mockContext.Setup(m => m.Hobbies).Returns(mockDocumentDbSet.Object);

        //    var documentManager = new HobbyService(mockContext.Object);

        //    var hobbies = documentManager.GetAllHobbies();

        //    // This line doesn't get hit as the .First falls over before here
        //    Assert.Equal(hobbies, GetAllHobbies());
        //}

        //private static Mock<DbSet<Hobby>> GetQueryableMockDocumentDbSet()
        //{
        //    var data = new List<Hobby>(GetAllHobbies());

        //    var mockDocumentDbSet = new Mock<DbSet<Hobby>>();
        //    mockDocumentDbSet.As<IQueryable<Hobby>>().Setup(m => m.Provider).Returns(data.AsQueryable().Provider);
        //    mockDocumentDbSet.As<IQueryable<Hobby>>().Setup(m => m.Expression).Returns(data.AsQueryable().Expression);
        //    mockDocumentDbSet.As<IQueryable<Hobby>>().Setup(m => m.ElementType).Returns(data.AsQueryable().ElementType);
        //    mockDocumentDbSet.As<IQueryable<Hobby>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        //    mockDocumentDbSet.Setup(m => m.Add(It.IsAny<Hobby>())).Callback<Hobby>(data.Add);
        //    return mockDocumentDbSet;
        //}



        //private static List<Hobby> GetAllHobbies()
        //{
        //    var hobbies = new List<Hobby>
        //        {
        //            new Hobby() { Name = "Football"},
        //            new Hobby() { Name = "Tennis"}
        //        };
        //    return hobbies;
        //}

        //[Fact]
        //public void GetAllHobbies_ShouldReturnList_WhenHobbiesExists()
        //{

        //    var hobbies = _sut.GetAllHobbies();


        //    _contextMock.Setup(x => x.Hobbies).Returns(GetAllHobbies());
        //    var _service = new HobbyService(_contextMock.Object);

        //    var result = _service.GetAllHobbies();

        //    Assert.Equal(GetAllHobbies().Count, result.Count);
        //}

    }


    //public class FakeDbSet<T> : IDbSet<T> where T : class
    //{
    //    ObservableCollection<T> _data;
    //    IQueryable _query;

    //    public FakeDbSet()
    //    {
    //        _data = new ObservableCollection<T>();
    //        _query = _data.AsQueryable();
    //    }

    //    public virtual T Find(params object[] keyValues)
    //    {
    //        throw new NotImplementedException("Derive from FakeDbSet<T> and override Find");
    //    }

    //    public T Add(T item)
    //    {
    //        _data.Add(item);
    //        return item;
    //    }

    //    public T Remove(T item)
    //    {
    //        _data.Remove(item);
    //        return item;
    //    }

    //    public T Attach(T item)
    //    {
    //        _data.Add(item);
    //        return item;
    //    }

    //    public T Detach(T item)
    //    {
    //        _data.Remove(item);
    //        return item;
    //    }

    //    public T Create()
    //    {
    //        return Activator.CreateInstance<T>();
    //    }

    //    public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
    //    {
    //        return Activator.CreateInstance<TDerivedEntity>();
    //    }

    //    public ObservableCollection<T> Local
    //    {
    //        get { return _data; }
    //    }

    //    Type IQueryable.ElementType
    //    {
    //        get { return _query.ElementType; }
    //    }

    //    System.Linq.Expressions.Expression IQueryable.Expression
    //    {
    //        get { return _query.Expression; }
    //    }

    //    IQueryProvider IQueryable.Provider
    //    {
    //        get { return _query.Provider; }
    //    }

    //    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    //    {
    //        return _data.GetEnumerator();
    //    }

    //    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    //    {
    //        return _data.GetEnumerator();
    //    }
    //}
}
