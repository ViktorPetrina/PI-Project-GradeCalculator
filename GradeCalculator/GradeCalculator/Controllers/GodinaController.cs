using AutoMapper;
using GradeCalculator.Interfaces;
using GradeCalculator.Models;
using GradeCalculator.Repository;
using GradeCalculator.Service;
using GradeCalculator.Utilities;
using GradeCalculator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Composition;
using System.Text;

// TODO:
// dodati godine userima i tako ih prikazivati

namespace GradeCalculator.Controllers                   
{
    public class GodinaController : Controller, IAveragable
    {
        private const string FILE_ERROR = "Unesite valjanu json datoteku.";
        private const string YEAR_EXISTS_ERROR = "Vec postoji godina sa istim nazivom";

        private readonly PiGradeCalculatorContext _context;
        private readonly IRepository<Godina> _godinaRepo;
        private readonly IRepository<Predmet> _subjectRepo;
        private readonly IMapper _mapper;
        private readonly LogService _logService;

        public GodinaController(
            PiGradeCalculatorContext context,
            IRepository<Predmet> subjectRepo,
            IRepository<Godina> yearRepo,
            IMapper mapper, 
            LogService logService)
        {
            _context = context;
            _godinaRepo = yearRepo;
            _subjectRepo = subjectRepo;
            _mapper = mapper;
            _logService = logService;
        }

        // GET: GodinaController
        public ActionResult Index()
        {
            var years = _godinaRepo.GetAll();
            var yearVms = _mapper.Map<IEnumerable<GodinaVM>>(years);

            ViewBag.Id = yearVms.FirstOrDefault()?.KorisnikId;

            return View(yearVms);
        }

        public ActionResult ExportData(int id)
        {
            var years = _godinaRepo.GetAll().Where(g => g.KorisnikId == id);

            var exportHelper = ExportHelperFactory.GetJsonExportHelper();

            return exportHelper.GetDownload(years);
        }

        public ActionResult ImportData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportData(IFormFile file)
        {
            if (file == null || file.Length == 0) 
            { 
                ModelState.AddModelError("file", FILE_ERROR); 
                return View(); 
            }

            var fileImporter = new FileImporterBuilder()
                .SetImportFile(file)
                .SetRepository(_godinaRepo)
                .SetImportFileType(FileType.Json)
                .Build();

            fileImporter.ImportFile();

            return RedirectToAction(nameof(Index));
        }

        // GET: GodinaController/Create
        public ActionResult Create()
        {
            var year = new GodinaVM();

            return View(year);
        }

        // POST: GodinaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GodinaVM godinaVm)
        {
            try
            {
                if (_godinaRepo.GetAll().Any(g => g.Naziv == godinaVm.Naziv))
                {
                    ModelState.AddModelError("", YEAR_EXISTS_ERROR);

                    return View();
                }

                var year = _mapper.Map<Godina>(godinaVm);
                year.KorisnikId = 1;
                _godinaRepo.Add(year);
                //_logService.AddLog("Korisnik spremio godinu u bazu.");

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

        // GET: GodinaController/Delete/5
        public ActionResult Delete(int id)
        {
            var year = _mapper.Map<GodinaVM>(_godinaRepo.Get(id)); 

            return View(year);
        }

        // POST: GodinaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, GodinaVM godina)
        {
            try
            {
                _subjectRepo.GetAll().ToList().ForEach(p => 
                { 
                    if (p.GodinaId == id) 
                        _subjectRepo.Remove(p.Idpredmet); 
                } );
                _godinaRepo.Remove(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public double? GetAverage(int id)
        {
            var year = _godinaRepo.Get(id);

            if(year == null)
                return null;

            List<double?> subjects = _context.Predmets
                .Include(s => s.Godina)
                .AsEnumerable()
                .Where(s => year.Predmets.Any(p => p.Naziv.ToLower() == s.Naziv.ToLower()))
                .Select(s => s.Prosjek)
                .ToList();

            if(subjects == null)
                return null;

            return Math.Round((double)subjects.Average(), 2, MidpointRounding.AwayFromZero);
        }
    }
}
