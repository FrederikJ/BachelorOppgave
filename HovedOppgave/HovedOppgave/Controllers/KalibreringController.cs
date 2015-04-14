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
            List<CalibrationViews> modelList = new List<CalibrationViews>();
            List<LogEvent> logEventList = myrep.GetAllLogEvent();

            foreach (var item in logEventList)
            {
                if (item.EndDate.Date >= DateTime.Now.Date)
                {
                    var device = myrep.GetDevice(item.DeviceID);
                    var room = myrep.GetRoom(item.RoomID);
                    var eventType = myrep.GetEventType(item.EventTypeID);
                    var contact = myrep.GetContact(item.ContactID);

                    CalibrationViews model = new CalibrationViews();
                    model.Contact = contact;
                    model.Device = device;
                    model.EventType = eventType;
                    model.LogEvent = item;
                    model.Room = room;

                    modelList.Add(model);
                }
            }
            return View(modelList);
        }

        // GET: Kalibrering/Create
        public ActionResult Create()
        {
            CreateCalibrationViewModel model = this.CreateCalibrationView();
            return View(model);
        }

        // POST: Kalibrering/Create
        [HttpPost]
        public ActionResult Create(CreateCalibrationViewModel model,
            IEnumerable<int> SelectedDevice, IEnumerable<int> SelectedEventType,
            IEnumerable<int> SelectedRoom, IEnumerable<int> SelectedContact,
            HttpPostedFileBase file)
        {
            LogEvent calibration = new LogEvent();
            
            //Sjekker om dem har skrevet inn feil dato. Feil dato forekommer standar verdien 01.01.0001
            if (model.LogEvent.EndDate.Year == 1 || model.LogEvent.StartDate.Year == 1 || model.LogEvent.RegisteredDate.Year == 1)
            {
                CreateCalibrationViewModel newmodel = this.CreateCalibrationView();
                return View(newmodel);
            }
            else
            {
                if (model.LogEvent.EndDate >= model.LogEvent.RegisteredDate && model.LogEvent.EndDate >= model.LogEvent.StartDate)
                {
                    //Fyller inn i log event som skal inn i db
                    calibration.RegisteredDate = model.LogEvent.RegisteredDate;
                    calibration.StartDate = model.LogEvent.StartDate;
                    calibration.EndDate = model.LogEvent.EndDate;
                    calibration.ContactID = SelectedContact.FirstOrDefault();
                    calibration.DeviceID = SelectedDevice.FirstOrDefault();
                    calibration.EventTypeID = SelectedEventType.FirstOrDefault();
                    calibration.RoomID = SelectedRoom.FirstOrDefault();
                    if (model.LogEvent.DataFilePath != null)
                        calibration.DataFilePath = model.LogEvent.DataFilePath;
                    if (model.LogEvent.Data1 != null)
                        calibration.Data1 = model.LogEvent.Data1;
                    if (model.LogEvent.Data2 != null)
                        calibration.Data2 = model.LogEvent.Data2;

                    try
                    {
                        myrep.CreateKalibreringLogEvent(calibration);

                        return RedirectToAction("Overview");
                    }
                    catch
                    {
                        CreateCalibrationViewModel newmodel = this.CreateCalibrationView();
                        return View(newmodel);
                    }
                }
                model = this.CreateCalibrationView();
                return View(model);
            }
        }

        // GET: Kalibrering/Edit/5
        public ActionResult EditCalibration(int id)
        {
            LogEvent logEvent = myrep.GetLogEvent(id);
            CreateCalibrationViewModel model = this.CreateCalibrationView();
            model.LogEvent = logEvent;
            return View(model);
        }

        // POST: Kalibrering/Edit/5
        [HttpPost]
        public ActionResult EditCalibration(CreateCalibrationViewModel model,
            IEnumerable<int> SelectedDevice, IEnumerable<int> SelectedEventType,
            IEnumerable<int> SelectedRoom, IEnumerable<int> SelectedContact)
        {
            LogEvent calibration = new LogEvent();
            CreateCalibrationViewModel newmodel;
            LogEvent logEvent;

            //Sjekker om dem har skrevet inn feil dato. Feil dato forekommer standar verdien 01.01.0001
            if (model.LogEvent.EndDate.Year == 1 || model.LogEvent.StartDate.Year == 1 || model.LogEvent.RegisteredDate.Year == 1)
            {
                logEvent = myrep.GetLogEvent(model.LogEvent.LogEventID);
                newmodel = this.CreateCalibrationView();
                newmodel.LogEvent = logEvent;
                return View(newmodel);
            }
            else
            {
                if (model.LogEvent.EndDate >= model.LogEvent.RegisteredDate && model.LogEvent.EndDate >= model.LogEvent.StartDate)
                {
                    //Fyller inn i log event som skal inn i db
                    calibration.RegisteredDate = model.LogEvent.RegisteredDate;
                    calibration.StartDate = model.LogEvent.StartDate;
                    calibration.EndDate = model.LogEvent.EndDate;
                    calibration.ContactID = SelectedContact.FirstOrDefault();
                    calibration.DeviceID = SelectedDevice.FirstOrDefault();
                    calibration.EventTypeID = SelectedEventType.FirstOrDefault();
                    calibration.RoomID = SelectedRoom.FirstOrDefault();
                    if (model.LogEvent.DataFilePath != null)
                        calibration.DataFilePath = model.LogEvent.DataFilePath;
                    if (model.LogEvent.Data1 != null)
                        calibration.Data1 = model.LogEvent.Data1;
                    if (model.LogEvent.Data2 != null)
                        calibration.Data2 = model.LogEvent.Data2;

                    try
                    {
                        if(myrep.EditLogEvent(calibration))
                            return RedirectToAction("Overview");
                    }
                    catch
                    {
                        
                    }
                }
            }
            logEvent = myrep.GetLogEvent(model.LogEvent.LogEventID);
            newmodel = this.CreateCalibrationView();
            newmodel.LogEvent = logEvent;
            return View(newmodel);
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

        // GET: Kalibrering/History
        public ActionResult History()
        {
            List<CalibrationViews> modelList = new List<CalibrationViews>();
            List<LogEvent> logEventList = myrep.GetAllLogEvent();

            foreach (var item in logEventList)
            {
                if (item.EndDate.Date <= DateTime.Now.Date)
                {
                    var device = myrep.GetDevice(item.DeviceID);
                    var room = myrep.GetRoom(item.RoomID);
                    var eventType = myrep.GetEventType(item.EventTypeID);
                    var contact = myrep.GetContact(item.ContactID);

                    CalibrationViews model = new CalibrationViews();
                    model.Contact = contact;
                    model.Device = device;
                    model.EventType = eventType;
                    model.LogEvent = item;
                    model.Room = room;

                    modelList.Add(model);
                }
            }
            return View(modelList);
        }

        // GET: Kalibrering/Details
        public ActionResult CalibrationViewDetails(int id)
        {
            CalibrationViews model = this.CalibrationViews(id);
            return View(model);
        }

        #region Create Calibration view method
        public CreateCalibrationViewModel CreateCalibrationView()
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
                    Text = item.Name,
                    Value = item.ContactID.ToString()
                };
                contactList.Add(selectlist);
            }

            CreateCalibrationViewModel model = new CreateCalibrationViewModel()
            {
                Device = deviceList,
                Contact = contactList,
                EventType = eventTypeList,
                Room = roomList
            };

            return model;
        }
        #endregion
        #region Calibration view model
        public CalibrationViews CalibrationViews(int id)
        {
            var logEvent = myrep.GetLogEvent(id);
            var device = myrep.GetDevice(logEvent.DeviceID);
            var room = myrep.GetRoom(logEvent.RoomID);
            var eventType = myrep.GetEventType(logEvent.EventTypeID);
            var contact = myrep.GetContact(logEvent.ContactID);

            CalibrationViews model = new CalibrationViews();
            model.LogEvent = logEvent;
            model.Room = room;
            model.Device = device;
            model.Contact = contact;
            model.EventType = eventType;

            return model;
        }
        #endregion
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
