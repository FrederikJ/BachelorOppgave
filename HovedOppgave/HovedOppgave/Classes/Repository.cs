using HovedOppgave.Classes;
using HovedOppgave.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;

//for info om skrive query - http://www.codeproject.com/Articles/43438/Connect-C-to-MySQL

namespace HovedOppgave.Models
{
    public class Repository : IRepository
    {
        MySqlConnection conn;
        HttpContext http = HttpContext.Current;

        /**
         * Starter initialiseringen til db
        */
        public Repository()
        {
            this.Initialize();    
        }

        #region DB klasser
        /**
         * Initialiser db kontakt, setter opp kontakt stringen
        */
        private void Initialize()
        {
            string myConnectionString = "server=82.164.4.64;UID=boppg;PASSWORD=AXf698LN2VKu8cAn;persistsecurityinfo=True;database=mydb;port=3300;allowuservariables=True";
            conn = new MySqlConnection(myConnectionString);  
        }
        /**
         * Opner den allerede eksisterende kontakten til db 
        */
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
                        http.Session["flashStatus"] = Constant.NotificationType.danger.ToString();
                        break;
                    case 1045:
                        http.Session["flashMessage"] = "Feil brukernavn/passord, prøv igjen";
                        http.Session["flashStatus"] = Constant.NotificationType.danger.ToString();
                        break;
                }
                return false;
            }
        }
        /**
         * stenger den allerede eksisterende kontakten til db 
        */
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
                http.Session["flashStatus"] = Constant.NotificationType.danger.ToString();
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
        //henter alle firmaer fra db
        public List<Company> GetAllCompanys()
        {
            string query = "select * from company";
            List<Company> list = new List<Company>();

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
                        var item = new Company();
                        item.CompanyID = Convert.ToInt32(reader["companyID"]);
                        if (reader["org_num"] != DBNull.Value)
                            item.OrganisationNum = Convert.ToInt32(reader["org_num"]);
                        if (reader["companyname"] != DBNull.Value)
                            item.Name = (string)reader["companyname"];

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
        //henter et firma med en firma id
        public Company GetCompany(int companyId)
        {
            string query = "select * from company where companyID = '" + companyId + "'";
            Company company = new Company();

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
                        company.CompanyID = Convert.ToInt32(reader["companyID"]);
                        if (reader["companyname"] != DBNull.Value)
                            company.Name = (string)reader["companyname"];
                        if (reader["org_num"] != DBNull.Value)
                            company.OrganisationNum = Convert.ToInt32(reader["org_num"]);
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
            return company;
        }
        //henter et firma med kontakt personen til firmaet
        public CompanyWithContact GetCompanyWithContactInfo(int companyId)
        {
            string query = "select * from company cm, contact cn, contact_info ci, contact_info_type cit where cm.companyID = '" + companyId + "' and cn.company_companyID = cm.companyID and cn.contactID = ci.contactID and ci.type = cit.type";
            CompanyWithContact model = new CompanyWithContact();
            Company cm = new Company();
            Contact cn = new Contact();
            ContactInfo ci = new ContactInfo();
            ContactInfoType cit = new ContactInfoType();

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
                        cm.CompanyID = Convert.ToInt32(reader["companyID"]);
                        if (reader["companyname"] != DBNull.Value)
                            cm.Name = (string)reader["companyname"];
                        if (reader["org_num"] != DBNull.Value)
                            cm.OrganisationNum = Convert.ToInt32(reader["org_num"]);

                        cn.ContactID = Convert.ToInt32(reader["contactID"]);
                        cn.CompanyID = Convert.ToInt32(reader["company_companyID"]);
                        if (reader["name"] != DBNull.Value)
                            cn.Name = (string)reader["name"];
                        if (reader["title"] != DBNull.Value)
                            cn.Title = (string)reader["title"];

                        ci.ContactInfoID = Convert.ToInt32(reader["contact_infoID"]);
                        ci.Value = (string)reader["value"];

                        cit.ContactInfoTypeID = Convert.ToInt32(reader["type"]);
                        cit.TypeName = (string)reader["type_name"];

                        model.Company = cm;
                        model.Contact = cn;
                        model.ContactInfo = ci;
                        model.ContactInfoType = cit;
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
            return model;
        }
        //oppdaterer firma i db
        public bool EditCompany(Company company)
        {
            string query = "UPDATE company SET companyname='" + company.Name + "', org_num='" + company.OrganisationNum + "' where companyID='" + company.CompanyID + "'";

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
        #region Connection Queries
        #endregion
        #region ConnectorType Queries
        #endregion
        #region ConnectorTypeHasPin Queries
        #endregion
        #region Contact Queries
        //henter alle kontakt personene til hvert firma
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
                        var item = new Contact();
                        item.ContactID = Convert.ToInt32(reader["contactID"]);
                        item.CompanyID = Convert.ToInt32(reader["company_companyID"]);
                        if (reader["name"] != DBNull.Value)
                            item.Name = (string)reader["name"];
                        if (reader["title"] != DBNull.Value)
                            item.Title = (string)reader["title"];

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
        //henter en kotakt person med en kontakt person
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
                        contact.ContactID = Convert.ToInt32(reader["contactID"]);
                        contact.CompanyID = Convert.ToInt32(reader["company_companyID"]);
                        if (reader["name"] != DBNull.Value)
                            contact.Name = (string)reader["name"];
                        if (reader["title"] != DBNull.Value)
                            contact.Title = (string)reader["title"];
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
        //sletter kotakt personen fra db
        public bool DeleteContact(Contact contact)
        {
            string query = "DELETE FROM contact WHERE contactID='" + contact.ContactID + "'";

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
                return true;
            }
            this.CloseConnection();
            return false;
        }
        //oppdaterer kontakt person i db
        public bool EditContact(Contact contact)
        {
            string query = "UPDATE contact SET name='" + contact.Name + "', title='" + contact.Title + "' where contactID='" + contact.ContactID + "'";

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
        #region ContactInfo Queries
        public bool EditContactInfo(ContactInfo contactInfo)
        {
            string query = "UPDATE contact_info SET value='" + contactInfo.Value + "' where contact_infoID='" + contactInfo.ContactInfoID + "'";

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
        #region ContactInfoType Queries
        public bool EditContactInfoType(ContactInfoType contactInfoType)
        {
            string query = "UPDATE contact_info_type SET type_name='" +contactInfoType.TypeName + "' where type='" + contactInfoType.ContactInfoTypeID + "'";

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
        #region Device Queries
        //henter alle enheter fra db
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
                        var item = new Device();
                        item.DeviceID = Convert.ToInt32(reader["idDevice"]);
                        item.DeviceTypeID = Convert.ToInt32(reader["type"]);
                        item.RoomID = Convert.ToInt32(reader["default_location"]);
                        if (reader["devicename"] != DBNull.Value)
                            item.Name = (string)reader["name"];
                        if (reader["description"] != DBNull.Value)
                            item.Description = (string)reader["description"];
                        if (reader["serial_num"] != DBNull.Value)
                            item.SerialNum = (string)reader["serial_num"];
                        if (reader["height"] != DBNull.Value)
                            item.Height = Convert.ToInt32(reader["height"]);
                        if (reader["weight"] != DBNull.Value)
                            item.Weight = Convert.ToInt32(reader["weight"]);
                        if (reader["isRackMountable"] != DBNull.Value)
                            item.IsRackMountable = (bool)reader["isRackMountable"];
                        if (reader["model"] != DBNull.Value)
                            item.Model = (string)reader["model"];
                        if (reader["brand"] != DBNull.Value)
                            item.Brand = (string)reader["brand"];
                        if (reader["input_voltage"] != DBNull.Value)
                            item.InputVoltage = (decimal)reader["input_voltage"];

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
        //henter en enhet med en enhets id
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
                        device.DeviceID = Convert.ToInt32(reader["idDevice"]);
                        device.DeviceTypeID = Convert.ToInt32(reader["type"]);
                        device.RoomID = Convert.ToInt32(reader["default_location"]);
                        if (reader["devicename"] != DBNull.Value)
                            device.Name = (string)reader["devicename"];
                        if (reader["description"] != DBNull.Value)
                            device.Description = (string)reader["description"];
                        if (reader["serial_num"] != DBNull.Value)
                            device.SerialNum = (string)reader["serial_num"];
                        if (reader["height"] != DBNull.Value)
                            device.Height = Convert.ToInt32(reader["height"]);
                        if (reader["weight"] != DBNull.Value)
                            device.Weight = Convert.ToInt32(reader["weight"]);
                        if (reader["isRackMountable"] != DBNull.Value)
                            device.IsRackMountable = (bool)reader["isRackMountable"];
                        if (reader["model"] != DBNull.Value)
                            device.Model = (string)reader["model"];
                        if (reader["brand"] != DBNull.Value)
                            device.Brand = (string)reader["brand"];
                        if (reader["input_voltage"] != DBNull.Value)
                            device.InputVoltage = (decimal)reader["input_voltage"];
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
        //sjekker om en enhet er kassert eller ikke
        public string DeviceIsDiscarded(Device device)
        {
            string query = "select * from log_event where device_id = '" + device.DeviceID + "' and event_type='12'";
            string message = null;
            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                        message = "Denne enheten er kassert";
                    else
                        message = "Denne enheten er ikke kassert";

                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    message = "Denne enheten er ikke kassert";
                    //close connection
                    this.CloseConnection();
                }
            }
            return message;
        }
        //sjekker om en enhet er i bruk eller ikke
        public string DeviceIsInUse(Device device)
        {
            string query = "select * from device_connectors where device_id = '" + device.DeviceID + "'";
            string message = null;
            //Sjekke for åpen connection mot db
            if (this.OpenConnection() == true)
            {
                //Lager en kommando
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //lage en data reader og utfører kommandoen
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                        message = "Denne enheten er i bruk";
                    else
                        message = "Denne enheten er ikke i bruk";

                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    message = "Denne enheten er ikke i bruk";
                    //close connection
                    this.CloseConnection();
                }
            }
            return message;
        }
        //sjekker om en enhet er i lånt bort eller ikke
        public string DeviceIsBorrowed(Device device)
        {
            string query = "select * from log_event l, company c where device_id = '" + device.DeviceID + "' and event_type='15' or event_type='16' and c.companyID = event_company";
            List<JoinLogEventWithNames> list = new List<JoinLogEventWithNames>();
            string message = null;
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
                        JoinLogEventWithNames addToList = new JoinLogEventWithNames();
                        var logEvent = new LogEvent();
                        logEvent.EventTypeID = Convert.ToInt32(reader["event_type"]);
                        logEvent.RegisteredDate = Convert.ToDateTime(reader["event_registered_date"]);
                        addToList.LogEvent = logEvent;
                        var company = new Company();
                        company.Name = (string)reader["companyname"];
                        addToList.Company = company;
                        list.Add(addToList);
                    }
                    reader.Close();
                    //close connection
                    this.CloseConnection();
                }
                catch
                {
                    message = "Denne enheten er ikke i bruk";
                    //close connection
                    this.CloseConnection();
                }
            }
            List<JoinLogEventWithNames> borrows = list.Where(r => r.LogEvent.EventTypeID == 15).OrderBy(x => x.LogEvent.RegisteredDate).Reverse().ToList();
            List<JoinLogEventWithNames> deliveries = list.Where(r => r.LogEvent.EventTypeID == 16).OrderBy(x => x.LogEvent.RegisteredDate).Reverse().ToList();

            if(deliveries.Count != 0 && borrows.Count != 0 && borrows[0].LogEvent.RegisteredDate >= deliveries[0].LogEvent.RegisteredDate)
                message = "Denne enheten er lånt ut til " + borrows[0].Company.Name;
            else if(deliveries.Count == 0 && borrows.Count != 0)
                message = "Denne enheten er lånt ut til " + borrows[0].Company.Name;
            else
                message = "Denne enheten er ikke lånt ut";

            return message;
        }
        //henter en enhet med netverk informasjon
        public DeviceWithNetworkInfo GetDeviceWithNetworkInfo(int deviceId)
        {
            string query = "select * from device d, network_info n where idDevice = '" + deviceId + "' and d.idDevice = n.device_id";
            DeviceWithNetworkInfo model = new DeviceWithNetworkInfo();
            Device device = new Device();
            NerworkInfo netInfo = new NerworkInfo();

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
                        device.DeviceID = Convert.ToInt32(reader["idDevice"]);
                        device.DeviceTypeID = Convert.ToInt32(reader["type"]);
                        device.RoomID = Convert.ToInt32(reader["default_location"]);
                        if (reader["devicename"] != DBNull.Value)
                            device.Name = (string)reader["devicename"];
                        if (reader["description"] != DBNull.Value)
                            device.Description = (string)reader["description"];
                        if (reader["serial_num"] != DBNull.Value)
                            device.SerialNum = (string)reader["serial_num"];
                        if (reader["height"] != DBNull.Value)
                            device.Height = Convert.ToInt32(reader["height"]);
                        if (reader["weight"] != DBNull.Value)
                            device.Weight = Convert.ToInt32(reader["weight"]);
                        if (reader["isRackMountable"] != DBNull.Value)
                            device.IsRackMountable = (bool)reader["isRackMountable"];
                        if (reader["model"] != DBNull.Value)
                            device.Model = (string)reader["model"];
                        if (reader["brand"] != DBNull.Value)
                            device.Brand = (string)reader["brand"];
                        if (reader["input_voltage"] != DBNull.Value)
                            device.InputVoltage = (decimal)reader["input_voltage"];

                        netInfo.DeviceID = Convert.ToInt32(reader["device_id"]);
                        if (reader["mac"] != DBNull.Value)
                            netInfo.MAC = (string)reader["mac"];
                        if (reader["ip"] != DBNull.Value)
                            netInfo.IP = (string)reader["ip"];
                        if (reader["subnet"] != DBNull.Value)
                            netInfo.Subnet = (string)reader["subnet"];
                        if (reader["network_infocol"] != DBNull.Value)
                            netInfo.NetworkInfocol = (string)reader["network_infocol"];

                        model.Device = device;
                        model.NetworkInfo = netInfo;
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
            return model;
        }
        //henter en enhet med alle forbindelser til anre enheter
        public DeviceWithConnections GetDeviceWithConnections(int deviceId)
        {
            string query = "select * from device d, device_connectors dc, connector_type ct, connector_type_has_pin cthp, connection c, pin p, signal_standard ss where d.idDevice = '" + deviceId + "' and d.idDevice = dc.device_id and dc.connector_type = ct.connID and ct.connID = cthp.conn_type_connID and c.connector_1 = dc.connector_id or c.connector_2 = dc.connector_id and p.pinId = cthp.pin_pinId and p.signal_standard = ss.signal_standardID";
            DeviceWithConnections model = new DeviceWithConnections();
            Device d = new Device();
            Connection c = new Connection();
            ConnectorType ct = new ConnectorType();
            ConnectorTypeHasPin cthp = new ConnectorTypeHasPin();
            SignalStandard ss = new SignalStandard();
            Pin p = new Pin();

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
                        d.DeviceID = Convert.ToInt32(reader["idDevice"]);
                        d.DeviceTypeID = Convert.ToInt32(reader["type"]);
                        d.RoomID = Convert.ToInt32(reader["default_location"]);
                        if (reader["devicename"] != DBNull.Value)
                            d.Name = (string)reader["devicename"];
                        if (reader["description"] != DBNull.Value)
                            d.Description = (string)reader["description"];
                        if (reader["serial_num"] != DBNull.Value)
                            d.SerialNum = (string)reader["serial_num"];
                        if (reader["height"] != DBNull.Value)
                            d.Height = Convert.ToInt32(reader["height"]);
                        if (reader["weight"] != DBNull.Value)
                            d.Weight = Convert.ToInt32(reader["weight"]);
                        if (reader["isRackMountable"] != DBNull.Value)
                            d.IsRackMountable = (bool)reader["isRackMountable"];
                        if (reader["model"] != DBNull.Value)
                            d.Model = (string)reader["model"];
                        if (reader["brand"] != DBNull.Value)
                            d.Brand = (string)reader["brand"];
                        if (reader["input_voltage"] != DBNull.Value)
                            d.InputVoltage = (decimal)reader["input_voltage"];

                        p.PinID = Convert.ToInt32(reader["pinId"]);
                        p.Description = (string)reader["descrip"];
                        p.Direction = (Enum)reader["direction"];
                        p.Signal = (Enum)reader["signal_p"];
                        if (reader["opticalMode"] != DBNull.Value)
                            p.OpticalMode = (Enum)reader["optiocalMode"];
                        if (reader["frequency"] != DBNull.Value)
                            p.Frequency = (Enum)reader["frequency"];
                        if (reader["impedance"] != DBNull.Value)
                            p.Impedance = (decimal)reader["impedance"];
                        if (reader["max_volt"] != DBNull.Value)
                            p.MaxVolt = (decimal)reader["max_volt"];
                        if (reader["min_volt"] != DBNull.Value)
                            p.MinVolt = (decimal)reader["min_volt"];

                        cthp.PinNr = Convert.ToInt32(reader["pin_nr"]);

                        ct.ConnectorTypeID = Convert.ToInt32(reader["connID"]);
                        ct.Type = (string)reader["conn_type"];
                        ct.Pins = Convert.ToInt32(reader["pins"]);
                        if (reader["signal_ct"] != DBNull.Value)
                            ct.Signal = (Enum)reader["signal_ct"];
                        if (reader["gender"] != DBNull.Value)
                            ct.Gender = (Enum)reader["gender"];
                        if (reader["defaultPinNames"] != DBNull.Value)
                            ct.DefaultPinNames = (Enum)reader["defaultPinNames"];
                        if (reader["shielded"] != DBNull.Value)
                            ct.Shielded = (bool)reader["shielded"];

                        c.ConnectionID = Convert.ToInt32(reader["connection_id"]);
                        c.ConnectorID1 = Convert.ToInt32(reader["connector_1"]);
                        c.ConnectorID2 = Convert.ToInt32(reader["connector_2"]);
                        c.PinNumCon1 = Convert.ToInt32(reader["pin_num_con1"]);
                        c.PinNumCon2 = Convert.ToInt32(reader["pin_num_con2"]);
                        if (reader["strand_num"] != DBNull.Value)
                            c.StrandNum = Convert.ToInt32(reader["strand_num"]);

                        ss.SignalStandardID = Convert.ToInt32(reader["signal_standardID"]);
                        ss.Name = (string)reader["signalname"];

                        model.Device = d;
                        model.Pin = p;
                        model.Connection = c;
                        model.ConnectorType = ct;
                        model.ConnectorTypeHasPin = cthp;
                        model.SignaldStandard = ss;
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
            return model;
        }
        //oppdatere en enhet i db
        public bool EditDevice(Device device)
        {
            string query = "UPDATE device SET devicename='" + device.Name + "', default_localtion='" + device.RoomID + "', description='" + device.Description + "', type='" + device.DeviceTypeID + "', serial_num='" + device.SerialNum + "', height=" + device.Height + ", weigth='" + device.Weight + "', isRackMountable='" + device.IsRackMountable + "', model='" + device.Model + "', brand='" + device.Brand + "', input_voltage='" + device.InputVoltage + "' where idDevice='" + device.DeviceID + "'";

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
        //sletter en enhet i db
        public bool DeleteDevice(Device device)
        {
            string query = "DELETE FROM device WHERE idDevice='" + device.DeviceID + "'";

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
                return true;
            }
            this.CloseConnection();
            return false;
        }
        #endregion
        #region DeviceConnector Queries
        #endregion
        #region DeviceType Queries
        //henter alle enhet typer
        public List<DeviceType> GetAllDeviceTypes()
        {
            string query = "select * from device_type";
            List<DeviceType> list = new List<DeviceType>();

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
                        var item = new DeviceType();
                        item.DeviceTypeID = Convert.ToInt32(reader["iddevice_type"]);
                        item.CanCalibrateID = Convert.ToInt32(reader["can_calibrate"]);
                        if (reader["type"] != DBNull.Value)
                            item.Type = (string)reader["type"];
                        if (reader["description"] != DBNull.Value)
                            item.Description = (string)reader["description"];

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
        //henter en enhet type med type id
        public DeviceType GetDeviceType(int deviceTypeId)
        {
            string query = "select * from device_type where iddevice_type = '" + deviceTypeId + "'";
            DeviceType deviceType = new DeviceType();

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
                        deviceType.DeviceTypeID = Convert.ToInt32(reader["iddevice_type"]);
                        deviceType.CanCalibrateID = Convert.ToInt32(reader["can_calibrate"]);
                        if (reader["type"] != DBNull.Value)
                            deviceType.Type = (string)reader["type"];
                        if (reader["description"] != DBNull.Value)
                            deviceType.Description = (string)reader["description"];
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
            return deviceType;
        }
        #endregion
        #region EventType Queries
        //henter alle log event typer
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
        //henter en log event type med type id
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
        #region FileQueries
        //oppretter en fil i db
        public int CreateFile(Files file)
        {
            string query = "Insert into files (filename, filepath, filetype, filesize, date) values('" + file.FileName + "','" + file.FilePath + "','" + file.FileType + "','" + file.FileSize + "','" + file.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'); select last_insert_id();";

            if (this.OpenConnection())
            {
                //Lager en kommando med query og forbindelse
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //Execute query
                    IDataReader reader = cmd.ExecuteReader();
                    int id = 0;
                    while (reader != null && reader.Read())
                        id = reader.GetInt32(0);
                    //close connection
                    this.CloseConnection();
                    return id;
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return 0;
        }
        //henter absolutt alle filene i db
        public List<Files> GetAllFiles()
        {
            string query = "select * from files";
            List<Files> list = new List<Files>();

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
                        var item = new Files();
                        item.FileID = Convert.ToInt32(reader["fileid"]);
                        if (reader["filename"] != DBNull.Value)
                            item.FileName = (string)reader["filename"];
                        item.FilePath = (string)reader["filepath"];
                        if (reader["filetype"] != DBNull.Value)
                            item.FileType = (string)reader["filetype"];
                        if (reader["filesize"] != DBNull.Value)
                            item.FileSize = Convert.ToInt32(reader["filesize"]);
                        item.Date = Convert.ToDateTime(reader["date"]);
                        if ((reader["deleted"]) != null)
                            item.Kassert = (bool)reader["deleted"];

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
        //henter alle filer som ikke er kassert
        public List<Files> GetAllFilesNotDiscarded()
        {
            string query = "select * from files where deleted = '0'";
            List<Files> list = new List<Files>();

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
                        var item = new Files();
                        item.FileID = Convert.ToInt32(reader["fileid"]);
                        if (reader["filename"] != DBNull.Value)
                            item.FileName = (string)reader["filename"];
                        item.FilePath = (string)reader["filepath"];
                        if (reader["filetype"] != DBNull.Value)
                            item.FileType = (string)reader["filetype"];
                        if (reader["filesize"] != DBNull.Value)
                            item.FileSize = Convert.ToInt32(reader["filesize"]);
                        item.Date = Convert.ToDateTime(reader["date"]);
                        if ((reader["deleted"]) != null)
                            item.Kassert = (bool)reader["deleted"];

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
        //henter alle filer som er kassert
        public List<Files> GetAllDiscardedFiles()
        {
            string query = "select * from files where deleted = '1'";
            List<Files> list = new List<Files>();

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
                        var item = new Files();
                        item.FileID = Convert.ToInt32(reader["fileid"]);
                        if (reader["filename"] != DBNull.Value)
                            item.FileName = (string)reader["filename"];
                        item.FilePath = (string)reader["filepath"];
                        if (reader["filetype"] != DBNull.Value)
                            item.FileType = (string)reader["filetype"];
                        if (reader["filesize"] != DBNull.Value)
                            item.FileSize = Convert.ToInt32(reader["filesize"]);
                        item.Date = Convert.ToDateTime(reader["date"]);
                        if ((reader["deleted"]) != null)
                            item.Kassert = (bool)reader["deleted"];

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
        //henter en fil med fil id
        public Files GetFile(int fileId)
        {
            string query = "select * from files where fileid = '" + fileId + "'";
            Files file = new Files();

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
                        file.FileID = Convert.ToInt32(reader["fileid"]);
                        if (reader["filename"] != DBNull.Value)
                            file.FileName = (string)reader["filename"];
                        file.FilePath = (string)reader["filepath"];
                        if (reader["filetype"] != DBNull.Value)
                            file.FileType = (string)reader["filetype"];
                        if (reader["filesize"] != DBNull.Value)
                            file.FileSize = Convert.ToInt32(reader["filesize"]);
                        file.Date = Convert.ToDateTime(reader["date"]);
                        if ((reader["deleted"]) != null)
                            file.Kassert = (bool)reader["deleted"];
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
            return file;
        }
        //sletter en fil fra db
        public bool DeleteFile(Files file)
        {
            string query = "DELETE FROM files WHERE fileid='" + file.FileID + "'";

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
                return true;
            }
            this.CloseConnection();
            return false;
        }
        //oppdatere en fil fra db
        public bool EditFile(Files file)
        {
            string query = "UPDATE files SET filename='" + file.FileName + "', filepath='" + file.FilePath + "', filetype='" + file.FileType + "', filesize='" + file.FileSize + "', date='" + file.Date.ToString("yyyy-MM-dd") + "', deleted=" + file.Kassert + " where fileid='" + file.FileID + "'";

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
        #region LogEvent Queries
        //oppretter et log event
        public bool CreateLogEvent(LogEvent logEvent)
        {
            string query = null;
            if(logEvent.FileID != 0)
                query = "Insert into log_event (event_type, device_id, event_registered_date, event_start_date, event_end_date, data_file, event_data1, event_data2, event_company, event_location) values('" + logEvent.EventTypeID + "','" + logEvent.DeviceID + "','" + logEvent.RegisteredDate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + logEvent.StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + logEvent.EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + logEvent.FileID + "','" + logEvent.Data1 + "','" + logEvent.Data2 + "','" + logEvent.CompanyID + "','" + logEvent.RoomID + "')";
            else
                query = "Insert into log_event (event_type, device_id, event_registered_date, event_start_date, event_end_date, event_data1, event_data2, event_company, event_location) values('" + logEvent.EventTypeID + "','" + logEvent.DeviceID + "','" + logEvent.RegisteredDate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + logEvent.StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + logEvent.EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + logEvent.Data1 + "','" + logEvent.Data2 + "','" + logEvent.CompanyID + "','" + logEvent.RoomID + "')";

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
        //henter alle log event fra db som ikke er kassert
        public List<LogEvent> GetAllLogEvent()
        {
            string query = "select * from log_event where event_id != '12'";
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
                        var item = new LogEvent();
                        item.LogEventID = Convert.ToInt32(reader["event_id"]);
                        item.EventTypeID = Convert.ToInt32(reader["event_type"]);
                        item.RoomID = Convert.ToInt32(reader["event_location"]);
                        item.CompanyID = Convert.ToInt32(reader["event_company"]);
                        item.DeviceID = Convert.ToInt32(reader["device_id"]);
                        item.RegisteredDate = Convert.ToDateTime(reader["event_registered_date"]);
                        if (reader["data_file"] != DBNull.Value)
                            item.FileID = Convert.ToInt32(reader["data_file"]);
                        if (reader["event_start_date"] != DBNull.Value)
                            item.StartDate = Convert.ToDateTime(reader["event_start_date"]);
                        if (reader["event_end_date"] != DBNull.Value)
                            item.EndDate = Convert.ToDateTime(reader["event_end_date"]);
                        if (reader["event_data1"] != DBNull.Value)
                            item.Data1 = (string)reader["event_data1"];
                        if (reader["event_data2"] != DBNull.Value)
                            item.Data2 = (string)reader["event_data2"];

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
        //henter absolutt alle log event
        public List<LogEvent> GetAllLogEventWithDiscarded()
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
                        var item = new LogEvent();
                        item.LogEventID = Convert.ToInt32(reader["event_id"]);
                        item.EventTypeID = Convert.ToInt32(reader["event_type"]);
                        item.RoomID = Convert.ToInt32(reader["event_location"]);
                        item.CompanyID = Convert.ToInt32(reader["event_company"]);
                        item.DeviceID = Convert.ToInt32(reader["device_id"]);
                        item.RegisteredDate = Convert.ToDateTime(reader["event_registered_date"]);
                        if(reader["data_file"] != DBNull.Value)
                            item.FileID = Convert.ToInt32(reader["data_file"]);
                        if (reader["event_start_date"] != DBNull.Value)
                            item.StartDate = Convert.ToDateTime(reader["event_start_date"]);
                        if (reader["event_end_date"] != DBNull.Value)
                            item.EndDate = Convert.ToDateTime(reader["event_end_date"]);
                        if (reader["event_data1"] != DBNull.Value)
                            item.Data1 = (string)reader["event_data1"];
                        if (reader["event_data2"] != DBNull.Value)
                            item.Data2 = (string)reader["event_data2"];

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
        //henter en log event fra db med event id
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
                        logEvent.LogEventID = Convert.ToInt32(reader["event_id"]);
                        logEvent.EventTypeID = Convert.ToInt32(reader["event_type"]);
                        logEvent.RoomID = Convert.ToInt32(reader["event_location"]);
                        logEvent.CompanyID = Convert.ToInt32(reader["event_company"]);
                        logEvent.DeviceID = Convert.ToInt32(reader["device_id"]);
                        logEvent.RegisteredDate = Convert.ToDateTime(reader["event_registered_date"]);
                        if (reader["data_file"] != DBNull.Value)
                            logEvent.FileID = Convert.ToInt32(reader["data_file"]);
                        if (reader["event_start_date"] != DBNull.Value)
                            logEvent.StartDate = Convert.ToDateTime(reader["event_start_date"]);
                        if (reader["event_end_date"] != DBNull.Value)
                            logEvent.EndDate = Convert.ToDateTime(reader["event_end_date"]);
                        if (reader["event_data1"] != DBNull.Value)
                            logEvent.Data1 = (string)reader["event_data1"];
                        if (reader["event_data2"] != DBNull.Value)
                            logEvent.Data2 = (string)reader["event_data2"];
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
        //oppdaterer et event i db
        public bool EditLogEvent(LogEvent logEvent)
        {
            string query = null;
            //logEvent.registrert vil alltid være datetime.now, start og end date kan være null, har dem ikke en dato over år 2000, skal dem heller ikke settes inn i db
            if(logEvent.StartDate.Year > 2000 && logEvent.EndDate.Year > 2000)
                query = "UPDATE log_event SET event_type='" + logEvent.EventTypeID + "', device_id='" + logEvent.DeviceID + "', event_company='" + logEvent.CompanyID + "', event_location='" + logEvent.RoomID + "', event_registered_date='" + logEvent.RegisteredDate.Date.ToString("yyyy-MM-dd") + "', event_start_date='" + logEvent.StartDate.Date.ToString("yyyy-MM-dd") + "', event_end_date='" + logEvent.EndDate.Date.ToString("yyyy-MM-dd") + "', data_file='" + logEvent.FileID + "', event_data1='" + logEvent.Data1 + "', event_data2='" + logEvent.Data2 + "' WHERE event_id='" + logEvent.LogEventID + "'";
            else
                query = "UPDATE log_event SET event_type='" + logEvent.EventTypeID + "', device_id='" + logEvent.DeviceID + "', event_company='" + logEvent.CompanyID + "', event_location='" + logEvent.RoomID + "', event_registered_date='" + logEvent.RegisteredDate.Date.ToString("yyyy-MM-dd") + "', event_start_date='" + DBNull.Value + "', event_end_date='" + DBNull.Value + "', data_file='" + logEvent.FileID + "', event_data1='" + logEvent.Data1 + "', event_data2='" + logEvent.Data2 + "' WHERE event_id='" + logEvent.LogEventID + "'";

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
        //henter det siste eventet til en enhet
        public LogEvent GetLastEventForDevice(int deviceID)
        {
            string query = "select * from log_event where device_id = '" + deviceID + "' where event_start_date < CURDATE()";
            LogEvent logEvent = new LogEvent();
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
                        var item = new LogEvent();
                        item.LogEventID = Convert.ToInt32(reader["event_id"]);
                        item.EventTypeID = Convert.ToInt32(reader["event_type"]);
                        item.RoomID = Convert.ToInt32(reader["event_location"]);
                        item.CompanyID = Convert.ToInt32(reader["event_company"]);
                        item.DeviceID = Convert.ToInt32(reader["device_id"]);
                        item.RegisteredDate = Convert.ToDateTime(reader["event_registered_date"]);
                        if (reader["data_file"] != DBNull.Value)
                            item.FileID = Convert.ToInt32(reader["data_file"]);
                        if (reader["event_start_date"] != DBNull.Value)
                            item.StartDate = Convert.ToDateTime(reader["event_start_date"]);
                        if (reader["event_end_date"] != DBNull.Value)
                            item.EndDate = Convert.ToDateTime(reader["event_end_date"]);
                        if (reader["event_data1"] != DBNull.Value)
                            item.Data1 = (string)reader["event_data1"];
                        if (reader["event_data2"] != DBNull.Value)
                            item.Data2 = (string)reader["event_data2"];

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
            list.GroupBy(x => x.EndDate);
            list.Reverse();
            for (int i = 0; i < list.Count; i++)
                logEvent = list[0];

            return logEvent;
        }
        //henter alle event for en enhet
        public List<LogEvent> GetLogEventForDevice(int deviceID)
        {
            string query = "select * from log_event where device_id = '" + deviceID + "'";
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
                        var item = new LogEvent();
                        item.LogEventID = Convert.ToInt32(reader["event_id"]);
                        item.EventTypeID = Convert.ToInt32(reader["event_type"]);
                        item.RoomID = Convert.ToInt32(reader["event_location"]);
                        item.CompanyID = Convert.ToInt32(reader["event_company"]);
                        item.DeviceID = Convert.ToInt32(reader["device_id"]);
                        item.RegisteredDate = Convert.ToDateTime(reader["event_registered_date"]);
                        if (reader["data_file"] != DBNull.Value)
                            item.FileID = Convert.ToInt32(reader["data_file"]);
                        if (reader["event_start_date"] != DBNull.Value)
                            item.StartDate = Convert.ToDateTime(reader["event_start_date"]);
                        if (reader["event_end_date"] != DBNull.Value)
                            item.EndDate = Convert.ToDateTime(reader["event_end_date"]);
                        if (reader["event_data1"] != DBNull.Value)
                            item.Data1 = (string)reader["event_data1"];
                        if (reader["event_data2"] != DBNull.Value)
                            item.Data2 = (string)reader["event_data2"];

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
        //henter det siste eventet til en fil
        public LogEvent GetLogEventByFileId(int fileId)
        {
            string query = "select * from log_event where data_file = '" + fileId + "'";
            LogEvent logEvent = new LogEvent();
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
                        var item = new LogEvent();
                        item.LogEventID = Convert.ToInt32(reader["event_id"]);
                        item.EventTypeID = Convert.ToInt32(reader["event_type"]);
                        item.RoomID = Convert.ToInt32(reader["event_location"]);
                        item.CompanyID = Convert.ToInt32(reader["event_company"]);
                        item.DeviceID = Convert.ToInt32(reader["device_id"]);
                        item.RegisteredDate = Convert.ToDateTime(reader["event_registered_date"]);
                        if (reader["data_file"] != DBNull.Value)
                            item.FileID = Convert.ToInt32(reader["data_file"]);
                        if (reader["event_start_date"] != DBNull.Value)
                            item.StartDate = Convert.ToDateTime(reader["event_start_date"]);
                        if (reader["event_end_date"] != DBNull.Value)
                            item.EndDate = Convert.ToDateTime(reader["event_end_date"]);
                        if (reader["event_data1"] != DBNull.Value)
                            item.Data1 = (string)reader["event_data1"];
                        if (reader["event_data2"] != DBNull.Value)
                            item.Data2 = (string)reader["event_data2"];

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
            list.OrderBy(x => x.LogEventID).Reverse();
            logEvent = list.FirstOrDefault();

            return logEvent;
        }
        //henter alle event til en event type
        public List<LogEvent> GetAllLogEventToEventType(int eventTypeId)
        {
            string query = "select * from log_event where event_type='" + eventTypeId + "'";
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
                        var item = new LogEvent();
                        item.LogEventID = Convert.ToInt32(reader["event_id"]);
                        item.EventTypeID = Convert.ToInt32(reader["event_type"]);
                        item.RoomID = Convert.ToInt32(reader["event_location"]);
                        item.CompanyID = Convert.ToInt32(reader["event_company"]);
                        item.DeviceID = Convert.ToInt32(reader["device_id"]);
                        item.RegisteredDate = Convert.ToDateTime(reader["event_registered_date"]);
                        if (reader["data_file"] != DBNull.Value)
                            item.FileID = Convert.ToInt32(reader["data_file"]);
                        if (reader["event_start_date"] != DBNull.Value)
                            item.StartDate = Convert.ToDateTime(reader["event_start_date"]);
                        if (reader["event_end_date"] != DBNull.Value)
                            item.EndDate = Convert.ToDateTime(reader["event_end_date"]);
                        if (reader["event_data1"] != DBNull.Value)
                            item.Data1 = (string)reader["event_data1"];
                        if (reader["event_data2"] != DBNull.Value)
                            item.Data2 = (string)reader["event_data2"];

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
        //sletter en event i db
        public bool DeleteLogEvent(LogEvent logEvent)
        {
            string query = "DELETE FROM log_event WHERE event_id='" + logEvent.LogEventID + "'";

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
                return true;
            }
            this.CloseConnection();
            return false;
        }
        //henter et event med rom, enhet, fil, event type og firma
        public JoinLogEventWithNames JoinQuery(LogEvent logevent)
        {
            JoinLogEventWithNames model = new JoinLogEventWithNames();
            string query;
            if(logevent.FileID != 0)
                query = "select * from log_event l, device d, event_types e, room r, company c, files f where l.event_id = '" + logevent.LogEventID + "' and l.event_type = e.log_type and l.device_id = d.idDevice and l.data_file = f.fileid and l.event_company = c.companyID and l.event_location = r.id";
            else
                query = "select * from log_event l, device d, event_types e, room r, company c where l.event_id = '" + logevent.LogEventID + "' and l.event_type = e.log_type and l.device_id = d.idDevice and l.event_company = c.companyID and l.event_location = r.id";

            LogEvent logEvent = new LogEvent();
            Device device = new Device();
            EventType type = new EventType();
            Room room = new Room();
            Company company = new Company();
            Files file = new Files();
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
                        logEvent.LogEventID = Convert.ToInt32(reader["event_id"]);
                        company.CompanyID = Convert.ToInt32(reader["companyID"]);
                        device.DeviceID = Convert.ToInt32(reader["idDevice"]);
                        logEvent.RegisteredDate = Convert.ToDateTime(reader["event_registered_date"]);
                        if (reader["log_type_name"] != DBNull.Value)
                            type.Name = (string)reader["log_type_name"];
                        if (logevent.FileID != 0 && reader["filename"] != DBNull.Value)
                            file.FileName = (string)reader["filename"];
                        if (reader["roomname"] != DBNull.Value)
                            room.Name = (string)reader["roomname"];
                        if (reader["devicename"] != DBNull.Value)
                            device.Name = (string)reader["devicename"];
                        if (reader["companyname"] != DBNull.Value)
                            company.Name = (string)reader["companyname"];
                        if (reader["event_start_date"] != DBNull.Value)
                            logEvent.StartDate = Convert.ToDateTime(reader["event_start_date"]);
                        if (reader["event_end_date"] != DBNull.Value)
                            logEvent.EndDate = Convert.ToDateTime(reader["event_end_date"]);
                        if (reader["event_data1"] != DBNull.Value)
                            logEvent.Data1 = (string)reader["event_data1"];
                        if (reader["event_data2"] != DBNull.Value)
                            logEvent.Data2 = (string)reader["event_data2"];
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
            model.Company = company;
            model.Device = device;
            model.EventType = type;
            model.File = file;
            model.LogEvent = logEvent;
            model.Room = room;
            return model;
        }
        //henter den neste kalibrering for en enhet
        public LogEvent GetNextCalibrationForDeivce(Device deivce)
        {
            string query = "select * from log_event where device_id = '" + deivce.DeviceID + "' and event_start_date > CURRENT_DATE";
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
                        var item = new LogEvent();
                        item.LogEventID = Convert.ToInt32(reader["event_id"]);
                        item.EventTypeID = Convert.ToInt32(reader["event_type"]);
                        item.RoomID = Convert.ToInt32(reader["event_location"]);
                        item.CompanyID = Convert.ToInt32(reader["event_company"]);
                        item.DeviceID = Convert.ToInt32(reader["device_id"]);
                        item.RegisteredDate = Convert.ToDateTime(reader["event_registered_date"]);
                        if (reader["data_file"] != DBNull.Value)
                            item.FileID = Convert.ToInt32(reader["data_file"]);
                        if (reader["event_start_date"] != DBNull.Value)
                            item.StartDate = Convert.ToDateTime(reader["event_start_date"]);
                        if (reader["event_end_date"] != DBNull.Value)
                            item.EndDate = Convert.ToDateTime(reader["event_end_date"]);
                        if (reader["event_data1"] != DBNull.Value)
                            item.Data1 = (string)reader["event_data1"];
                        if (reader["event_data2"] != DBNull.Value)
                            item.Data2 = (string)reader["event_data2"];

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
            if(list.Count != 0)
            {
                list.GroupBy(x => x.StartDate);
                return list[0];
            }
            return null;
        }
        #endregion
        #region NetworkInfo Queries
        #endregion
        #region Pin Queries
        #endregion
        #region PostCode Queries
        #endregion
        #region Rights Queries
        //henter rettigheten til en bruker
        public Rights GetRightToUser(User user)
        {
            string query = "select * from rights where rightsid =" + user.RightsID;
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
                        right.RightsID = Convert.ToInt32(reader["rightsID"]);
                        right.Name = (string)reader["rightName"];
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
        //henter alle rettigheter
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
                        var item = new Rights();
                        item.RightsID = Convert.ToInt32(reader["rightsID"]);
                        item.Name = (string)reader["rightName"];

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
        //henter alle rommene
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
                        var item = new Room();
                        item.RoomID = Convert.ToInt32(reader["id"]);
                        item.BuildingID = Convert.ToInt32(reader["building_id"]);
                        if (reader["roomname"] != DBNull.Value)
                            item.Name = (string)reader["roomname"];
                        if (reader["description"] != DBNull.Value)
                            item.Description = (string)reader["description"];

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
        //henter et rom med rom id
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
                        room.RoomID = Convert.ToInt32(reader["id"]);
                        room.BuildingID = Convert.ToInt32(reader["building_id"]);
                        if (reader["roomname"] != DBNull.Value)
                            room.Name = (string)reader["roomname"];
                        if (reader["description"] != DBNull.Value)
                            room.Description = (string)reader["description"];
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
        //henter en bruker med rettigheten til brukeren
        public UserRight GetUserWithRights(int userID, Constant.Rights rights)
        {
            string query = "select * from user u, rights r where u.iduser='" + userID + "' and r.rightName='" + rights + "' and u.rightsid=r.rightsID";
            User user = new User();
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
                        user.UserId = Convert.ToInt32(reader["iduser"]);
                        user.RightsID = Convert.ToInt32(reader["rightsid"]);
                        user.Checked = (bool)reader["checked"];
                        if (reader["name"] != DBNull.Value)
                            user.Name = (string)reader["name"];
                        if (reader["email"] != DBNull.Value)
                            user.Email = (string)reader["email"];
                        if (reader["passhash"] != DBNull.Value)
                            user.PassHash = (string)reader["passhash"];
                        if (reader["passsalt"] != DBNull.Value)
                            user.PassSalt = (string)reader["passsalt"];                            

                        right.RightsID = Convert.ToInt32(reader["rightsID"]);
                        right.Name = (string)reader["rightName"];
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
            UserRight model = new UserRight();
            if (right.RightsID != 0 && user.UserId != 0)
            {
                model.User = user;
                model.Right = right;
            }
            return model;
        }
        //henter en bruker med bruker id
        public User GetUser(int userID)
        {
            string query = "select * from user where iduser ='" + userID + "'";
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
                        user.UserId = Convert.ToInt32(reader["iduser"]);
                        user.RightsID = Convert.ToInt32(reader["rightsid"]);
                        user.Checked = (bool)reader["checked"];
                        if (reader["name"] != DBNull.Value)
                            user.Name = (string)reader["name"];
                        if (reader["email"] != DBNull.Value)
                            user.Email = (string)reader["email"];
                        if (reader["passhash"] != DBNull.Value)
                            user.PassHash = (string)reader["passhash"];
                        if (reader["passsalt"] != DBNull.Value)
                            user.PassSalt = (string)reader["passsalt"];
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
        //henter en bruker med email
        public User GetUser(string email)
        {
            string query = "select * from user where email ='" + email + "'";
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
                        user.UserId = Convert.ToInt32(reader["iduser"]);
                        user.RightsID = Convert.ToInt32(reader["rightsid"]);
                        user.Checked = (bool)reader["checked"];
                        if (reader["name"] != DBNull.Value)
                            user.Name = (string)reader["name"];
                        if (reader["email"] != DBNull.Value)
                            user.Email = (string)reader["email"];
                        if (reader["passhash"] != DBNull.Value)
                            user.PassHash = (string)reader["passhash"];
                        if (reader["passsalt"] != DBNull.Value)
                            user.PassSalt = (string)reader["passsalt"];
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
        //oppretter en bruker
        public int CreateUser(User user)
        {
            string query = null;
            if(user.RightsID != 0)
                query = "Insert into user (name, email, passhash, passsalt, rightsid, checked) values('" + user.Name.ToLower() + "','" + user.Email.ToLower() + "','" + user.PassHash + "','" + user.PassSalt + "','" + user.RightsID + "','1'); select last_insert_id();";
            else
                query = "Insert into user (name, email, passhash, passsalt) values('" + user.Name.ToLower() + "','" + user.Email.ToLower() + "','" + user.PassHash + "','" + user.PassSalt + "'); select last_insert_id();";

            if(this.OpenConnection())
            {
                //Lager en kommando med query og forbindelse
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    //utfører kommandoen
                    IDataReader reader = cmd.ExecuteReader();
                    int id = 0;
                    while (reader != null && reader.Read())
                        id = reader.GetInt32(0);
                    //close connection
                    this.CloseConnection();
                    return id;
                }
                catch
                {
                    //close connection
                    this.CloseConnection();
                }
            }
            return 0;
        }
        //henter alle brukere som har fått rettigheter av admin
        public List<User> GetAllUsers()
        {
            string query = "select * from user where checked='1'";
            List<User> userList = new List<User>();

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
                        var user = new User();
                        user.UserId = Convert.ToInt32(reader["iduser"]);
                        user.RightsID = Convert.ToInt32(reader["rightsid"]);
                        user.Checked = (bool)reader["checked"];
                        if (reader["name"] != DBNull.Value)
                            user.Name = (string)reader["name"];
                        if (reader["email"] != DBNull.Value)
                            user.Email = (string)reader["email"];
                        if (reader["passhash"] != DBNull.Value)
                            user.PassHash = (string)reader["passhash"];
                        if (reader["passsalt"] != DBNull.Value)
                            user.PassSalt = (string)reader["passsalt"];

                        userList.Add(user);
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
            return userList;
        }
        //henter alle brukere som ikke har fått rettigheter enda av admin
        public List<User> GetAllUsersUnchecked()
        {
            string query = "select * from user where checked='0'";
            List<User> userList = new List<User>();

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
                        var user = new User();
                        user.UserId = Convert.ToInt32(reader["iduser"]);
                        user.RightsID = Convert.ToInt32(reader["rightsid"]);
                        user.Checked = (bool)reader["checked"];
                        if (reader["name"] != DBNull.Value)
                            user.Name = (string)reader["name"];
                        if (reader["email"] != DBNull.Value)
                            user.Email = (string)reader["email"];
                        if (reader["passhash"] != DBNull.Value)
                            user.PassHash = (string)reader["passhash"];
                        if (reader["passsalt"] != DBNull.Value)
                            user.PassSalt = (string)reader["passsalt"];

                        userList.Add(user);
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
            return userList;
        }
        //oppdaterer en bruker i db
        public bool EditUser(User user)
        {
            string query = "UPDATE user SET name='" + user.Name + "', email='" + user.Email + "', passhash='" + user.PassHash + "', passsalt='" + user.PassSalt + "', rightsid='" + user.RightsID + "', checked=" + user.Checked + " where iduser='" + user.UserId + "'";

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
    }
}