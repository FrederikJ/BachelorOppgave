using HovedOppgave.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

//for info om skrive query - http://www.codeproject.com/Articles/43438/Connect-C-to-MySQL

namespace HovedOppgave.Models
{
    public class Repository : IRepository
    {
        MySqlConnection conn;
        HttpContext http = HttpContext.Current;

        public Repository()
        {
            this.Initialize();    
        }

        #region DB klasser
        private void Initialize()
        {
            string myConnectionString = "server=82.164.4.64;UID=boppg;PASSWORD=AXf698LN2VKu8cAn;persistsecurityinfo=True;database=mydb;port=3300;allowuservariables=True";
            conn = new MySqlConnection(myConnectionString);  
        }
        private bool OpenConnection()
        {
            try
            {
                conn.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //To meste vanlige problemene
                switch (ex.Number)
                {
                    case 0:
                        http.Session["flashMessage"] = "Kunne ikke åpne forbindelse";
                        http.Session["flashStatus"] = Constants.NotificationType.danger.ToString();
                        break;
                    case 1045:
                        http.Session["flashMessage"] = "Feil brukernavn/passord, prøv igjen";
                        http.Session["flashStatus"] = Constants.NotificationType.danger.ToString();
                        break;
                }
                return false;
            }
        }
        private bool CloseConnection()
        {
            try
            {
                conn.Close();
                return true;
            }
            catch(MySqlException ex)
            {
                http.Session["flashMessage"] = "Kunne ikke stenge forbindelse - " + ex.Message;
                http.Session["flashStatus"] = Constants.NotificationType.danger.ToString();
                return false;
            }
        }
        #endregion

