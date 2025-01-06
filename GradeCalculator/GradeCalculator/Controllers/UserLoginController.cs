using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GradeCalculator.ViewModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using GradeCalculator.Models;
using Microsoft.EntityFrameworkCore;
using GradeCalculator.Security;
using GradeCalculator.Service;

namespace GradeCalculator.Controllers
{
    public class UserLoginController : Controller
    {
        private readonly PiGradeCalculatorContext _context;
        private readonly LogService _logService;

        public UserLoginController(PiGradeCalculatorContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var model = new AutentikacijaVM
            {
                ReturnUrl = returnUrl
            };

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(AutentikacijaVM model)
        {
            var postojeciKorisnik =
                _context
                .Korisniks
                .Include(x => x.Uloga)
                    .FirstOrDefault(x => x.KorisnickoIme == model.Username);
            if (postojeciKorisnik == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }
            var b64hash = PasswordProvider.GetHash(model.Password, postojeciKorisnik.LozinkaSalt);
            if (b64hash != postojeciKorisnik.LozinkaHash)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Role, postojeciKorisnik.Uloga.Naziv)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            Task.Run(async () =>
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties)
            ).GetAwaiter().GetResult();

            if (model.ReturnUrl != null)
                return LocalRedirect(model.ReturnUrl);
            else if (postojeciKorisnik.UlogaId == 2)
            {
                _logService.AddLog($"Osoba sa ulogom{postojeciKorisnik.UlogaId} se prijavio");
                return RedirectToAction("Index", "Korisnik");
            }
            else if (postojeciKorisnik.UlogaId == 1)
            {
                _logService.AddLog($"Osoba sa ulogom{postojeciKorisnik.UlogaId} se prijavio");
                return RedirectToAction("Index", "Home");
            }
            else
                return View();

        }
        public IActionResult Logout()
        {
            Task.Run(async () =>
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme)
            ).GetAwaiter().GetResult();

            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(KorisnikVM model)
        {
            try
            {
                var trimmedUsername = model.UserName.Trim();
                if (_context.Korisniks.Any(x => x.KorisnickoIme.Equals(trimmedUsername)))
                    return BadRequest($"Username {trimmedUsername} already exists");

                var b64salt = PasswordProvider.GetSalt();
                var b64hash = PasswordProvider.GetHash(model.Password, b64salt);

                var korisnik = new Korisnik
                {
                    Idkorisnik = model.Id,
                    KorisnickoIme = model.UserName,
                    Eposta = model.Email,
                    LozinkaHash = b64hash,
                    LozinkaSalt = b64salt,
                    Godinas = new List<Godina>(),
                    UkupnaOcjena = 0.00,
                    UlogaId = 1
                    
                };
                _context.Add(korisnik);
                _context.SaveChanges();
                model.Id = korisnik.Idkorisnik;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
