using GradeCalculator.Adapter;
using GradeCalculator.Controllers;
using GradeCalculator.Models;
using GradeCalculator.Service;
using GradeCalculator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GradeCalculatorTest
{
    public class KorisnikControllerTests
    {
        private readonly Mock<PiGradeCalculatorContext> _mockContext;
        private readonly Mock<StatistikaService> _mockStatistikaService;
        private readonly Mock<IKorisnikService> _mockKorisnikService;
        private readonly Mock<IKorisnikAdapter> _mockKorisnikAdapter;
        private readonly KorisnikController _controller;

        public KorisnikControllerTests()
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
            var fakeUser = new Korisnik { Idkorisnik = 1, KorisnickoIme = "Testomir", Eposta = "test@mir.com", UkupnaOcjena = 3 };
            _mockKorisnikService.Setup(s => s.GetUser(1)).Returns(fakeUser);

            var result = _controller.Details(1);

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

        [Fact]
        public void SetProfileData_UpdateEmailAndUsername()
        {
            var userVm = new ShowKorisnikVM { UserName = "Testomirovic", Email = "test@miro.vic" };
            var fakeUser = new Korisnik { Idkorisnik = 1, KorisnickoIme = "Testomir", Eposta = "test@mir.com" };
            var korisniks = new List<Korisnik> { fakeUser };
            _mockContext.Setup(c => c.Korisniks).Returns(MockDbSetFromList(korisniks));

            var result = _controller.SetProfileData(1, userVm);

            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal("Testomirovic", fakeUser.KorisnickoIme);
            Assert.Equal("test@miro.vic", fakeUser.Eposta);
        }

        [Fact]
        public void SetProfileData_UpdateEmail()
        {
            var userVm = new ShowKorisnikVM { Email = "test@miro.vic" };
            var fakeUser = new Korisnik { Idkorisnik = 1, Eposta = "test@mir.com" };
            var korisniks = new List<Korisnik> { fakeUser };
            _mockContext.Setup(c => c.Korisniks).Returns(MockDbSetFromList(korisniks));

            var result = _controller.SetProfileData(1, userVm);

            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal("test@miro.vic", fakeUser.Eposta);
        }

        [Fact]
        public void SetProfileData_UpdateUsername()
        {
            var userVm = new ShowKorisnikVM { UserName = "Testomirovic" };
            var fakeUser = new Korisnik { Idkorisnik = 1, KorisnickoIme = "Testomir" };
            var korisniks = new List<Korisnik> { fakeUser };
            _mockContext.Setup(c => c.Korisniks).Returns(MockDbSetFromList(korisniks));

            var result = _controller.SetProfileData(1, userVm);

            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal("Testomirovic", fakeUser.KorisnickoIme);
        }

        [Fact]
        public void ChangePassword_Valid()
        {
            var passwordVm = new ChangePasswordVM { NewPassword = "CarobnaRijec" };
            var fakeUser = new Korisnik { Idkorisnik = 1, KorisnickoIme = "Testomir", Eposta = "test@mir.com", LozinkaHash = "1TcHBkOuqki6BoFGvoP0uLbGJYgBPN0uLhZsf9YIFGg=", LozinkaSalt = "Y5OQpJe7XffpVmh6k3TCCg==" };
            var korisniks = new List<Korisnik> { fakeUser };
            _mockContext.Setup(c => c.Korisniks).Returns(MockDbSetFromList(korisniks));

            var result = _controller.ChangePassword(1, passwordVm);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void Create_Valid()
        {
            var userVm = new KorisnikVM { UserName = "Testomir", Email = "test@mir.com", Password = "password", IsAdmin = false };
            _mockKorisnikService.Setup(s => s.IsEmailTaken(It.IsAny<string>())).Returns(false);
            _mockKorisnikService.Setup(s => s.IsUsernameTaken(It.IsAny<string>())).Returns(false);

            var result = _controller.Create(userVm);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Create_InvalidUser_NoData()
        {
            var userVm = new KorisnikVM { UserName = "", Email = "" };
            _controller.ModelState.AddModelError("UserName", "Username is required.");
            _controller.ModelState.AddModelError("Email", "Email is required.");

            var result = _controller.Create(userVm);

            Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public void Create_InvalidUser_EmailTaken()
        {
            var userVm = new KorisnikVM { UserName = "Testomir", Email = "test@mir.com" };
            _mockKorisnikService.Setup(s => s.IsEmailTaken(userVm.Email)).Returns(true);

            var result = _controller.Create(userVm);

            var viewResult = Assert.IsType<ViewResult>(result);
            var modelState = _controller.ModelState;

            Assert.True(modelState.ContainsKey("Email"));
            var errorMessage = modelState["Email"].Errors.FirstOrDefault()?.ErrorMessage;
            Assert.Equal("Email is already taken", errorMessage);
        }

        [Fact]
        public void Create_InvalidUser_UsernameTaken()
        {
            var userVm = new KorisnikVM { UserName = "Testomir", Email = "test@mir.com" };
            _mockKorisnikService.Setup(s => s.IsUsernameTaken(userVm.UserName)).Returns(true);

            var result = _controller.Create(userVm);

            var viewResult = Assert.IsType<ViewResult>(result);
            var modelState = _controller.ModelState;

            Assert.True(modelState.ContainsKey("UserName"));
            var errorMessage = modelState["UserName"].Errors.FirstOrDefault()?.ErrorMessage;
            Assert.Equal("Username is already taken", errorMessage);
        }

        [Fact]
        public void Delete_Valid()
        {
            var userVm = new ShowKorisnikVM { UserName = "Testomir", Email = "test@mir.com" };
            _mockKorisnikService.Setup(s => s.GetUser(It.IsAny<int>())).Returns(new Korisnik { Idkorisnik = 1 });
            _mockKorisnikService.Setup(s => s.RemoveUser(It.IsAny<int>()));

            var result = _controller.Delete(1, userVm);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        private DbSet<T> MockDbSetFromList<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mockSet.Object;
        }
    }
}