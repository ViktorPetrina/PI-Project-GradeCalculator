using GradeCalculator.Models;
using GradeCalculator.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GradeCalculator.Controllers
{
    public class KorisnikController : Controller
    {
        // GET: KorisnikController
        public ActionResult Index()
        {
            return View();
        }

        // GET: KorisnikController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: KorisnikController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KorisnikController/Create
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
        //GET: KorisnikController
        public ActionResult StatisticsGet()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            //Add datapoints
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            return View();
        }
    }
}
