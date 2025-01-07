using GradeCalculator.Models;
using GradeCalculator.Security;
using GradeCalculator.Service;
using GradeCalculator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradeCalculator.Utilities;

namespace GradeCalculator.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly PiGradeCalculatorContext _context;
        private readonly StatistikaService _statistikaService;
        private readonly KorisnikService _korisnikService;

        public KorisnikController(PiGradeCalculatorContext context, StatistikaService statistikaService, KorisnikService korisnikService)
        {
            _context = context;
            _statistikaService = statistikaService;
            _korisnikService = korisnikService;
        }

        private const int REGULAR_USER_ID = 1;

        // GET: KorisnikController
        public ActionResult Index()
        {
            var usersVMs = _korisnikService.GetAllUsers();

            return View(usersVMs);
        }

        public ActionResult Details(int id)
        {
            var user = _korisnikService.GetUserDetails(id);
            if (user == null)
            {
                return NotFound($"Could not find user with id {id}");
            }

            var userVm = new ShowKorisnikVM
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                TotalGrade = user.TotalGrade,
                RoleName = user.RoleName
            };

            return View(userVm);
        }

        public ActionResult Profile()
        {
            int userId = ProfileUtils.GetLoggedInUserId();
            ViewBag.UserID = userId;

            var user = _korisnikService.GetUserDetails(userId);
            if (user == null)
            {
                return NotFound($"Could not find user you're looking for");
            }

            var userVm = new ShowKorisnikVM
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                TotalGrade = user.TotalGrade,
                RoleName = user.RoleName
            };

            return View(userVm);
        }

        public JsonResult GetProfileData(int id)
        {
            var user = _context.Korisniks
                .Include(p => p.Uloga)
                .First(p => p.Idkorisnik == id);
            return Json(new
            {
                user.KorisnickoIme,
                user.Eposta,
                user.UkupnaOcjena
            });
        }

        [HttpPut]
        public ActionResult SetProfileData(int id, [FromBody] ShowKorisnikVM userVm)
        {
            var user = _context.Korisniks.First(p => p.Idkorisnik == id);
            user.Eposta = userVm.Email;
            user.KorisnickoIme = userVm.UserName;

            _context.SaveChanges();
            
            return Ok();
        }

        [HttpPut]
        public ActionResult ChangePassword(int id, [FromBody] ChangePasswordVM passwordVm)
        {
            var user = _context.Korisniks.First(p => p.Idkorisnik == id);

            user.LozinkaSalt = PasswordProvider.GetSalt();
            user.LozinkaHash = PasswordProvider.GetHash(passwordVm.NewPassword, user.LozinkaSalt);

            _context.SaveChanges();

            return Ok();
        }

        // GET: KorisnikController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KorisnikController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KorisnikVM userVm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View();
                if (_korisnikService.IsEmailTaken(userVm.Email)) 
                    return View();
                if (_korisnikService.IsUsernameTaken(userVm.UserName)) 
                    return View();

                var b64salt = PasswordProvider.GetSalt();
                var b64hash = PasswordProvider.GetHash(userVm.Password, b64salt);

                var user = new Korisnik
                {
                    KorisnickoIme = userVm.UserName,
                    Eposta = userVm.Email,
                    LozinkaHash = b64hash,
                    LozinkaSalt = b64salt,
                    UlogaId = REGULAR_USER_ID
                };

                _context.Add(user);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: KorisnikController/Delete/5
        public ActionResult Delete(int id)
        {
            var user = _korisnikService.GetUserDetails(id);
            var userVm = new ShowKorisnikVM
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
            return View(userVm);
        }

        // POST: KorisnikController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ShowKorisnikVM userVm)
        {
            try
            {
                _korisnikService.RemoveUser(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: 
        public JsonResult GetDataPoints()
        {
            
            var ocjene = _statistikaService.KalkulacijaUkupnihProsjeka();
            List<DataPoint> dataPoints = new List<DataPoint>();

            for (int i = 0; i <= 5; i++) 
            {
                double value = ocjene.ContainsKey(i) ? ocjene[i] : 0;
                dataPoints.Add(new DataPoint(i.ToString(), value));
            }
            
            return Json(dataPoints);
        }
    }
}
