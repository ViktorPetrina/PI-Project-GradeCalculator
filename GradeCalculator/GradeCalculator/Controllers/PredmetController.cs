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
        private readonly IRepository<Predmet> _subjectRepo;
        private readonly IRepository<Godina> _yearRepo;
        private readonly IMapper _mapper;
        private readonly StatistikaService _statistikaService;
        private readonly LogService _logService;
        public PredmetController(
            IRepository<Predmet> subjectRepo, 
            IRepository<Godina> yearRepo, 
            IMapper mapper, 
            StatistikaService statistikaService, 
            LogService logService)
        {
            _subjectRepo = subjectRepo;
            _yearRepo = yearRepo;
            _mapper = mapper;
            _statistikaService = statistikaService;
            _logService = logService;
        }

        // GET: PredmetController
        public ActionResult Index()
        {
            var subjects = _subjectRepo.GetAll();
            var subjectVms = _mapper.Map<IEnumerable<PredmetVM>>(subjects);

            return View(subjectVms);
        }

        // GET: PredmetController/SubjectsByYear/5
        public ActionResult SubjectsByYear(int id)
        {
            var subjects = _subjectRepo.GetAll();
            var subjectVms = _mapper.Map<IEnumerable<PredmetVM>>(subjects);

            subjectVms = subjectVms.Where(s => s.GodinaId == id);

            return View(subjectVms);
        }

        // GET: PredmetController/Details/5
        public ActionResult Details(int id)
        {
            var subject = _subjectRepo.Get(id);
            var subjectVm = _mapper.Map<PredmetVM>(subject);

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
                if (_subjectRepo.GetAll().Any(p => p.Naziv == subjectVm.Naziv))
                {
                    ModelState.AddModelError("", "Vec postoji predmet sa istim nazivom");

                    return View();
                }

                var subject = _mapper.Map<Predmet>(subjectVm);
                _subjectRepo.Add(subject);
                _logService.AddLog("Korisnik spremio predmet u bazu.");

                return RedirectToAction("SubjectsByYear", new {id = subjectVm.GodinaId});
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
                var subject = _subjectRepo.Get(id);
                var subjectVm = _mapper.Map<PredmetVM>(subject);

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
                var subject = _mapper.Map<Predmet>(subjectVm);

                _subjectRepo.Modify(id, subject);

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
            var subject = _mapper.Map<PredmetVM>(_subjectRepo.Get(id));

            return View(subject);
        }

        // POST: PredmetController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, PredmetVM predmetVM)
        {
            try
            {
                _subjectRepo.Remove(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public IEnumerable<SelectListItem> GetYears()
        {
            return _yearRepo.GetAll()
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
