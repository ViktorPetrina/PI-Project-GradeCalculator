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
            _logController = new LogController(_mockContext.Object);
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

        
    }
}
