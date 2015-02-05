using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Adress { get; set; }
        public string Postalcode { get; set; }
        public string PostalPlace { get; set; }

        public User()
        {

        }

        public User(User user)
        {
            this.FirstName = user.FirstName;
            this.SurName = user.SurName;
            this.Email = user.Email;
            this.PhoneNumber = user.PhoneNumber;
            this.Adress = user.Adress;
            this.Postalcode = user.Postalcode;
            this.PostalPlace = user.PostalPlace;
        }

        public User(string firstName, string surName, string email, string phoneNumber, string adress, string postalcode, string postalPlace)
        {
            this.FirstName = firstName;
            this.SurName = surName;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
            this.Adress = adress;
            this.Postalcode = postalcode;
            this.PostalPlace = postalPlace;
        }
    }
}