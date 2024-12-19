using GradeCalculator.Models;
using GradeCalculator.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GradeCalculator.ViewModels;
using NuGet.Protocol;
using System.Diagnostics;

namespace GradeCalculator.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly PiGradeCalculatorContext _context;

        public KorisnikController(PiGradeCalculatorContext context)
        {
            _context = context;
        }

        private const int REGULAR_USER_ID = 1;

        // GET: KorisnikController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var user = _context.Korisniks
                .Include(r => r.Uloga)
                .FirstOrDefault(r => r.Idkorisnik == id);
            if (user == null)
            {
                return NotFound($"Could not find user with id {id}");
            }

            var userVm = new KorisnikVM
            {
                Id = user.Idkorisnik,
                UserName = user.KorisnickoIme,
                Email = user.Eposta,
                TotalGrade = user.UkupnaOcjena,
                RoleId = user.UlogaId
            };

            return View(userVm);
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
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    return View();
                }

                if (_context.Korisniks.Any(x => x.KorisnickoIme.Equals(userVm.UserName)))
                {
                    ModelState.AddModelError("UserName", "User already EXISTS");
                    return View();
                }

                if (_context.Korisniks.Any(x => x.Eposta.Equals(userVm.Email)))
                {
                    ModelState.AddModelError("Email", "Email already EXISTS");
                    return View();
                }

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

        // GET: KorisnikController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: KorisnikController/Edit/5
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

        // GET: KorisnikController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: KorisnikController/Delete/5
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