        #region Building Queries
        #endregion
        #region Cable Queries
        #endregion
        #region CableType Queries
        #endregion
        #region Company Queries
        #endregion
        #region Connection Queries
        #endregion
        #region ConnectorType Queries
        #endregion
        #region ConnectorTypeHasPin Queries
        #endregion
        #region Contact Queries
        public List<Contact> GetAllContacts()
        {
            string query = "select * from contact";
            List<Contact> list = new List<Contact>();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var id = reader["contactID"];
                        var name = reader["name"];
                        var title = reader["title"];
                        var companyId = reader["company_companyID"];

                        var item = new Contact();
                        item.ContactID = Convert.ToInt32(id);
                        item.CompanyID = Convert.ToInt32(companyId);
                        if (name != DBNull.Value)
                            item.Name = (string)name;
                        if (title != DBNull.Value)
                            item.Title = (string)title;

                        list.Add(item);
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return list;
        }
        public Contact GetContact(int contactId)
        {
            string query = "select * from contact where contactID = '" + contactId + "'";
            Contact contact = new Contact();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var id = reader["contactID"];
                        var name = reader["name"];
                        var title = reader["title"];
                        var companyId = reader["company_companyID"];

                        contact.ContactID = Convert.ToInt32(id);
                        contact.CompanyID = Convert.ToInt32(companyId);
                        if (name != DBNull.Value)
                            contact.Name = (string)name;
                        if (title != DBNull.Value)
                            contact.Title = (string)title;
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return contact;
        }
        #endregion
        #region ContactInfo Queries
        #endregion
        #region ContactInfoType Queries
        #endregion
        #region Device Queries
        public List<Device> GetAllDevices()
        {
            string query = "select * from device";
            List<Device> list = new List<Device>();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var id = reader["idDevice"];
                        var name = reader["name"];
                        var roomId = reader["default_location"];
                        var description = reader["description"];
                        var deviceTypeId = reader["type"];
                        var serialNum = reader["serial_num"];
                        var height = reader["height"];
                        var weigth = reader["weight"];
                        var isRackMountable = reader["isRackMountable"];
                        var model = reader["model"];
                        var brand = reader["brand"];
                        var inputVoltage = reader["input_voltage"];

                        var item = new Device();
                        item.DeviceID = Convert.ToInt32(id);
                        item.DeviceTypeID = Convert.ToInt32(deviceTypeId);
                        item.RoomID = Convert.ToInt32(roomId);
                        if (name != DBNull.Value)
                            item.Name = (string)name;
                        if (description != DBNull.Value)
                            item.Description = (string)description;
                        if (serialNum != DBNull.Value)
                            item.SerialNum = (string)serialNum;
                        if (height != DBNull.Value)
                            item.Height = Convert.ToInt32(height);
                        if (weigth != DBNull.Value)
                            item.Weight = Convert.ToInt32(weigth);
                        if (isRackMountable != DBNull.Value)
                            item.IsRackMountable = (bool)isRackMountable;
                        if (model != DBNull.Value)
                            item.Model = (string)model;
                        if (brand != DBNull.Value)
                            item.Brand = (string)brand;
                        if (inputVoltage != DBNull.Value)
                            item.InputVoltage = (decimal)inputVoltage;

                        list.Add(item);
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return list;
        }
        public Device GetDevice(int deviceId)
        {
            string query = "select * from device where idDevice = '" + deviceId + "'";
            Device device = new Device();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var id = reader["idDevice"];
                        var name = reader["name"];
                        var roomId = reader["default_location"];
                        var description = reader["description"];
                        var deviceTypeId = reader["type"];
                        var serialNum = reader["serial_num"];
                        var height = reader["height"];
                        var weigth = reader["weight"];
                        var isRackMountable = reader["isRackMountable"];
                        var model = reader["model"];
                        var brand = reader["brand"];
                        var inputVoltage = reader["input_voltage"];

                        device.DeviceID = Convert.ToInt32(id);
                        device.DeviceTypeID = Convert.ToInt32(deviceTypeId);
                        device.RoomID = Convert.ToInt32(roomId);
                        if (name != DBNull.Value)
                            device.Name = (string)name;
                        if (description != DBNull.Value)
                            device.Description = (string)description;
                        if (serialNum != DBNull.Value)
                            device.SerialNum = (string)serialNum;
                        if (height != DBNull.Value)
                            device.Height = Convert.ToInt32(height);
                        if (weigth != DBNull.Value)
                            device.Weight = Convert.ToInt32(weigth);
                        if (isRackMountable != DBNull.Value)
                            device.IsRackMountable = (bool)isRackMountable;
                        if (model != DBNull.Value)
                            device.Model = (string)model;
                        if (brand != DBNull.Value)
                            device.Brand = (string)brand;
                        if (inputVoltage != DBNull.Value)
                            device.InputVoltage = (decimal)inputVoltage;
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return device;
        }
        #endregion
        #region DeviceConnector Queries
        #endregion
        #region DeviceType Queries
        #endregion
        #region EventType Queries
        public List<EventType> GetAllEventTypes()
        {
            string query = "select * from event_types";
            List<EventType> list = new List<EventType>();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var id = reader["log_type"];
                        var name = reader["log_type_name"];
                        var description = reader["log_type_desc"];

                        var item = new EventType();
                        item.EventTypeID = Convert.ToInt32(id);
                        item.Name = (string)name;
                        if (description != DBNull.Value)
                            item.Description = (string)description;

                        list.Add(item);
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return list;
        }
        public EventType GetEventType(int eventTypeId)
        {
            string query = "select * from event_types where log_type = '" + eventTypeId + "'";
            EventType eventType = new EventType();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var id = reader["log_type"];
                        var name = reader["log_type_name"];
                        var description = reader["log_type_desc"];

                        eventType.EventTypeID = Convert.ToInt32(id);
                        eventType.Name = (string)name;
                        if (description != DBNull.Value)
                            eventType.Description = (string)description;
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return eventType;
        }
        #endregion
        #region LogEvent Queries
        public void CreateKalibreringLogEvent(LogEvent logEvent)
        {
            string query = "Insert into log_event (event_type, device_id, event_registered_date, event_start_date, event_end_date, data_file_path, event_data1, event_data2, event_contact, event_location) values('" + logEvent.EventTypeID + "','" + logEvent.DeviceID + "','" + logEvent.RegisteredDate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + logEvent.StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + logEvent.EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + logEvent.DataFilePath + "','" + logEvent.Data1 + "','" + logEvent.Data2 + "','" + logEvent.ContactID + "','" + logEvent.RoomID + "')";

            if (this.OpenConnection())
            {
                //Lager en kommando med query og forbindelse
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //Execute query
                    cmd.ExecuteReader();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
        }
        public List<LogEvent> GetAllLogEvent()
        {
            string query = "select * from log_event";
            List<LogEvent> list = new List<LogEvent>();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection())
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var id = reader["event_id"];
                        var eventTypeId = reader["event_type"];
                        var deviceId = reader["device_id"];
                        var eventLocationId = reader["event_location"];
                        var eventContactId = reader["event_contact"];
                        var registeredDate = reader["event_registered_date"];
                        var startDate = reader["event_start_date"];
                        var endDate = reader["event_end_date"];
                        var filePath = reader["data_file_path"];
                        var data1 = reader["event_data1"];
                        var data2 = reader["event_data2"];

                        var item = new LogEvent();
                        item.LogEventID = Convert.ToInt32(id);
                        item.EventTypeID = Convert.ToInt32(eventTypeId);
                        item.RoomID = Convert.ToInt32(eventLocationId);
                        item.ContactID = Convert.ToInt32(eventContactId);
                        item.DeviceID = Convert.ToInt32(deviceId);
                        item.RegisteredDate = Convert.ToDateTime(registeredDate);
                        if (startDate != DBNull.Value)
                            item.StartDate = Convert.ToDateTime(startDate);
                        if (endDate != DBNull.Value)
                            item.EndDate = Convert.ToDateTime(endDate);
                        if (filePath != DBNull.Value)
                            item.DataFilePath = (string)filePath;
                        if (data1 != DBNull.Value)
                            item.Data1 = (string)data1;
                        if (data2 != DBNull.Value)
                            item.Data2 = (string)data2;

                        list.Add(item);
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return list;
        }
        public LogEvent GetLogEvent(int logEventId)
        {
            string query = "select * from log_event where event_id = '" + logEventId + "'";
            LogEvent logEvent = new LogEvent();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection())
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var id = reader["event_id"];
                        var eventTypeId = reader["event_type"];
                        var deviceId = reader["device_id"];
                        var eventLocationId = reader["event_location"];
                        var eventContactId = reader["event_contact"];
                        var registeredDate = reader["event_registered_date"];
                        var startDate = reader["event_start_date"];
                        var endDate = reader["event_end_date"];
                        var filePath = reader["data_file_path"];
                        var data1 = reader["event_data1"];
                        var data2 = reader["event_data2"];

