using GradeCalculator.Adapter;
using GradeCalculator.Controllers;
using GradeCalculator.Models;
using GradeCalculator.Service;
using GradeCalculator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GradeCalculatorTest
{
    public class KorisnikTests
    {
        private readonly Mock<PiGradeCalculatorContext> _mockContext;
        private readonly Mock<StatistikaService> _mockStatistikaService;
        private readonly Mock<IKorisnikService> _mockKorisnikService;
        private readonly Mock<IKorisnikAdapter> _mockKorisnikAdapter;
        private readonly KorisnikController _controller;

        public KorisnikTests()
        {
            _mockContext = new Mock<PiGradeCalculatorContext>();
            _mockStatistikaService = new Mock<StatistikaService>();
            _mockKorisnikService = new Mock<IKorisnikService>();
            _mockKorisnikAdapter = new Mock<IKorisnikAdapter>();

            _controller = new KorisnikController(
                _mockContext.Object,
                _mockStatistikaService.Object,
                _mockKorisnikService.Object,
                _mockKorisnikAdapter.Object);
        }

        [Fact]
        public void Index_ReturnsUserList()
        {
            //Arrange
            var fakeUsers = new List<ShowKorisnikVM>
            {
                new ShowKorisnikVM { Id = 1, UserName = "Testomir", Email = "test@mir.com", TotalGrade = 3, RoleName = "regular"},
                new ShowKorisnikVM { Id = 2, UserName = "Testoslav", Email = "test@mir.com", TotalGrade = 3, RoleName = "regular" }
            };
            _mockKorisnikService.Setup(s => s.GetAllUsers()).Returns(fakeUsers);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ShowKorisnikVM>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Details_UserExists()
        {
            // Arrange
            var fakeUser = new Korisnik { Idkorisnik = 1, KorisnickoIme = "Testomir", Eposta = "test@mir.com", UkupnaOcjena = 3 };
            _mockKorisnikService.Setup(s => s.GetUser(1)).Returns(fakeUser);

            // Act
            var result = _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ShowKorisnikVM>(viewResult.Model);
            Assert.Equal("Testomir", model.UserName);
        }

        [Fact]
        public void Details_UserDoesNotExist()
        {
            var result = _controller.Details(1);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Could not find user", notFoundResult.Value.ToString());
        }
    }
}