using GradeCalculator.Models;
using GradeCalculator.Security;
using GradeCalculator.Service;
using GradeCalculator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradeCalculator.Utilities;
using GradeCalculator.Factory;
using GradeCalculator.Adapter;
using GradeCalculator.Builder;

namespace GradeCalculator.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly PiGradeCalculatorContext _context;
        private readonly StatistikaService _statistikaService;
        private readonly IKorisnikService _korisnikService;
        private readonly IKorisnikAdapter _korisnikAdapter;

        public KorisnikController(PiGradeCalculatorContext context, StatistikaService statistikaService, IKorisnikService korisnikService, IKorisnikAdapter korisnikAdapter)
        {
            _context = context;
            _statistikaService = statistikaService;
            _korisnikService = korisnikService;
            _korisnikAdapter = korisnikAdapter;
        }

        // GET: KorisnikController
        public ActionResult Index()
        {
            var usersVMs = _korisnikService.GetAllUsers();

            return View(usersVMs);
        }

        public ActionResult Details(int id)
        {
            var user = _korisnikService.GetUser(id);
            if (user == null)
            {
                return NotFound($"Could not find user with id {id}");
            }

            var userVm = new KorisnikBuilder()
                        .SetId(user.Idkorisnik)
                        .SetUserName(user.KorisnickoIme)
                        .SetEmail(user.Eposta)
                        .SetTotalGrade(user.UkupnaOcjena)
                        .Build();

            return View(userVm);
        }

        public ActionResult Profile()
        {
            int userId = ProfileUtils.GetLoggedInUserId();
            ViewBag.UserID = userId;

            var user = _korisnikService.GetUser(userId);
            if (user == null)
            {
                return NotFound($"Could not find user you're looking for");
            }

            var userVm = _korisnikAdapter.Adapt(user);

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
            var user = _context.Korisniks.FirstOrDefault(p => p.Idkorisnik == id);

            if (user == null)
                return NotFound();
            if (passwordVm.NewPassword != passwordVm.ConfirmPassword)
                return BadRequest();

            user.LozinkaSalt = PasswordProvider.Instance.GetSalt();
            user.LozinkaHash = PasswordProvider.Instance.GetHash(passwordVm.NewPassword, user.LozinkaSalt);

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
                {
                    ModelState.AddModelError("Email", "Email is already taken");
                    return View();
                } 
                if (_korisnikService.IsUsernameTaken(userVm.UserName))
                {
                    ModelState.AddModelError("UserName", "Username is already taken");
                    return View();
                }

                var roleName = userVm.IsAdmin ? "admin" : "basic";
                var factory = KorisnikFactory.GetKorisnik(roleName);
                var user = factory.CreateUser(userVm);

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
            var user = _korisnikService.GetUser(id);
            var userVm = _korisnikAdapter.Adapt(user);
            return View(userVm);
        }

        // POST: KorisnikController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ShowKorisnikVM userVm)
        {
            try
            {
                var user = _korisnikService.GetUser(id);
                if (user == null)
                {
                    ModelState.AddModelError("User", "User not found");
                    return View(userVm);
                }

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