                        logEvent.LogEventID = Convert.ToInt32(id);
                        logEvent.EventTypeID = Convert.ToInt32(eventTypeId);
                        logEvent.RoomID = Convert.ToInt32(eventLocationId);
                        logEvent.ContactID = Convert.ToInt32(eventContactId);
                        logEvent.DeviceID = Convert.ToInt32(deviceId);
                        logEvent.RegisteredDate = Convert.ToDateTime(registeredDate);
                        if (startDate != DBNull.Value)
                            logEvent.StartDate = Convert.ToDateTime(startDate);
                        if (endDate != DBNull.Value)
                            logEvent.EndDate = Convert.ToDateTime(endDate);
                        if (filePath != DBNull.Value)
                            logEvent.DataFilePath = (string)filePath;
                        if (data1 != DBNull.Value)
                            logEvent.Data1 = (string)data1;
                        if (data2 != DBNull.Value)
                            logEvent.Data2 = (string)data2;
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return logEvent;
        }
        public bool EditLogEvent(LogEvent logEvent)
        {
            string query = "UPDATE log_event SET event_type='" + logEvent.EventTypeID + "', device_id='" + logEvent.DeviceID + "', event_contact='" + logEvent.ContactID + "', event_location='" + logEvent.RoomID + "', event_registered_date='" + logEvent.RegisteredDate.Date.ToString("yyyy-MM-dd") + "', event_start_date='" + logEvent.StartDate.Date.ToString("yyyy-MM-dd") + "', event_end_date='" + logEvent.EndDate.Date.ToString("yyyy-MM-dd") + "', data_file_path='" + logEvent.DataFilePath + "', event_data1='" + logEvent.Data1 + "', event_data2='" + logEvent.Data2 + "' WHERE event_id='" + logEvent.LogEventID + "'";

            //Open connection
            if (this.OpenConnection())
            {
                //create mysql command with cmdtext and connection
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //Execute query
                    cmd.ExecuteNonQuery();
                    //close connection
                    this.CloseConnection();
                    return true;
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                    
                }
            }
            return false;
        }
        #endregion
        #region NetworkInfo Queries
        #endregion
        #region Pin Queries
        #endregion
        #region PostCode Queries
        #endregion
        #region Rights Queries
        public Rights GetRightToUser(int userID)
        {
            string query = "select * from rights where id =" + userID;
            Rights right = new Rights();

            if (this.OpenConnection())
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var id = reader["rightsid"];
                        var name = reader["name"];

                        right.RightsID = (int)id;
                        if (name != DBNull.Value)
                            right.Name = (string)name;
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return right;
        }
        public List<Rights> GetAllRights()
        {
            string query = "select * from rights";
            List<Rights> list = new List<Rights>();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection())
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var id = reader["rightsid"];
                        var name = reader["name"];

                        var item = new Rights();
                        item.RightsID = (int)id;
                        if (name != DBNull.Value)
                            item.Name = (string)name;

                        list.Add(item);
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return list;
        }
        #endregion
        #region Room Queries
        public List<Room> GetAllRooms()
        {
            string query = "select * from room";
            List<Room> list = new List<Room>();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection())
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var id = reader["id"];
                        var name = reader["name"];
                        var description = reader["description"];
                        var buildingId = reader["building_id"];

                        var item = new Room();
                        item.RoomID = Convert.ToInt32(id);
                        item.BuildingID = Convert.ToInt32(buildingId);
                        if (name != DBNull.Value)
                            item.Name = (string)name;
                        if (description != DBNull.Value)
                            item.Description = (string)description;

                        list.Add(item);
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return list;
        }
        public Room GetRoom(int roomId)
        {
            string query = "select * from room where id = '" + roomId + "'";
            Room room = new Room();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection())
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var id = reader["id"];
                        var name = reader["name"];
                        var description = reader["description"];
                        var buildingId = reader["building_id"];

                        room.RoomID = Convert.ToInt32(id);
                        room.BuildingID = Convert.ToInt32(buildingId);
                        if (name != DBNull.Value)
                            room.Name = (string)name;
                        if (description != DBNull.Value)
                            room.Description = (string)description;
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return room;
        }
        #endregion
        #region SignalStandard Queries
        #endregion
        #region User Queries
        public User GetUserWithRights(int userID, Constants.Rights rights)
        {
            
            return null;
        }
        public User GetUser(int userID)
        {
            string query = "select * from user where id =" + userID;
            User user = new User();

            if (this.OpenConnection())
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var id = reader["iduser"];
                        var name = reader["name"];
                        var email = reader["email"];
                        var passhash = reader["passhash"];
                        var passsalt = reader["passsalt"];
                        var rightsId = reader["rightsid"];

                        user.UserId = (int)id;
                        user.RightsID = (int)rightsId;
                        if (name != DBNull.Value)
                            user.Name = (string)name;
                        if (email != DBNull.Value)
                            user.Email = (string)email;
                        if (passhash != DBNull.Value)
                            user.PassHash = (string)passhash;
                        if (passsalt != DBNull.Value)
                            user.PassSalt = (string)passsalt;
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return user;
        }
        public void CreateUser(User user)
        {
            string query = "Insert into user (name, email, passhash, passsalt, rightsid) values('" + user.Name.ToLower() + "','" + user.Email.ToLower() + "','" + user.PassHash + "','" + user.PassSalt + "','" + user.RightsID + "')";
            
            if(this.OpenConnection())
            {
                //Lager en kommando med query og forbindelse
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //utfører kommandoen
                    cmd.ExecuteReader();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
        }
        #endregion
    }
}