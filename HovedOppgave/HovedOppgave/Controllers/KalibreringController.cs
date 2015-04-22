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
                    var file = myrep.GetFile(item.FileID);

                    CalibrationViews model = new CalibrationViews();
                    model.Contact = contact;
                    model.Device = device;
                    model.EventType = eventType;
                    model.LogEvent = item;
                    model.Room = room;
                    model.File = file;

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

        [HttpPost]
        public JsonResult GetFile(Files item)
        {
            return Json(item);
        }

        // POST: Kalibrering/Create
        [HttpPost]
        public ActionResult Create(CreateCalibrationViewModel model,
            IEnumerable<int> SelectedDevice, IEnumerable<int> SelectedEventType,
            IEnumerable<int> SelectedRoom, IEnumerable<int> SelectedContact, 
            IEnumerable<int> SelectedFile, HttpPostedFileBase file)
        {
            LogEvent calibration = new LogEvent();
            Files test = model.File;
            //Sjekker om dem har skrevet inn feil dato. Feil dato forekommer standar verdien 01.01.0001
            if (model.LogEvent.RegisteredDate.Year == 1)
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
                    calibration.ContactID = SelectedContact.FirstOrDefault();
                    calibration.DeviceID = SelectedDevice.FirstOrDefault();
                    calibration.EventTypeID = SelectedEventType.FirstOrDefault();
                    calibration.RoomID = SelectedRoom.FirstOrDefault();
                    if (SelectedFile.FirstOrDefault() != 0)
                        calibration.FileID = SelectedFile.FirstOrDefault();
                    if (model.LogEvent.StartDate.Year == 1)
                        calibration.StartDate = model.LogEvent.StartDate;
                    if (model.LogEvent.EndDate.Year == 1)
                        calibration.EndDate = model.LogEvent.EndDate;
                    if (model.LogEvent.Data1 != null)
                        calibration.Data1 = model.LogEvent.Data1;
                    if (model.LogEvent.Data2 != null)
                        calibration.Data2 = model.LogEvent.Data2;

                    try
                    {
                        if(file != null)
                            if (Validator.IsValidFile(file, 5))
                            {
                                string path = Path.Combine(Server.MapPath("~/App_Data/Sertifikat"), file.FileName);
                                file.SaveAs(path);
                                Files dbFile = new Files()
                                {
                                    FileName = file.FileName,
                                    FilePath = path,
                                    FileSize = file.ContentLength,
                                    FileType = file.ContentType,
                                    Date = DateTime.Now
                                };

                                if (myrep.CreateFile(dbFile))
                                {
                                    var files = myrep.GetAllFiles();
                                    files.GroupBy(x => x.Date);
                                    files.Reverse();

                                    var temp = new Files();
                                    for (int i = 0; i < files.Count; i++)
                                        temp = files[0];

                                    calibration.FileID = temp.FileID;
                                    
                                }
                            }

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
            IEnumerable<int> SelectedRoom, IEnumerable<int> SelectedFile, 
            IEnumerable<int> SelectedContact, HttpPostedFileBase file)
        {
            LogEvent calibration = new LogEvent();
            CreateCalibrationViewModel newmodel;
            LogEvent logEvent;

            //Sjekker om dem har skrevet inn feil dato. Feil dato forekommer standar verdien 01.01.0001
            if (model.LogEvent.RegisteredDate.Year == 1)
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
                    if (SelectedFile.FirstOrDefault() != 0)
                        calibration.FileID = SelectedFile.FirstOrDefault();
                    if (model.LogEvent.StartDate.Year == 1)
                        calibration.StartDate = model.LogEvent.StartDate;
                    if (model.LogEvent.EndDate.Year == 1)
                        calibration.EndDate = model.LogEvent.EndDate;
                    if (model.LogEvent.Data1 != null)
                        calibration.Data1 = model.LogEvent.Data1;
                    if (model.LogEvent.Data2 != null)
                        calibration.Data2 = model.LogEvent.Data2;
                    
                    try
                    {
                        if(file != null)
                            if (Validator.IsValidFile(file, 5))
                            {
                                string path = Path.Combine(Server.MapPath("~/App_Data/Sertifikat"), file.FileName);
                                file.SaveAs(path);
                                Files dbFile = new Files()
                                {
                                    FileName = file.FileName,
                                    FilePath = path,
                                    FileSize = file.ContentLength,
                                    FileType = file.ContentType,
                                    Date = DateTime.Now
                                };

                                if (myrep.CreateFile(dbFile))
                                {
                                    var files = myrep.GetAllFiles();
                                    files.GroupBy(x => x.Date);
                                    files.Reverse();

                                    var temp = new Files();
                                    for (int i = 0; i < files.Count; i++)
                                        temp = files[0];

                                    calibration.FileID = temp.FileID;

                                }
                            }
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
            List<SelectListItem> devices = new List<SelectListItem>();
            List<LogEvent> logEvents = myrep.GetAllLogEvent();
            SelectListItem selectlist = new SelectListItem()
            {
                Text = "",
                Value = "0"
            };
            devices.Add(selectlist);
            foreach (Device item in myrep.GetAllDevices())
                for(var i = 0; i < logEvents.Count; i++)
                    if(logEvents[i].DeviceID.Equals(item.DeviceID) && logEvents[i].EndDate >= DateTime.Now)
                    {
                        selectlist = new SelectListItem()
                        {
                            Text = item.Name,
                            Value = item.DeviceID.ToString()
                        };
                        devices.Add(selectlist);
                    }

            ImportFiles model = new ImportFiles()
            {
                Device = devices
            };
            return View(model);
        }

        // POST: Kalibrering/Import
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file, IEnumerable<int> SelectedDevice)
        {
            string path = Path.Combine(Server.MapPath("~/App_Data/Sertifikat"), file.FileName);
            
            try
            {
                if (Validator.IsValidFile(file, 5))
                {
                    file.SaveAs(path);
                    Files dbFile = new Files()
                    {
                        FileName = file.FileName,
                        FilePath = path,
                        FileSize = file.ContentLength,
                        FileType = file.ContentType,
                        Date = DateTime.Now
                    };
                    if (SelectedDevice.FirstOrDefault() != 0)
                    {
                        LogEvent logEvent = myrep.GetLastEventForDevice(SelectedDevice.FirstOrDefault());
                        if (logEvent.FileID > 0)
                        {
                            List<Files> files = myrep.GetAllFiles();
                            Files tempFile = myrep.GetFile(logEvent.FileID);
                            var tempList = new List<Files>();
                            for (int i = 0; i < files.Count; i++)
                                if (files[i].FileName.Equals(tempFile.FileName))
                                    tempList.Add(files[i]);

                            if (tempList.Count == 1)
                            {
                                DirectoryInfo myDir = new DirectoryInfo(Server.MapPath("~/App_Data/Sertifikat"));
                                foreach (FileInfo fil in myDir.GetFiles())
                                    if (fil.Name.Equals(tempFile.FileName))
                                        fil.Delete();
                            }
                            myrep.DeleteFile(tempFile);
                        }

                        if (myrep.CreateFile(dbFile))
                        {
                            var files = myrep.GetAllFiles();
                            files.GroupBy(x => x.Date);
                            files.Reverse();

                            var temp = new Files();
                            for (int i = 0; i < files.Count; i++)
                                temp = files[0];

                            logEvent.FileID = temp.FileID;

                            if (myrep.EditLogEvent(logEvent))
                                return RedirectToAction("License");
                        }
                    }
                    else
                        if (myrep.CreateFile(dbFile))
                            return RedirectToAction("License");
                }
                return RedirectToAction("Import");
            }
            catch
            {
                return RedirectToAction("Import");
            }
        }

        // GET: Kalibrering/License
        public ActionResult License()
        {
            //string[] pathOfFiles = Directory.GetFiles(@"C:\Users\Frederik\AppData\Sertifikat", "*.pdf");
            List<Files> list = myrep.GetAllFiles();
            List<CalibrationViews> model = new List<CalibrationViews>();
            
            for (int i = 0; i < list.Count; i++)
            {
                var temp = new CalibrationViews();
                temp.File = list[i];
                if(i == 0)
                    temp.FilePath = Server.MapPath("~/App_Data/Sertifikat");
                model.Add(temp);
            }
            return View(model);
        }

        // GET: Kalibrering/History
        public ActionResult History()
        {
            List<CalibrationViews> modelList = new List<CalibrationViews>();
            List<LogEvent> logEventList = myrep.GetAllLogEvent();
            CalibrationViews model = new CalibrationViews();

            foreach (var item in logEventList)
            {
                if (item.EndDate.Date <= DateTime.Now.Date)
                {
                    var device = myrep.GetDevice(item.DeviceID);
                    var room = myrep.GetRoom(item.RoomID);
                    var eventType = myrep.GetEventType(item.EventTypeID);
                    var contact = myrep.GetContact(item.ContactID);
                    if (item.FileID != 0)
                    {
                        var file = myrep.GetFile(item.FileID);
                        model.File = file;
                    }
                    
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

        // GET: Kalibrering/CalibrationViewDetails
        public ActionResult CalibrationViewDetails(int id)
        {
            CalibrationViews model = this.CalibrationViews(id);
            return View(model);
        }

        // GET: Kalibrering/Showfile
        public ActionResult Showfile(int id)
        {
            Files file = myrep.GetFile(id);
            string path = Path.Combine(Server.MapPath("~/App_Data/Sertifikat"), file.FileName);
            file.FilePath = path;

            return View(file);
        }

        // GET: Kalibrering/Detailsfile
        public ActionResult Detailsfile(int id)
        {
            Files file = myrep.GetFile(id);
            LogEvent logEvent = myrep.GetLogEventByFileId(id);
            CalibrationViews model = new CalibrationViews();
            if(logEvent != null)
            {
                Device device = myrep.GetDevice(logEvent.DeviceID);
                model.Device = device;
                model.LogEvent = logEvent;
            }
            model.File = file;
            model.FilePath = Server.MapPath("~/App_Data/Sertifikat");
            return View(model);
        }

        // GET: Kalibrering/Deletefile
        public ActionResult Deletefile(int id)
        {
            Files file = myrep.GetFile(id);
            LogEvent logEvent = myrep.GetLogEventByFileId(id);
            CalibrationViews model = new CalibrationViews();
            if (logEvent != null)
            {
                Device device = myrep.GetDevice(logEvent.DeviceID);
                model.Device = device;
                model.LogEvent = logEvent;
            }
            model.File = file;
            model.FilePath = Server.MapPath("~/App_Data/Sertifikat");
            return View(model);
        }

        // POST: Kalibrering/Deletefile
        [HttpPost]
        public ActionResult Deletefile(int? id)
        {
            Files file = myrep.GetFile((int)id);

            //Går igjennom alle logevents vi har, sjekker om det er flere med fil id
            List<Files> files = myrep.GetAllFiles();
            var tempList = new List<Files>();
            for (int i = 0; i < files.Count; i++)
                if (files[i].FileName.Equals(file.FileName))
                    tempList.Add(files[i]);

            try
            {
                //count vil være 1 vis det ikke er flere med samme id så man kan slette filen
                // fra directory. for når man lagre filen, så lagre den ikke om det er en fil med samme navnet fra før av.
                if(tempList.Count == 1)
                {
                    DirectoryInfo myDir = new DirectoryInfo(Server.MapPath("~/App_Data/Sertifikat"));
                    foreach(FileInfo fil in myDir.GetFiles())
                        if (fil.Name.Equals(file.FileName))
                            fil.Delete();

                }
                if (myrep.DeleteFile(file))
                    return RedirectToAction("License");
            }
            catch { }
            LogEvent logEvent = myrep.GetLogEventByFileId((int)id);
            Device device = myrep.GetDevice(logEvent.DeviceID);
            CalibrationViews model = new CalibrationViews();
            model.File = file;
            model.Device = device;
            model.LogEvent = logEvent;
            model.FilePath = Server.MapPath("~/App_Data/Sertifikat");
            return View(model);
        }

        #region Create Calibration view method
        public CreateCalibrationViewModel CreateCalibrationView()
        {
            List<SelectListItem> deviceList = new List<SelectListItem>();
            List<SelectListItem> roomList = new List<SelectListItem>();
            List<SelectListItem> eventTypeList = new List<SelectListItem>();
            List<SelectListItem> contactList = new List<SelectListItem>();
            List<Files> files = myrep.GetAllFiles();

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
                Room = roomList,
                Files = files
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
            var file = myrep.GetFile(logEvent.FileID);

            CalibrationViews model = new CalibrationViews();
            model.LogEvent = logEvent;
            model.Room = room;
            model.Device = device;
            model.Contact = contact;
            model.EventType = eventType;
            model.File = file;

            return model;
        }
        #endregion
    }
}
