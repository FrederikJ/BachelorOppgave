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
            string query = "select * from contact*";
            List<Contact> list = new List<Contact>();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                //lage en data reader og utfører kommandoen
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var id = reader["contactID"];
                    var firstName = reader["first_name"];
                    var surName = reader["last_name"];
                    var title = reader["title"];
                    var companyId = reader["company_companyID"];

                    var item = new Contact();
                    item.ContactID = (int)id;
                    item.FirstName = (string)firstName;
                    item.LastName = (string)surName;
                    item.Title = (string)title;
                    item.CompanyID = (int)companyId;

                    list.Add(item);
                }
                reader.Close();

                this.CloseConnection();
            }

            return list;
        }
        #endregion
        #region ContactInfo Queries
        #endregion
        #region ContactInfoType Queries
        #endregion
        #region Device Queries
        public List<Device> GetAllDevices()
        {
            string query = "select * from device*";
            List<Device> list = new List<Device>();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
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
                    var weigth = reader["weigth"];
                    var isRackMountable = reader["isRackMountable"];
                    var model = reader["model"];
                    var brand = reader["brand"];
                    var inputVoltage = reader["input_voltage"];

                    var item = new Device();
                    item.DeviceID = (int)id;
                    item.Name = (string)name;
                    item.RoomID = (int)roomId;
                    item.Description = (string)description;
                    item.DeviceTypeID = (int)deviceTypeId;
                    item.SerialNum = (string)serialNum;
                    item.Height = (int)height;
                    item.Weight = (int)weigth;
                    item.IsRackMountable = (bool)isRackMountable;
                    item.Model = (string)model;
                    item.Brand = (string)brand;
                    item.InputVoltage = (decimal)inputVoltage;

                    list.Add(item);
                }
                reader.Close();

                this.CloseConnection();
            }

            return list;
        }
        #endregion
        #region DeviceConnector Queries
        #endregion
        #region DeviceType Queries
        #endregion
        #region EventType Queries
        public List<EventType> GetAllEventTypes()
        {
            string query = "select * from event_types*";
            List<EventType> list = new List<EventType>();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                //lage en data reader og utfører kommandoen
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var id = reader["log_type"];
                    var name = reader["log_type_name"];
                    var description = reader["log_type_desc"];

                    var item = new EventType();
                    item.Description = (string)description;
                    item.EventTypeID = (int)id;
                    item.Name = (string)name;
                    
                    list.Add(item);
                }
                reader.Close();

                this.CloseConnection();
            }

            return list;
        }
        #endregion
        #region LogEvent Queries
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

            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                //lage en data reader og utfører kommandoen
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var id = reader["rightsid"];
                    var name = reader["name"];

                    right.RightsID = (int)id;
                    right.Name = (string)name;
                }
                reader.Close();

                this.CloseConnection();
            }
            return right;
        }

        public List<Rights> GetAllRights()
        {
            string query = "select * from rights";
            List<Rights> list = new List<Rights>();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                //lage en data reader og utfører kommandoen
                MySqlDataReader reader = cmd.ExecuteReader();
                
                while(reader.Read())
                {
                    var id = reader["rightsid"];
                    var name = reader["name"];
                    
                    var item = new Rights();
                    item.RightsID = (int)id;
                    item.Name = (string)name;

                    list.Add(item);
                }
                reader.Close();

                this.CloseConnection();
            }

            return list;
        }
        #endregion
        #region Room Queries
        public List<Room> GetAllRooms()
        {
            string query = "select * from room*";
            List<Room> list = new List<Room>();

            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                //lage en data reader og utfører kommandoen
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var id = reader["id"];
                    var name = reader["name"];
                    var description = reader["description"];
                    var buildingId = reader["building_id"];

                    var item = new Room();
                    item.RoomID = (int)id;
                    item.Name = (string)name;
                    item.Description = (string)description;
                    item.BuildingID = (int)buildingId;

                    list.Add(item);
                }
                reader.Close();

                this.CloseConnection();
            }

            return list;
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

            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
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
                    user.Name = (string)name;
                    user.Email = (string)email;
                    user.PassHash = (string)passhash;
                    user.PassSalt = (string)passsalt;
                    user.RightsID = (int)rightsId;
                }
                reader.Close();

                this.CloseConnection();
            }
            return user;
        }
        public void CreateUser(User user)
        {
            string query = "Insert into user (name, email, passhash, passsalt, rightsid) values('" + user.Name + "','" + user.Email + "','" + user.PassHash + "','" + user.PassSalt + "','" + user.RightsID + "')";
            
            if(this.OpenConnection())
            {
                //Lager en kommando med query og forbindelse
                MySqlCommand cmd = new MySqlCommand(query, conn);
                //utfører kommandoen
                cmd.ExecuteReader();
                //stenger forbindelsen
                this.CloseConnection();
            }
        }
        #endregion
    }
}