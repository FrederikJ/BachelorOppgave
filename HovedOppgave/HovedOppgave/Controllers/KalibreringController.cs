using HovedOppgave.Models;
using HovedOppgave.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace HovedOppgave.Controllers
{
    public class KalibreringController : Controller
    {
        IRepository myrep = new Repository();

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
            List<SelectListItem> deviceList = new List<SelectListItem>();
            List<SelectListItem> roomList = new List<SelectListItem>();
            List<SelectListItem> eventTypeList = new List<SelectListItem>();
            List<SelectListItem> contactList = new List<SelectListItem>();

            foreach (Device item in myrep.GetAllDevices())
            {
                SelectListItem selectlist = new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.DeviceID.ToString()
                };
                deviceList.Add(selectlist);
            }
            foreach (Room item in myrep.GetAllRooms())
            {
                SelectListItem selectlist = new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.RoomID.ToString()
                };
                roomList.Add(selectlist);
            }
            foreach (EventType item in myrep.GetAllEventTypes())
            {
                SelectListItem selectlist = new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.EventTypeID.ToString()
                };
                eventTypeList.Add(selectlist);
            }
            foreach (Contact item in myrep.GetAllContacts())
            {
                SelectListItem selectlist = new SelectListItem()
                {
                    Text = item.FirstName + item.LastName,
                    Value = item.ContactID.ToString()
                };
                contactList.Add(selectlist);
            }

            CreateCalibrationViewModel model = new CreateCalibrationViewModel()
            {
                ListDevice = deviceList,
                ListContact = contactList,
                ListEventType = eventTypeList,
                ListRoom = roomList
            };

            return View(model);
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

        // GET: Kalibrering/Import
        public ActionResult Import()
        {
            return View();
        }

        // POST: Kalibrering/Import
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file)
        {
            try
            {
                if (Validator.IsValidFile(file, 5))
                {
                    return RedirectToAction("Overview");
                }
                return RedirectToAction("Import");
                
            }
            catch
            {
                return View();
            }
        }
        
        public FileStreamResult DisplayFile()
        {
            var path = "C:/Users/Frederik/Documents/Bachelor Oppgave/info fra kunden/Dok_konnektorer_mottaker.pdf";
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            return File(fs, "application/pdf");

            /*if (Validator.IsValidFile(file, 5))
            {
                var extension = Path.GetExtension(file.FileName);
                if (extension == ".pdf")
                    return File(fs, "application/pdf");
                else
                    return File(fs, "application/csv");
            }
            else
                return null;*/
        }

        public ActionResult License()
        {
            string[] pathOfFiles = Directory.GetFiles(@"C:\Users\Frederik\AppData\Sertifikat", "*.pdf");
            

            foreach(string pathOfFile in pathOfFiles)
            {
                FileStream stream = new FileStream(pathOfFile, FileMode.Create);
                
                
                
            }
            
            

            return View();
        }

        // GET: Kalibrering/Import
        public ActionResult History()
        {
            return View();
        }

        // POST: Kalibrering/Import
        [HttpPost]
        public ActionResult History(string file)
        {
            return null;
        }

        /*
        [HttpPost]
        public async Task<JsonResult> TempSaveFIle()
        {
            try
            {
                foreach(string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if(fileContent != null && fileContent.ContentLength > 0)
                    {
                        var stream = fileContent.InputStream;
                        var fileName = Path.GetFileName(file);
                        var path = Path.Combine(Server.MapPath("~/App_Data/Sertifikat"), fileName);
                        using(var fileStream = File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch (Exception) {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
            return Json("File uploaded successfully");
        }*/
    }
}
