using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HovedOppgave.Models
{
    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nåværende passord")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nytt passord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekreft nytt passord")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public User AdminUser { get; set; }
        public User ChangeUser { get; set; }
        public Rights Right { get; set; }
        public List<Rights> Rights { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Epost")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [Display(Name = "Husk meg?")]
        public bool RememberMe { get; set; }
    }

    public class CreatUserViewModel
    {
        [Required]
        [Display(Name = "Navn")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekreft passord")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Epost")]
        public string Email { get; set; }

        [Display(Name = "Rettigheter")]
        public Rights Right { get; set; }

        public List<Rights> Rights { get; set; }
    }

    public class LostPasswordViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
    
    public class CalibrationViews
    {
        public Room Room { get; set; }
        public Files FileTo { get; set; }
        public Device Device { get; set; }
        public Company Company { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Files> Files { get; set; }
        public LogEvent LogEvent { get; set; }
        public EventType EventType { get; set; }
        public List<Device> Devices { get; set; }
        public DeviceType DeviceType { get; set; }
        public List<Company> Companys { get; set; }
        public string ExtraStringHelp { get; set; }
        public string ExtraStringHelp1 { get; set; }
        public string ExtraStringHelp2 { get; set; }
        public List<LogEvent> LogEvents { get; set; }
        public List<EventType> EventTypes { get; set; }
        public List<Files> FilesToLogevent { get; set; }
        public List<DeviceType> DeviceTypes { get; set; }
        public List<JoinLogEventWithNames> JoinQuery { get; set; }
        public DeviceWithNetworkInfo DeviceWithNetwork { get; set; }
    }

    public class UserRight
    {
        public User User { get; set; }
        public Rights Right { get; set; }
    }

    public class AdminViews
    {
        public User User { get; set; }
        public Rights Right { get; set; }
        public List<User> Users { get; set; }
        public List<Rights> Rights { get; set; }
    }

    public class JoinLogEventWithNames
    {
        public Room Room { get; set; }
        public Files File { get; set; }
        public Device Device { get; set; }
        public Company Company { get; set; }
        public LogEvent LogEvent { get; set; }
        public EventType EventType { get; set; }
    }

    public class DeviceWithNetworkInfo
    {
        public NerworkInfo NetworkInfo { get; set; }
        public Device Device { get; set; }
    }

    public class DeviceWithConnections
    {
        public Device Device { get; set; }
        public ConnectorType ConnectorType { get; set; }
        public ConnectorTypeHasPin ConnectorTypeHasPin { get; set; }
        public Connection Connection { get; set; }
        public Pin Pin { get; set; }
        public SignalStandard SignaldStandard { get; set; }
    }

    public class CompanyWithContact
    {
        public Contact Contact { get; set; }
        public Company Company { get; set; }
        public ContactInfo ContactInfo { get; set; }
        public ContactInfoType ContactInfoType { get; set; }
    }
}