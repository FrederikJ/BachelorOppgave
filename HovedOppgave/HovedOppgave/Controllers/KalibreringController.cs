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

        /**
         * hvem som kan få adgang til denne siden
        */
        public KalibreringController()
        {
            //SessionCheck.CheckForRightsOnLogInUser(Constant.Rights.Administrator);
            //SessionCheck.CheckForRightsOnLogInUser(Constant.Rights.User);
        }

        /**
         * oversikt over alle event som skal skje, planlagt i framtien, dem som skjer og de som har skjedd
         * samt en oversikt over alle enheter som kan kalibreres
        */
        // GET: Kalibrering
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

        /**
         * oppretter en log event her, kan få inn en enhet id som gjør at enheten og envent typen blir automatisk fylt inn
        */
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

        /**
         * får inn modellen og evt en opplastet sertifikat til log eventet
        */
        // POST: Kalibrering/Create
        [HttpPost]
        public ActionResult Create(CalibrationViews model, HttpPostedFileBase file)
        {
            //fyller inn log eventet med det som trengs og sjekker at den ikke er null
            LogEvent calibration = CreatEditFillLogEvent(model, file);

            if (calibration == null)
                return View(model);

            try
            {
                //oppdaterer og sender deg videre
                myrep.CreateLogEvent(calibration);
                return RedirectToAction("Overview");
            }
            catch
            {
                return View(model);
            }
        }

        /**
         * endrer logeventet, får inn en log event id, fyller inn modellen av det som trengs
         * og sender deg til viewet ferdig utfylt
        */
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

        /**
         * oppdatere db med de nye endringene til log eventet, får inn modelen og evt. en 
         * opplastet fil
        */
        // POST: Kalibrering/Edit/5
        [HttpPost]
        public ActionResult EditCalibration(CalibrationViews model, HttpPostedFileBase file)
        {
            //fyller inn log eventet med det som trengs og sjekker at den ikke er null
            LogEvent calibration = CreatEditFillLogEvent(model, file);

            if (calibration != null)
                return View(model);

            //sjekker om den filen som var i log eventet er det samme som den som er nu, viss den
            //ikke er det så kasserer man den gamle filen
            if (calibration.FileID  != 0 && calibration.FileID != model.LogEvent.FileID)
            {
                var tempFile = myrep.GetFile(model.LogEvent.FileID);
                tempFile.Kassert = true;
                myrep.EditFile(tempFile);
            }

            try
            {
                //oppdaterer db og sender deg videre
                myrep.EditLogEvent(calibration);
                return RedirectToAction("Overview");
            }
            catch
            {
                return View(model);
            }
        }

        /**
         * her importere man sertifikat fra directory også kan man evt ta og sette det til
         * siste log event til en bestemt enhet
        */
        // GET: Kalibrering/Import
        public ActionResult Import(int id)
        {
            CalibrationViews model = new CalibrationViews();
            //Hard kodet inn id for kalibrering
            List<LogEvent> logEvents = myrep.GetAllLogEventToEventType(11);
            //sortere listen
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
            //Device id, viss man allerede har valgt enheten, så blir alt fylt inn med en gang
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

        /**
         * post medtoden for importering, oppdaterer db, får inn modellen og en fil fra 
         * directory
        */
        // POST: Kalibrering/Import
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file, CalibrationViews model)
        {
            //lagrer filen til db og directory
            int id = this.SaveFileToDirectoryAndDB(file);

            try
            {
                //viss man har valgt en enhet til sertifikatet
                if (model.LogEvent.LogEventID != 0)
                {
                    //henter siste kalibrering til enheten
                    LogEvent logEvent = myrep.GetLogEvent(model.LogEvent.LogEventID);
                    if (logEvent.FileID != 0)
                    {
                        //kasserer forrige fil som ligger
                        Files tempFile = myrep.GetFile(logEvent.FileID);
                        tempFile.Kassert = true;
                        myrep.EditFile(tempFile);
                    }
                    //legger til ny
                    logEvent.FileID = id;

                    //oppdaterer db og sender deg videre
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

        /**
         * lister ut alle filene som er i systemet. 
        */
        // GET: Kalibrering/License
        public ActionResult License()
        {
            List<Files> list = myrep.GetAllFilesNotDiscarded();
            CalibrationViews model = new CalibrationViews();
            //for å kunne vise filene i fancybox. 
            model.ExtraStringHelp = Url.Content("~/Sertifikat");
            model.Files = list;

            string master = sessionCheck.FindMaster();
            return View("License", master, model);
        }

        /**
         * skriver ut all istorie som har skjedd i hele systemet 
        */
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

        /**
         * detalje om et log event 
        */
        // GET: Kalibrering/CalibrationViewDetails
        public ActionResult CalibrationViewDetails(int id)
        {
            CalibrationViews model = this.CalibrationViews(id);

            string master = sessionCheck.FindMaster();
            return View("CalibrationViewDetails", master, model);
        }

        /**
         * detaljer om en fil
        */
        // GET: Kalibrering/Detailsfile
        public ActionResult Detailsfile(int id)
        {
            CalibrationViews model = this.DeleteDetailsFile(id);

            string master = sessionCheck.FindMaster();
            return View("Detailsfile", master, model);
        }

        /**
         * får detaljer om en fil og spør deg igjen om du vil slette filen
        */
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
                //setter filen til kassert
                myrep.EditFile(file);
                //oppretter et logevent
                myrep.CreateLogEvent(logevent);
                //redirekte deg til oversikt over alle filer
                return RedirectToAction("License");
            }
            catch 
            {
                CalibrationViews model = this.DeleteDetailsFile((int)id);
                return View(model);
            }
        }

        /**
         * brukes av en get metode for å fylle inn i viewet 
        */
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

        /**
         * brukes av en get metode for å fylle inn i modellen
        */
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

        /**
         * brukes av post metode for å fylle inn log eventet som skal settes inn i db
         * får inn modellen fra viewet og evt. en opplastet fil fra directory
        */
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
                //sjekker først om start date og end date er satt inn default verdi på .year er 1
                if (model.LogEvent.StartDate.Year != 1 || model.LogEvent.EndDate.Year != 1)
                {
                    calibration.StartDate = model.LogEvent.StartDate;
                    calibration.EndDate = model.LogEvent.EndDate;
                    //viss start date er større enn enddate, så retuneres det null og du går tilbake til viewet
                    if (model.LogEvent.StartDate.Year != 1 && model.LogEvent.EndDate.Year != 1 && model.LogEvent.EndDate < model.LogEvent.StartDate)
                        return null;
                }
                if (model.LogEvent.Data1 != null)
                    calibration.Data1 = model.LogEvent.Data1;
                if (model.LogEvent.Data2 != null)
                    calibration.Data2 = model.LogEvent.Data2;
                try
                {
                    //lagrer filen i db og directory og retunerer fil id som blir satt inn i log eventet
                    calibration.FileID = this.SaveFileToDirectoryAndDB(file);
                    return calibration;
                }
                catch
                {
                    return null;
                }
            }
        }

        /**
         * lagrer filen i directory og db 
        */
        public int SaveFileToDirectoryAndDB(HttpPostedFileBase file)
        {
            if (file != null)
            {
                //gjør en sjekk på filen
                if (Validator.IsValidFile(file, 5))
                {
                    //vilken mappe og navn hvor filen skal lagres
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
                    //lagrer filen i db og retunere iden
                    int fileId = myrep.CreateFile(dbFile);
                    return fileId;
                }
            }
            return 0;
        }

        /**
         * brukes av get metode hvor man fyller inn modellen for 2 like views 
        */
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
