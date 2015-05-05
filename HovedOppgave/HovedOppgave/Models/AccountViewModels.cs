using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HovedOppgave.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
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
        [Display(Name = "Gjenta passord")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Epost")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Rettigheter")]
        public IEnumerable<int> SelectedRight { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> Rights { get; set; }
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
        public Files File { get; set; }
        public Device Device { get; set; }
        public string FilePath { get; set; }
        public Company Company { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Files> Files { get; set; }
        public LogEvent LogEvent { get; set; }
        public EventType EventType { get; set; }
        public List<Device> Devices { get; set; }
        public DeviceType DeviceType { get; set; }
        public List<Company> Companys { get; set; }
        public List<LogEvent> LogEvents { get; set; }
        public List<EventType> EventTypes { get; set; }
        public List<DeviceType> DeviceTypes { get; set; }
    }

    public class UserRight
    {
        public User user { get; set; }
        public Rights rights { get; set; }
    }
}