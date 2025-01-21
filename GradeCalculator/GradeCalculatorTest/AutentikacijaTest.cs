using GradeCalculator.Controllers;
using GradeCalculator.Models;
using GradeCalculator.Security;
using GradeCalculator.Service;
using GradeCalculator.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis.Elfie.Model.Structures;
using System.Security.Claims;

namespace GradeCalculatorTest
{
    public class AutentikacijaTest
    {
        [Fact]
        public void Login_Get_VM()
        {

            var controller = new UserLoginController(null, null);
            var returnUrl = "/returnUrl";

            var result = controller.Login(returnUrl) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<AutentikacijaVM>(result.Model);
            Assert.Equal(returnUrl, model.ReturnUrl);
        }
        [Fact]
        public void Login_Post_Valid_Redirect()
        {

            var mockContext = new Mock<PiGradeCalculatorContext>();
            var mockLogService = new Mock<ILogService>();


            var mockUloga = new Uloga { Iduloga = 1, Naziv = "korisnik" };


            var korisnik = new Korisnik
            {
                Idkorisnik = 1,
                KorisnickoIme = "validanKorisnik",
                LozinkaSalt = "testSalt",
                LozinkaHash = PasswordProvider.Instance.GetHash("validanPassword", "testSalt"),
                Uloga = mockUloga,
                UlogaId = mockUloga.Iduloga
            };

            var korisnici = new List<Korisnik> { korisnik }.AsQueryable();
            var mockKorisniciDbSet = new Mock<DbSet<Korisnik>>();
            mockKorisniciDbSet.As<IQueryable<Korisnik>>().Setup(m => m.Provider).Returns(korisnici.Provider);
            mockKorisniciDbSet.As<IQueryable<Korisnik>>().Setup(m => m.Expression).Returns(korisnici.Expression);
            mockKorisniciDbSet.As<IQueryable<Korisnik>>().Setup(m => m.ElementType).Returns(korisnici.ElementType);
            mockKorisniciDbSet.As<IQueryable<Korisnik>>().Setup(m => m.GetEnumerator()).Returns(korisnici.GetEnumerator());

            mockContext.Setup(c => c.Korisniks).Returns(mockKorisniciDbSet.Object);

            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthService.Object);

            mockHttpContext
                .Setup(hc => hc.RequestServices)
                .Returns(serviceProvider.Object);

            var controller = new UserLoginController(mockContext.Object, mockLogService.Object)
            {
                ControllerContext = new ControllerContext
                {HttpContext = mockHttpContext.Object}
            };

            var validCredentials = new AutentikacijaVM
            {
                Username = "validanKorisnik",
                Password = "validanPassword",
                ReturnUrl = "/dashboard"
            };


            var result = controller.Login(validCredentials) as LocalRedirectResult;


            Assert.NotNull(result); 
            Assert.Equal(validCredentials.ReturnUrl, result.Url); 


            mockAuthService.Verify(auth => auth.SignInAsync(
                mockHttpContext.Object,
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.Is<ClaimsPrincipal>(principal =>
                    principal.Identity.Name == validCredentials.Username &&
                    principal.IsInRole(mockUloga.Naziv)),
                It.IsAny<AuthenticationProperties>()),
                Times.Once);
        }
        [Fact]
        public void Login_Post_Invalid()
        {

            var mockContext = new Mock<PiGradeCalculatorContext>();
            var mockLogService = new Mock<ILogService>();

   
            var korisniks = new List<Korisnik>().AsQueryable(); 
            var mockKorisniksDbSet = new Mock<DbSet<Korisnik>>();
            mockKorisniksDbSet.As<IQueryable<Korisnik>>().Setup(k => k.Provider).Returns(korisniks.Provider);
            mockKorisniksDbSet.As<IQueryable<Korisnik>>().Setup(k => k.Expression).Returns(korisniks.Expression);
            mockKorisniksDbSet.As<IQueryable<Korisnik>>().Setup(k => k.ElementType).Returns(korisniks.ElementType);
            mockKorisniksDbSet.As<IQueryable<Korisnik>>().Setup(k => k.GetEnumerator()).Returns(korisniks.GetEnumerator());

            mockContext.Setup(g => g.Korisniks).Returns(mockKorisniksDbSet.Object);

            var controller = new UserLoginController(mockContext.Object, mockLogService.Object);

            var invalidCredentials = new AutentikacijaVM
            {
                Username = "invalidUse",
                Password = "inavliPassword"
            };


            var result = controller.Login(invalidCredentials) as ViewResult;

            Assert.NotNull(result); 
            Assert.False(controller.ModelState.IsValid); 
            Assert.Contains("", controller.ModelState.Keys); 

        }
        [Fact]
        public void Register_Valid()
        {

            var mockKorisniciDbSet = new Mock<DbSet<Korisnik>>();
            var korisniciList = new List<Korisnik>();

            mockKorisniciDbSet.As<IQueryable<Korisnik>>()
                .Setup(k => k.Provider).Returns(korisniciList.AsQueryable().Provider);
            mockKorisniciDbSet.As<IQueryable<Korisnik>>()
                .Setup(k => k.Expression).Returns(korisniciList.AsQueryable().Expression);
            mockKorisniciDbSet.As<IQueryable<Korisnik>>()
                .Setup(k => k.ElementType).Returns(korisniciList.AsQueryable().ElementType);
            mockKorisniciDbSet.As<IQueryable<Korisnik>>()
                .Setup(k => k.GetEnumerator()).Returns(korisniciList.GetEnumerator());

            mockKorisniciDbSet.Setup(k => k.Add(It.IsAny<Korisnik>()))
                .Callback<Korisnik>(k => korisniciList.Add(k));

            var mockContext = new Mock<PiGradeCalculatorContext>();
            mockContext.Setup(g => g.Korisniks).Returns(mockKorisniciDbSet.Object);
            mockContext.Setup(g => g.SaveChanges()).Returns(1);

            var mockLogService = new Mock<ILogService>();
            var controller = new UserLoginController(mockContext.Object, mockLogService.Object);

            var newUser = new KorisnikVM
            {
                Id = 1,
                UserName = "noviU",
                Email = "noviU@test.com",
                Password = "testing123"
            };


            var result = controller.Register(newUser);


            Assert.NotNull(result); 
            var redirectResult = Assert.IsType<RedirectToActionResult>(result); 
            Assert.Equal("Index", redirectResult.ActionName); 
            Assert.Equal("Home", redirectResult.ControllerName); 




        }
        [Fact]
        public void Logout_CallsSignOutAndRedirects()
        {

            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var serviceProvider = new Mock<IServiceProvider>();
            var mockLogService = new Mock<ILogService>();

            serviceProvider
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthService.Object);

            mockHttpContext
                .Setup(hc => hc.RequestServices)
                .Returns(serviceProvider.Object);

            var tempData = new Mock<ITempDataDictionary>();

            var controller = new UserLoginController(null, mockLogService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
                TempData = tempData.Object 
            };


            var result = controller.Logout();


            mockAuthService.Verify(auth => auth.SignOutAsync(
                mockHttpContext.Object,
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<AuthenticationProperties>()),
                Times.Once);

            Assert.IsType<ViewResult>(result); 
        }

    }
}
