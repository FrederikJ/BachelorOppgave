using HovedOppgave.Models;
using HovedOppgave.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HovedOppgave.Controllers
{
    public class KalibreringController : Controller
    {
        IRepository myRepository = new Repository();

        // GET: Kalibrering
        public ActionResult Overview()
        {
            Device device = new Device("heia", "dette", "214952", 123, 543, true, "911", "Porsche", 65);
            Device device1 = new Device("hallo", "skjer", "765433", 321, 345, false, "M8", "BMW", 75);
            List<Device> list = new List<Device>();
            list.Add(device);
            list.Add(device1);
            return View(list);
        }

        // GET: Kalibrering/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Kalibrering/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Kalibrering/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Overview");
            }
            catch
            {
                return View();
            }
        }

        // GET: Kalibrering/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Kalibrering/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Overview");
            }
            catch
            {
                return View();
            }
        }

        // GET: Kalibrering/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Kalibrering/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Overview");
            }
            catch
            {
                return View();
            }
        }

        // GET: Kalibrering/Discarded
        public ActionResult Discarded()
        {
            return View();
        }

        // POST: Kalibrering/Discarded
        [HttpPost]
        public ActionResult Discarded(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Overview");
            }
            catch
            {
                return View();
            }
        }
    }
}
