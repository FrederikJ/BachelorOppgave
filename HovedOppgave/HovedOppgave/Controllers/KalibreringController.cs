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
        SessionCheck sessionCheck = new SessionCheck();

        public KalibreringController()
        {
            //SessionCheck.CheckForRightsOnLogInUser(Constant.Rights.Administrator);
            //SessionCheck.CheckForRightsOnLogInUser(Constant.Rights.User);
        }
        // GET: Kalibrering
        /// <summary>
        /// SETT INN WINDOWS.LOCATION.REPLACE VERDI FOR DETALJE KNAPPEN I "KAN KALIBRERES" KNAPPEN
        /// detalje om enheten
        /// </summary>
        /// <returns></returns>
        public ActionResult Overview()
        {
            CalibrationViews model = new CalibrationViews();
            //Har kodet inn id til kalibrering event type
            List<LogEvent> logEvents = myrep.GetAllLogEventToEventType(11);
            List<DeviceType> deviceTypes = myrep.GetAllDeviceTypes();
            List<Device> devices = myrep.GetAllDevices();
            List<Room> rooms = myrep.GetAllRooms();
            List<Company> companies = myrep.GetAllCompanys();
            List<EventType> eventTypes = myrep.GetAllEventTypes();
            List<Files> files = myrep.GetAllFilesNotDiscarded();
            logEvents.Sort((x, y) => DateTime.Compare(x.StartDate, y.StartDate));
            logEvents.Reverse();
            
            model.DeviceTypes = deviceTypes;
            model.Companys = companies;
            model.Devices = devices;
            model.Rooms = rooms;
            model.EventTypes = eventTypes;
            model.LogEvents = logEvents;
            if (files.Count != 0)
                model.Files = files;

            string master = sessionCheck.FindMaster();
            return View("Overview", master, model);
        }
        #region funker
        // GET: Kalibrering/Create
        public ActionResult Create(int id)
        {
            CalibrationViews model = this.CreateCalibrationView();
            if (id != 0)
            {
                var device = myrep.GetDevice(id);
                var eventType = myrep.GetEventType(11);
                model.Device = device;
                model.EventType = eventType;
            }
            
            
            string master = "";
            int userId = Validator.ConvertToNumbers(Session["UserID"].ToString());
            User user = myrep.GetUser(userId);
            Rights rights = myrep.GetRightToUser(user);

            if (rights.Name == Constant.Rights.Administrator.ToString())
                master = "~/Views/Shared/_AdminLayout.cshtml";
            else if (rights.Name == Constant.Rights.User.ToString())
                master = "~/Views/Shared/_UserLayout.cshtml";
            else if (rights.Name == Constant.Rights.Guest.ToString())
                master = "~/Views/Shared/_GuestLayout.cshtml";

            return View("Create", master, model);
        }

        // POST: Kalibrering/Create
        [HttpPost]
        public ActionResult Create(CalibrationViews model, HttpPostedFileBase file)
        {
            LogEvent calibration = CreatEditFillLogEvent(model, file);

            if (calibration == null)
                return View(model);

            try
            {
                myrep.CreateLogEvent(calibration);
                return RedirectToAction("Overview");
            }
            catch
            {
                return View(model);
            }
        }
        
        // GET: Kalibrering/Edit/5
        public ActionResult EditCalibration(int id)
        {
            CalibrationViews model = this.CalibrationViews(id);
            CalibrationViews model1 = this.CreateCalibrationView();
            model.Devices = model1.Devices;
            model.EventTypes = model1.EventTypes;
            model.Rooms = model1.Rooms;
            model.Files = model1.Files;
            model.Companys = model1.Companys;

            string master = sessionCheck.FindMaster();
            return View("EditCalibration", master, model);
        }

        // POST: Kalibrering/Edit/5
        [HttpPost]
        public ActionResult EditCalibration(CalibrationViews model, HttpPostedFileBase file)
        {
            LogEvent calibration = CreatEditFillLogEvent(model, file);

            if (calibration != null)
                return View(model);

            //siden man evt skifter filen sjekke om det filnavnet ligger i systemet, viss ikke
            //kasseres filen
            if (file != null && calibration.FileID  != 0 && calibration.FileID != model.LogEvent.FileID)
            {
                var tempFile = myrep.GetFile(calibration.FileID);
                tempFile.Kassert = true;
                myrep.EditFile(tempFile);
            }
            else if(model.FileTo != null && model.FileTo.FileID != model.LogEvent.FileID)
            {
                var tempFile = myrep.GetFile(model.LogEvent.FileID);
                tempFile.Kassert = true;
                myrep.EditFile(tempFile);
            }

            try
            {
                myrep.EditLogEvent(calibration);
                return RedirectToAction("Overview");
            }
            catch
            {
                return View(model);
            }
        }
        
        // GET: Kalibrering/Import
        public ActionResult Import(int id)
        {
            CalibrationViews model = new CalibrationViews();
            //Hard kodet inn id for kalibrering
            List<LogEvent> logEvents = myrep.GetAllLogEventToEventType(11);
            logEvents = logEvents.GroupBy(t => t.DeviceID).Select(g => g.Last()).ToList();
            List<Device> devices = new List<Device>();
            List<Company> companies = new List<Company>();
            List<Room> rooms = new List<Room>();
            foreach (var item in logEvents)
            {
                var device = myrep.GetDevice(item.DeviceID);
                var company = myrep.GetCompany(item.CompanyID);
                var room = myrep.GetRoom(item.RoomID);
                
                devices.Add(device);
                companies.Add(company);
                rooms.Add(room);
            }
            //Device id
            if (id != 0)
            {
                Device deviceModel = myrep.GetDevice(id);
                LogEvent logEvent = myrep.GetLastEventForDevice(id);
                Room room = myrep.GetRoom(logEvent.RoomID);
                Company company = myrep.GetCompany(logEvent.CompanyID);
                model.Device = deviceModel;
                model.LogEvent = logEvent;
                model.Room = room;
                model.Company = company;
            }
            
            model.Companys = companies;
            model.LogEvents = logEvents;
            model.Devices = devices;
            model.Rooms = rooms;

            string master = sessionCheck.FindMaster();
            return View("Import", master, model);
        }

        // POST: Kalibrering/Import
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file, CalibrationViews model)
        {
            int id = this.SaveFileToDirectoryAndDB(file);

            try
            {
                if (model.LogEvent.LogEventID != 0)
                {
                    LogEvent logEvent = myrep.GetLogEvent(model.LogEvent.LogEventID);
                    if (logEvent.FileID != 0)
                    {
                        Files tempFile = myrep.GetFile(logEvent.FileID);
                        tempFile.Kassert = true;
                        myrep.EditFile(tempFile);
                    }
                    logEvent.FileID = id;

                    myrep.EditLogEvent(logEvent);
                    return RedirectToAction("License");
                }
                else if(id != 0)
                    return RedirectToAction("License");
                else
                    return View(model);
            }
            catch
            {
                return View(model);
            }
        }

        // GET: Kalibrering/License
        public ActionResult License()
        {
            List<Files> list = myrep.GetAllFilesNotDiscarded();
            CalibrationViews model = new CalibrationViews();
            model.ExtraStringHelp = Url.Content("~/Sertifikat");
            model.Files = list;

            string master = sessionCheck.FindMaster();
            return View("License", master, model);
        }

        // GET: Kalibrering/History
        public ActionResult History()
        {
            CalibrationViews model = new CalibrationViews();
            List<LogEvent> logEvents = myrep.GetAllLogEventWithDiscarded();
            List<Device> devices = myrep.GetAllDevices();
            List<Room> rooms = myrep.GetAllRooms();
            List<Company> companies = myrep.GetAllCompanys();
            List<EventType> eventTypes = myrep.GetAllEventTypes();
            List<Files> files = myrep.GetAllFilesNotDiscarded();

            model.Companys = companies;
            model.Devices = devices;
            model.Rooms = rooms;
            model.EventTypes = eventTypes;
            model.LogEvents = logEvents;
            if (files.Count != 0)
                model.Files = files;

            string master = sessionCheck.FindMaster();
            return View("History", master, model);
        }
        
        // GET: Kalibrering/CalibrationViewDetails
        public ActionResult CalibrationViewDetails(int id)
        {
            CalibrationViews model = this.CalibrationViews(id);

            string master = sessionCheck.FindMaster();
            return View("CalibrationViewDetails", master, model);
        }

        // GET: Kalibrering/Detailsfile
        public ActionResult Detailsfile(int id)
        {
            CalibrationViews model = this.DeleteDetailsFile(id);

            string master = sessionCheck.FindMaster();
            return View("Detailsfile", master, model);
        }
        
        // GET: Kalibrering/Deletefile
        public ActionResult Deletefile(int id)
        {
            CalibrationViews model = this.DeleteDetailsFile(id);

            string master = sessionCheck.FindMaster();
            return View("Deletefile", master, model);
        }
        
        //settes opp et log event og kassere filen
        // POST: Kalibrering/Deletefile
        [HttpPost]
        public ActionResult Deletefile(int? id)
        {
            Files file = myrep.GetFile((int)id);
            file.Kassert = true;
            LogEvent calibration = myrep.GetLogEventByFileId((int)id);
            LogEvent logevent = new LogEvent();
            logevent.CompanyID = calibration.CompanyID;
            logevent.DeviceID = calibration.DeviceID;
            logevent.EventTypeID = 12;
            logevent.FileID = (int)id;
            logevent.RegisteredDate = DateTime.Now;
            logevent.RoomID = calibration.RoomID;

            try
            {
                myrep.EditFile(file);
                myrep.CreateLogEvent(logevent);
                return RedirectToAction("License");
            }
            catch 
            {
                CalibrationViews model = this.DeleteDetailsFile((int)id);
                return View(model);
            }
        }
        #endregion
        
        public CalibrationViews CreateCalibrationView()
        {
            List<Device> deviceList = myrep.GetAllDevices();
            List<Room> roomList = myrep.GetAllRooms();
            List<EventType> eventTypeList = myrep.GetAllEventTypes();
            List<Company> companyList = myrep.GetAllCompanys();
            List<Files> files = myrep.GetAllFilesNotDiscarded();

            CalibrationViews model = new CalibrationViews()
            {
                Devices = deviceList,
                Companys = companyList,
                EventTypes = eventTypeList,
                Rooms = roomList,
                Files = files
            };

            return model;
        }
        
        public CalibrationViews CalibrationViews(int id)
        {
            var logEvent = myrep.GetLogEvent(id);
            var device = myrep.GetDevice(logEvent.DeviceID);
            var room = myrep.GetRoom(logEvent.RoomID);
            var eventType = myrep.GetEventType(logEvent.EventTypeID);
            var Company = myrep.GetCompany(logEvent.CompanyID);
            var file = myrep.GetFile(logEvent.FileID);

            CalibrationViews model = new CalibrationViews();
            model.LogEvent = logEvent;
            model.Room = room;
            model.Device = device;
            model.Company = Company;
            model.EventType = eventType;
            model.FileTo = file;

            return model;
        }

        public LogEvent CreatEditFillLogEvent(CalibrationViews model, HttpPostedFileBase file)
        {
            LogEvent calibration = new LogEvent();
            //Sjekker om dem har skrevet inn feil dato. Feil dato forekommer standar verdien 01.01.0001
            if (model.LogEvent.RegisteredDate.Year == 1)
                return null;
            else
            {
                //Fyller inn i log event som skal inn i db
                calibration.RegisteredDate = model.LogEvent.RegisteredDate;
                calibration.CompanyID = model.Company.CompanyID;
                calibration.DeviceID = model.Device.DeviceID;
                calibration.EventTypeID = model.EventType.EventTypeID;
                calibration.RoomID = model.Room.RoomID;
                if (model.FileTo != null)
                    calibration.FileID = model.FileTo.FileID;
                if (model.LogEvent.StartDate.Year != 1 || model.LogEvent.EndDate.Year != 1)
                {
                    calibration.StartDate = model.LogEvent.StartDate;
                    calibration.EndDate = model.LogEvent.EndDate;
                    if (model.LogEvent.StartDate.Year != 1 && model.LogEvent.EndDate.Year != 1 && model.LogEvent.EndDate < model.LogEvent.StartDate)
                        return null;
                }
                if (model.LogEvent.Data1 != null)
                    calibration.Data1 = model.LogEvent.Data1;
                if (model.LogEvent.Data2 != null)
                    calibration.Data2 = model.LogEvent.Data2;
                try
                {
                    calibration.FileID = this.SaveFileToDirectoryAndDB(file);
                    return calibration;
                }
                catch
                {
                    return null;
                }
            }
        }

        public int SaveFileToDirectoryAndDB(HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (Validator.IsValidFile(file, 5))
                {
                    string path = Path.Combine(Server.MapPath("~/Sertifikat"), file.FileName);
                    file.SaveAs(path);
                    Files dbFile = new Files()
                    {
                        FileName = file.FileName,
                        FilePath = path,
                        FileSize = file.ContentLength,
                        FileType = file.ContentType,
                        Date = DateTime.Now
                    };
                    int fileId = myrep.CreateFile(dbFile);
                    return fileId;
                }
            }
            return 0;
        }

        public CalibrationViews DeleteDetailsFile(int id)
        {
            Files file = myrep.GetFile(id);
            LogEvent logEvent = myrep.GetLogEventByFileId(id);
            CalibrationViews model = new CalibrationViews();
            if (logEvent != null)
            {
                Device device = myrep.GetDevice(logEvent.DeviceID);
                Company company = myrep.GetCompany(logEvent.CompanyID);
                EventType eventType = myrep.GetEventType(logEvent.EventTypeID);
                model.EventType = eventType;
                model.Company = company;
                model.Device = device;
                model.LogEvent = logEvent;
            }
            model.FileTo = file;
            model.ExtraStringHelp = Server.MapPath("~/Sertifikat");

            return model;
        }
    }
}
