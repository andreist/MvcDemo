using Microsoft.VisualStudio.TestTools.UnitTesting;

using MvcDemo.BLL;
using MvcDemo.DAL;

using FakeItEasy;


namespace MvcDemo.Tests
{
    [TestClass]
    public class PersonBlTest
    {
        private const int PersonId = 1;

        private readonly Person _person;
        private readonly PersonDto _personDto;
        private readonly UnitOfWorkTest _uow;
        private readonly IPersonRepository _personRepository;

        public PersonBlTest()
        {
            DtoMappers.Init();

            _personDto = new PersonDto();
            _personRepository = A.Fake<IPersonRepository>();
            _uow = new UnitOfWorkTest {PersonRepository = _personRepository};

            _person = new Person
            {
                Id = PersonId,
                Age = 20,
                FirstName = "First",
                LastName = "Last",
                Sex = true
            };
        }

        [TestMethod]
        public void PersonBl_GetById_Should_Return_NewObject()
        {
            // Arrange
            A.CallTo(() => _personRepository.GetById(PersonId)).Returns(null);
            var personBl = new PersonBl(_uow);
            
            // Act
            var sut = personBl.GetById(PersonId);

            // Assert
            bool isEqual = AreObjectsEqualHelper.IsEqual(sut, _personDto);
            Assert.AreEqual(isEqual, true);
        }

        [TestMethod]
        public void PersonBl_GetById_Should_Return_Person()
        {
            // Arrange
            A.CallTo(() => _personRepository.GetById(PersonId)).Returns(_person);
            var personBl = new PersonBl(_uow);

            // Act
            var sut = personBl.GetById(PersonId);
            var actualPersonDto = personBl.FromEntityToModel(_person);
            
            // Assert
            bool isEqual = AreObjectsEqualHelper.IsEqual(sut, actualPersonDto);
            Assert.AreEqual(isEqual, true);
        }
    }
}
