using AutoMapper;
using GradeCalculator.Adapter;
using GradeCalculator.Controllers;
using GradeCalculator.Models;
using GradeCalculator.Repository;
using GradeCalculator.Security;
using GradeCalculator.Service;
using GradeCalculator.Utilities;
using GradeCalculator.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeCalculatorTest
{
    public class LogTest
    {
        private readonly Mock<PiGradeCalculatorContext> _mockContext;
        private readonly Mock<ILogService> _mockLogService;
        private readonly Mock<LogAdapter> _mockAdapter;
        private readonly Mock<IRepository<Godina>> _mockGodinaRepo;
        private readonly Mock<IRepository<Predmet>> _mockSubjectRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly LogController _logController;
        private readonly HomeController _homeController;
        private readonly UserLoginController _userLoginController;
        private readonly GodinaController _godinaController;

        public LogTest()
        {
            _mockContext = new Mock<PiGradeCalculatorContext>();
            _mockLogService = new Mock<ILogService>();
            _mockAdapter = new Mock<LogAdapter>();
            _mockGodinaRepo = new Mock<IRepository<Godina>>();
            _mockSubjectRepo = new Mock<IRepository<Predmet>>();
            _mockMapper = new Mock<IMapper>();
            _logController = new LogController(_mockContext.Object, _mockAdapter.Object);
            _homeController = new HomeController(_mockLogService.Object);
            _userLoginController = new UserLoginController(_mockContext.Object, _mockLogService.Object);

        }

        [Fact]
        public void Home_AddsLog()
        {
            var result = _homeController.Index();

            _mockLogService.Verify(
               logService => logService.AddLog("Korisnik ušao u stranicu"),
               Times.Once);

            Assert.IsType<ViewResult>(result);
        }
        
        [Fact]
        public void Logout_Log()
        {
            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var serviceProvider = new Mock<IServiceProvider>();
            var mockLogService = new Mock<ILogService>();
            var tempData = new Mock<ITempDataDictionary>();


            serviceProvider
                .Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthService.Object);

            mockHttpContext
                .Setup(http => http.RequestServices)
                .Returns(serviceProvider.Object);

            var controller = new UserLoginController(null, mockLogService.Object)
            {
                ControllerContext = new ControllerContext
                {HttpContext = mockHttpContext.Object},
                TempData = tempData.Object 
            };


            var result = controller.Logout();

            mockAuthService.Verify(auth => auth.SignOutAsync(
                mockHttpContext.Object,
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<AuthenticationProperties>()),
                Times.Once);


            mockLogService.Verify(log => log.AddLog("Korisnik  se odjavio iz sustava"), Times.Once);

            Assert.IsType<ViewResult>(result); 
        }
        [Fact]
        public void Log_Shown()
        {
            var logs = new List<Log>
            {
                new Log { Idlog = 1, Opis = "Log 1", Vrijeme = DateTime.Now },
                new Log { Idlog = 2, Opis = "log 2", Vrijeme = DateTime.Now }
            }.AsQueryable();

            var mockLogsDbSet = new Mock<DbSet<Log>>();
            mockLogsDbSet.As<IQueryable<Log>>().Setup(l => l.Provider).Returns(logs.Provider);
            mockLogsDbSet.As<IQueryable<Log>>().Setup(l => l.Expression).Returns(logs.Expression);
            mockLogsDbSet.As<IQueryable<Log>>().Setup(l => l.ElementType).Returns(logs.ElementType);
            mockLogsDbSet.As<IQueryable<Log>>().Setup(l => l.GetEnumerator()).Returns(logs.GetEnumerator());

            var mockContext = new Mock<PiGradeCalculatorContext>();
            mockContext.Setup(c => c.Logs).Returns(mockLogsDbSet.Object);

            var mockAdapter = new Mock<ILogAdapter>();
            mockAdapter.Setup(a => a.Adapt(It.IsAny<Log>()))
                .Returns<Log>(log => new ShowLogVM
            {
                Opis = log.Opis,
                Vrijeme = log.Vrijeme
            });
            var controller = new LogController(mockContext.Object, mockAdapter.Object);


            var result = controller.Details() as ViewResult;


            Assert.NotNull(result);
            var model = Assert.IsType<List<ShowLogVM>>(result.Model);
            Assert.Equal(2, model.Count); 
            Assert.Equal("Log 1", model[0].Opis); 



        }

        [Fact]
        public void Login_Admin_Log()
        {
            // Arrange
            var username = "admin";
            var password = "password";

            var user = new Korisnik
            {
                KorisnickoIme = username,
                LozinkaHash = "hashedPassword",
                LozinkaSalt = "salt",
                UlogaId = 2,
                Uloga = new Uloga { Naziv = "Admin" }
            };

            var model = new AutentikacijaVM
            {
                Username = username,
                Password = password
            };

            var users = new List<Korisnik> { user }.AsQueryable();

            var mockSet = new Mock<DbSet<Korisnik>>();
            mockSet.As<IQueryable<Korisnik>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<Korisnik>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<Korisnik>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<Korisnik>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _mockContext.Setup(c => c.Korisniks).Returns(mockSet.Object);
            var mockPasswordProvider = new Mock<PasswordProvider>();
            mockPasswordProvider.Setup(p => p.GetHash(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(user.LozinkaHash);


            // Act
            var result = _userLoginController.Login(model);

            // Assert
            _mockLogService.Verify(log => log.AddLog("Osoba sa ulogom1 se prijavio"), Times.Once);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Save_new_Changes()
        {
            var godinaVm = new GodinaVM { Naziv = "2025" };
            var godina = new Godina { Naziv = "2025", KorisnikId = 1 };

            _mockGodinaRepo.Setup(repo => repo.GetAll()).Returns(new List<Godina>());
            _mockMapper.Setup(mapper => mapper.Map<Godina>(godinaVm)).Returns(godina);



            // Act
            var result = _godinaController.Create(godinaVm);

            // Assert

            _mockLogService.Verify(log => log.AddLog("Korisnik spremio godinu u bazu."), Times.Once);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(GodinaController.Index), redirectResult.ActionName);
        }
        [Fact]
        public void Export_Data()
        {
            // Arrange
            int userId = 1;
            var years = new List<Godina>
    {
        new Godina
        {
            Idgodina = 1,
            Naziv = "Test Year",
            KorisnikId = userId
        }
    };

            _mockGodinaRepo.Setup(repo => repo.GetAll()).Returns(years);

            // Act
            var result = _godinaController.ExportData(userId) as FileContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("application/json", result.ContentType);
            Assert.Equal("ocjene.json", result.FileDownloadName);
            Assert.NotEmpty(result.FileContents); // Ensure content is not empty
            _mockLogService.Verify(
                log => log.AddLog($"Korisnik sa id {userId} je preuzeo podatke"),
                Times.Once);
        }
        [Fact]
        public void Signin_AddsLog()
        {
            // Arrange
            var username = "adminUser";
            var password = "correctPassword";

            var user = new Korisnik
            {
                KorisnickoIme = username,
                LozinkaHash = "hashedPassword",
                LozinkaSalt = "salt",
                UlogaId = 1,
                Uloga = new Uloga { Naziv = "Admin" }
            };

            var model = new AutentikacijaVM
            {
                Username = username,
                Password = password
            };

            var users = new List<Korisnik> { user }.AsQueryable();

            var mockSet = new Mock<DbSet<Korisnik>>();
            mockSet.As<IQueryable<Korisnik>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<Korisnik>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<Korisnik>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<Korisnik>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _mockContext.Setup(c => c.Korisniks).Returns(mockSet.Object);
            var mockPasswordProvider = new Mock<PasswordProvider>();
            mockPasswordProvider.Setup(p => p.GetHash(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(user.LozinkaHash);

            // Act
            var result = _userLoginController.Login(model);

            // Assert
            _mockLogService.Verify(log => log.AddLog("Osoba sa ulogom1 se prijavio"), Times.Once);
            Assert.IsType<RedirectToActionResult>(result);

        }
    }
}
