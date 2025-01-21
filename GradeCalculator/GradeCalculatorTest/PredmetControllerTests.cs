using AutoMapper;
using GradeCalculator.Controllers;
using GradeCalculator.Models;
using GradeCalculator.Repository;
using GradeCalculator.Service;
using GradeCalculator.Utilities;
using GradeCalculator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeCalculatorTest
{
    public class PredmetControllerTests
    {
        private readonly Mock<IRepository<Godina>> mockYearRepo;
        private readonly Mock<IRepository<Predmet>> mockSubjectRepo;
        private readonly Mock<IRepository<Ocjena>> mockGradeRepo;
        private readonly Mock<IMapper> mockMapper;
        private readonly PredmetController controller;
        private readonly PiGradeCalculatorContext context;

        public PredmetControllerTests()
        {
            mockYearRepo = new Mock<IRepository<Godina>>();
            mockSubjectRepo = new Mock<IRepository<Predmet>>();
            mockGradeRepo = new Mock<IRepository<Ocjena>>();
            mockMapper = new Mock<IMapper>();
            context = new PiGradeCalculatorContext();

            controller = new PredmetController(
            context,
            mockGradeRepo.Object,
            mockSubjectRepo.Object,
            mockYearRepo.Object,
            mockMapper.Object,
            null,
            new LogService());
        }

        [Fact]
        public void Index_ReturnsPredmetList()
        {
            var subjects = new List<Predmet>
            {
                new Predmet { Idpredmet = 1, Naziv = "Matematika", Prosjek = 4.5, GodinaId = 1 },
                new Predmet { Idpredmet = 2, Naziv = "Programiranje", Prosjek = 3.8, GodinaId = 1 }
            };
            var subjectVms = new List<PredmetVM>
            {
                new PredmetVM { Idpredmet = 1, Naziv = "Matematika", Prosjek = 4.5, GodinaId = 1 },
                new PredmetVM { Idpredmet = 2, Naziv = "Programiranje", Prosjek = 3.8, GodinaId = 1 }
            };

            mockSubjectRepo.Setup(repo => repo.GetAll()).Returns(subjects);
            mockMapper.Setup(m => m.Map<IEnumerable<PredmetVM>>(It.IsAny<IEnumerable<Predmet>>())).Returns(subjectVms);

            var result = controller.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            var model = result.Model as IEnumerable<PredmetVM>;
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void SubjectsByYear_RetursValidSubjects()
        {
            int yearId = 1;
            var subjects = new List<Predmet>
            {
                new Predmet { Idpredmet = 1, Naziv = "Matematika", Prosjek = 4.5, GodinaId = 1 },
                new Predmet { Idpredmet = 2, Naziv = "Programiranje", Prosjek = 3.8, GodinaId = 1 }
            };
            var subjectVms = new List<PredmetVM>
            {
                new PredmetVM { Idpredmet = 1, Naziv = "Matematika", Prosjek = 4.5, GodinaId = 1 },
                new PredmetVM { Idpredmet = 2, Naziv = "Programiranje", Prosjek = 3.8, GodinaId = 1 }
            };
            var year = new Godina { Idgodina = yearId, Naziv = "Prvi semestar" };

            mockSubjectRepo.Setup(repo => repo.GetAll()).Returns(subjects.AsQueryable());
            mockMapper.Setup(m => m.Map<IEnumerable<PredmetVM>>(It.IsAny<IEnumerable<Predmet>>())).Returns(subjectVms);
            mockYearRepo.Setup(repo => repo.Get(yearId)).Returns(year);

            var result = controller.SubjectsByYear(yearId) as ViewResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            var model = result.Model as IEnumerable<PredmetVM>;
            Assert.Equal(2, model.Count());
            Assert.Equal("Matematika", model.First().Naziv);
            Assert.Equal("Prvi semestar", controller.ViewBag.YearName);
        }

        [Fact]
        public void Details_ReturnsValidView()
        {
            int subjectId = 1;
            var subject = new Predmet { Idpredmet = subjectId, Naziv = "Matematika", Prosjek = 4.5, GodinaId = 1 };
            var subjectVm = new PredmetVM { Idpredmet = subjectId, Naziv = "Matematika", Prosjek = 4.5, GodinaId = 1 };

            mockSubjectRepo.Setup(repo => repo.Get(subjectId)).Returns(subject);
            mockMapper.Setup(m => m.Map<PredmetVM>(subject)).Returns(subjectVm);

            var result = controller.Details(subjectId) as ViewResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            var model = result.Model as PredmetVM;
            Assert.NotNull(model);
            Assert.Equal("Matematika", model.Naziv);
            Assert.Equal("Matematika", controller.ViewBag.SubjectName);
        }

        [Fact]
        public void Create_Valid()
        {
            var yearListItems = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Prvi semestar" },
                new SelectListItem { Value = "2", Text = "Drugi semestar" }
            };

            controller.ViewBag.GodineListItems = yearListItems;

            var result = controller.Create() as ViewResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            var model = result.Model as PredmetVM;
            Assert.NotNull(model);
            Assert.NotNull(controller.ViewBag.GodineListItems);
        }

        [Fact]
        public void Edit_Valid_GET()
        {
            int subjectId = 1;
            var subject = new Predmet { Idpredmet = subjectId, Naziv = "Matematika", GodinaId = 1 };
            var subjectVm = new PredmetVM { Idpredmet = subjectId, Naziv = "Matematika", GodinaId = 1 };
            var yearListItems = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Prvi semestar" },
                new SelectListItem { Value = "2", Text = "Drugi semestar" }
            };

            mockSubjectRepo.Setup(repo => repo.Get(subjectId)).Returns(subject);
            mockMapper.Setup(m => m.Map<PredmetVM>(subject)).Returns(subjectVm);
            controller.ViewBag.GodineListItems = yearListItems;

            var result = controller.Edit(subjectId) as ViewResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            var model = result.Model as PredmetVM;
            Assert.NotNull(model);
            Assert.Equal("Matematika", model.Naziv);
            Assert.NotNull(controller.ViewBag.GodineListItems);
        }

        [Fact]
        public void Edit_Valid_POST()
        {
            int subjectId = 1;
            var subjectVm = new PredmetVM { Idpredmet = subjectId, Naziv = "Matematika", GodinaId = 1 };
            var subject = new Predmet { Idpredmet = subjectId, Naziv = "Matematika", GodinaId = 1 };

            mockMapper.Setup(m => m.Map<Predmet>(subjectVm)).Returns(subject);

            var result = controller.Edit(subjectId, subjectVm) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("SubjectsByYear", result.ActionName);
            Assert.Equal(subjectVm.GodinaId, result.RouteValues["id"]);
            mockSubjectRepo.Verify(repo => repo.Modify(subjectId, subject), Times.Once);
        }

        [Fact]
        public void Delete_Valid_GET()
        {
            int subjectId = 1;
            var subject = new Predmet { Idpredmet = subjectId, Naziv = "Matematika", GodinaId = 1 };
            var subjectVm = new PredmetVM { Idpredmet = subjectId, Naziv = "Matematika", GodinaId = 1 };

            mockSubjectRepo.Setup(repo => repo.Get(subjectId)).Returns(subject);
            mockMapper.Setup(m => m.Map<PredmetVM>(subject)).Returns(subjectVm);

            var result = controller.Delete(subjectId) as ViewResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            var model = result.Model as PredmetVM;
            Assert.NotNull(model);
            Assert.Equal("Matematika", model.Naziv);
        }

        [Fact]
        public void Delete_Valid_POST()
        {
            int subjectId = 1;
            var subjectVm = new PredmetVM { Idpredmet = subjectId, Naziv = "Matematika", GodinaId = 1 };

            var result = controller.Delete(subjectId, subjectVm) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("SubjectsByYear", result.ActionName);
            Assert.Equal(subjectId, result.RouteValues["id"]);
            mockSubjectRepo.Verify(repo => repo.Remove(subjectId), Times.Once);
        }
    }
}
