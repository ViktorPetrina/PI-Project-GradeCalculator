using AutoMapper;
using GradeCalculator.Models;
using GradeCalculator.Repository;
using GradeCalculator.Service;
using GradeCalculator.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GradeCalculator.Controllers
{
    public class PredmetController : Controller
    {
        private readonly IRepository<Predmet> subjectRepo;
        private readonly IRepository<Godina> yearRepo;
        private readonly IMapper mapper;
        private readonly StatistikaService _statistikaService;
        private readonly LogService _logService;
        public PredmetController(IRepository<Predmet> _subjectRepo, IRepository<Godina> _yearRepo, IMapper _mapper, StatistikaService statistikaService, LogService logService)
        {
            subjectRepo = _subjectRepo;
            yearRepo = _yearRepo;
            mapper = _mapper;
            _statistikaService = statistikaService;
            _logService = logService;
        }

        // GET: PredmetController
        public ActionResult Index()
        {
            var subjects = subjectRepo.GetAll();
            var subjectVms = mapper.Map<IEnumerable<PredmetVM>>(subjects);

            return View(subjectVms);
        }

        // GET: PredmetController/Details/5
        public ActionResult Details(int id)
        {
            var subject = subjectRepo.Get(id);
            var subjectVm = mapper.Map<PredmetVM>(subject);

            return View(subjectVm);
        }

        // GET: PredmetController/Create
        public ActionResult Create()
        {
            ViewBag.GodineListItems = GetYears();

            var subject = new PredmetVM();
            
            return View(subject);
        }

        // POST: PredmetController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PredmetVM subjectVm)
        {
            try
            {
                if (subjectRepo.GetAll().Any(p => p.Naziv == subjectVm.Naziv))
                {
                    ModelState.AddModelError("", "Vec postoji predmet sa istim nazivom");

                    return View();
                }

                var subject = mapper.Map<Predmet>(subjectVm);
                subjectRepo.Add(subject);
                _logService.AddLog("Korisnik spremio predmet u bazu.");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PredmetController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var subject = subjectRepo.Get(id);
                var subjectVm = mapper.Map<PredmetVM>(subject);

                ViewBag.GodineListItems = GetYears();

                return View(subjectVm);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // POST: PredmetController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PredmetVM subjectVm)
        {
            try
            {
                var subject = mapper.Map<Predmet>(subjectVm);

                subjectRepo.Modify(id, subject);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(subjectVm);
            }
        }

        // GET: PredmetController/Delete/5
        public ActionResult Delete(int id)
        {
            var subject = mapper.Map<PredmetVM>(subjectRepo.Get(id));

            return View(subject);
        }

        // POST: PredmetController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, PredmetVM predmetVM)
        {
            try
            {
                subjectRepo.Remove(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public IEnumerable<SelectListItem> GetYears()
        {
            return yearRepo.GetAll()
                    .Select(y => new SelectListItem
                    {
                        Text = $"{y.Naziv}",
                        Value = y.Idgodina.ToString()
                    });
        }
        //GET: Predmet/UkupniProsjek
        public JsonResult UkupniProsjek()
        {
            var ukupniProsjek = _statistikaService.KalkulacijaProsjeka();
            return Json(ukupniProsjek);

        }
    }
}
