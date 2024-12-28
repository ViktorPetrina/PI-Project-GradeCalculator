using AutoMapper;
using GradeCalculator.Models;
using GradeCalculator.Repository;
using GradeCalculator.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GradeCalculator.Controllers
{
    public class GodinaController : Controller
    {
        private readonly IRepository<Godina> _godinaRepo;
        private readonly IMapper _mapper;

        public GodinaController(IRepository<Godina> godinaRepo, IMapper mapper)
        {
            _godinaRepo = godinaRepo;
            _mapper = mapper;
        }

        // GET: GodinaController
        public ActionResult Index()
        {
            var years = _godinaRepo.GetAll();
            var yearVms = _mapper.Map<IEnumerable<GodinaVM>>(years);

            return View(yearVms);
        }

        // mozda maknuti details jer je nepotrebno ?
        // GET: GodinaController/Details/5
        public ActionResult Details(int id)
        {
            var year = _godinaRepo.Get(id);
            var yearVm = _mapper.Map<GodinaVM>(year);

            return View(yearVm);
        }

        // GET: GodinaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GodinaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Subjects(int _id)
        {
            return RedirectToAction("SubjectsByYear", "Predmet", new {id = _id});
        }

        // GET: GodinaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GodinaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GodinaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GodinaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
