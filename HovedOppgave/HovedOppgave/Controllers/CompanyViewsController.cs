using HovedOppgave.Models;
using HovedOppgave.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HovedOppgave.Controllers
{
    public class CompanyViewsController : Controller
    {
        IRepository myrep = new Repository();

        /**
         * hvem som har tilgang til sidene
         */
        public CompanyViewsController()
        {
            //SessionCheck.CheckForRightsOnLogInUser(Constant.Rights.Administrator);
            //SessionCheck.CheckForRightsOnLogInUser(Constant.Rights.User);
        }

        /**
         * ser detaljer om kontakt person til et firma + firmaet
         */
        // GET: CompanyViews
        public ActionResult CompanyDetails(int id)
        {
            CompanyWithContact model = myrep.GetCompanyWithContactInfo(id);
            
            string master = SessionCheck.FindMaster();
            return View("CompanyDetails", master, model);
        }

        /**
         * endre firma og kontakt person detaljer
         */
        // POST: CompanyViews/Edit/5
        [HttpPost]
        public ActionResult EditContactCompany(CompanyWithContact model)
        {
            try
            {
                myrep.EditContact(model.Contact);
                myrep.EditContactInfo(model.ContactInfo);
                myrep.EditContactInfoType(model.ContactInfoType);
                myrep.EditCompany(model.Company);
                return RedirectToAction("CompanyDetails", new { id = model.Company.CompanyID });
            }
            catch
            {
                return View(model);
            }
        }
    }
}
