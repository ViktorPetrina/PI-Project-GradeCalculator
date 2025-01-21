using AutoMapper;
using GradeCalculator.Controllers;
using GradeCalculator.Models;
using GradeCalculator.Repository;
using GradeCalculator.Service;
using GradeCalculator.Utilities;
using GradeCalculator.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace GradeCalculatorTest
{
    public class GodinaControllerTests
    {
        private readonly Mock<IRepository<Godina>> mockYearRepo;
        private readonly Mock<IRepository<Predmet>> mockSubjectRepo;
        private readonly Mock<IMapper> mockMapper;
        private readonly Mock<JsonExportHelper> mockJsonExportHelper;
        private readonly GodinaController controller;
        private readonly PiGradeCalculatorContext context;

        public GodinaControllerTests()
        {
            mockYearRepo = new Mock<IRepository<Godina>>();
            mockSubjectRepo = new Mock<IRepository<Predmet>>();
            mockMapper = new Mock<IMapper>();
            mockJsonExportHelper = new Mock<JsonExportHelper>();
            context = new PiGradeCalculatorContext();

            controller = new GodinaController(
                context,
                mockSubjectRepo.Object,
                mockYearRepo.Object,
                mockMapper.Object,
                new LogService());
        }

        [Fact]
        public void Index_ReturnsViewResultAndValidIdAndUserList()
        {
            var years = new List<Godina>
            {
                new Godina { Idgodina = 1, KorisnikId = 1 },
                new Godina { Idgodina = 2, KorisnikId = 1 },
            };

            var yearVms = new List<GodinaVM>
            {
                new GodinaVM { Idgodina = 1, KorisnikId = 1 },
                new GodinaVM { Idgodina = 2, KorisnikId = 1 },
            };

            mockYearRepo.Setup(repo => repo.GetAll()).Returns(years);
            mockMapper.Setup(m => m.Map<IEnumerable<GodinaVM>>(years)).Returns(yearVms);

            var result = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<GodinaVM>>(viewResult.ViewData.Model);

            Assert.Equal(2, model.Count());
            Assert.Equal(1, controller.ViewBag.Id);
        }

        [Fact]
        public void GetDownload_ReturnsValidFileContentResult()
        {
            var data = new List<Godina>
            {
                new Godina { Idgodina = 1, KorisnikId = 1, Naziv = "Prvi semestar", Prosjek = 4.5 },
                new Godina { Idgodina = 2, KorisnikId = 1, Naziv = "Drugi semestar", Prosjek = 3.8 }
            };

            var serializedData = JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(serializedData);
            var expectedResult = new FileContentResult(bytes, "application/json")
            {
                FileDownloadName = "ocjene.json"
            };

            mockJsonExportHelper.Setup(h => h.Serialize(data)).Returns(serializedData);
            mockJsonExportHelper.Setup(h => h.ToBytes(serializedData)).Returns(bytes);
            mockJsonExportHelper.Setup(h => h.GetResult(bytes)).Returns(expectedResult);

            var result = mockJsonExportHelper.Object.GetDownload(data);

            var fileContentResult = Assert.IsType<FileContentResult>(result);
            var content = Encoding.UTF8.GetString(fileContentResult.FileContents);

            Assert.Contains("Prvi semestar", content);
            Assert.Contains("Drugi semestar", content);
        }

        [Fact]
        public void Create_Valid()
        {
            var result = controller.Create();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<GodinaVM>(viewResult.ViewData.Model);

            Assert.NotNull(model);
        }

        private const string YEAR_EXISTS_ERROR = "Vec postoji godina sa istim nazivom";

        [Fact]
        public void Create_InvalidYear_NameExists()
        {
            var godinaVm = new GodinaVM { Naziv = "Prvi semestar" };
            mockYearRepo.Setup(repo => repo.GetAll()).Returns(new List<Godina>
            {
                new Godina { Naziv = "Prvi semestar" }
            }.AsQueryable());

            var result = controller.Create(godinaVm);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(YEAR_EXISTS_ERROR, controller.ModelState[""]?.Errors.First().ErrorMessage);
        }

        [Fact]
        public void Create_Valid_POST()
        {
            var godinaVm = new GodinaVM { Naziv = "Drugi semestar" };

            mockYearRepo.Setup(repo => repo.GetAll()).Returns(new List<Godina>().AsQueryable());
            mockMapper.Setup(m => m.Map<Godina>(godinaVm)).Returns(new Godina { Naziv = godinaVm.Naziv });

            var result = controller.Create(godinaVm);

            mockYearRepo.Verify(repo => repo.Add(It.IsAny<Godina>()), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(GodinaController.Index), redirectResult.ActionName);
        }

        [Fact]
        public void Subjects_RedirectsToSubjectsByYear()
        {
            int yearId = 1;

            var result = controller.Subjects(yearId);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SubjectsByYear", redirectResult.ActionName);
            Assert.Equal("Predmet", redirectResult.ControllerName);
            Assert.Equal(yearId, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public void Delete_Valid()
        {
            int yearId = 1;
            var year = new Godina { Idgodina = yearId, Naziv = "Prvi semestar", Prosjek = 4.5, KorisnikId = 1 };
            var yearVm = new GodinaVM { Idgodina = yearId, Naziv = "Drugi semestar", Prosjek = 4.5, KorisnikId = 1 };

            mockYearRepo.Setup(repo => repo.Get(yearId)).Returns(year);
            mockMapper.Setup(m => m.Map<GodinaVM>(year)).Returns(yearVm);

            var result = controller.Delete(yearId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<GodinaVM>(viewResult.ViewData.Model);
            Assert.Equal(yearVm, model);
        }

        [Fact]
        public void Delete_Valid_POST()
        {
            int yearId = 1;
            var godinaVm = new GodinaVM { Idgodina = yearId, Naziv = "Prvi semestar", Prosjek = 4.5, KorisnikId = 1 };
            var subjects = new List<Predmet>
            {
                new Predmet { Idpredmet = 1, GodinaId = yearId, Naziv = "Programiranje" },
                new Predmet { Idpredmet = 2, GodinaId = yearId, Naziv = "Matematika" }
            };

            mockSubjectRepo.Setup(repo => repo.GetAll()).Returns(subjects.AsQueryable());

            var result = controller.Delete(yearId, godinaVm);

            mockSubjectRepo.Verify(repo => repo.Remove(It.Is<int>(id => id == 1 || id == 2)), Times.Exactly(2));
            mockYearRepo.Verify(repo => repo.Remove(yearId), Times.Once);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(GodinaController.Index), redirectResult.ActionName);
        }

        [Fact]
        public void ImportData_Valid_GET()
        {
            var result = controller.ImportData();

            var viewResult = Assert.IsType<ViewResult>(result);
        }

        private const string FILE_ERROR = "Unesite valjanu json datoteku.";

        [Fact]
        public void ImportData_POST_FileIsNull_ReturnsViewWithError()
        {
            var result = controller.ImportData(null);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(FILE_ERROR, controller.ModelState["file"].Errors.First().ErrorMessage);
        }

        [Fact]
        public void ImportData_FileIsEmpty()
        {
            var emptyFile = new Mock<IFormFile>();
            emptyFile.Setup(f => f.Length).Returns(0);

            var result = controller.ImportData(emptyFile.Object);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(FILE_ERROR, controller.ModelState["file"].Errors.First().ErrorMessage);
        }
    }
}