using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace PronadjiUBanovcima.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Telefon { get; set; }
        public string Adresa { get; set; }

        public string Broj { get; set; }
        public string Mesto { get; set; }

        public string Sajt { get; set; }
        public IEnumerable<SelectListItem> ListaDelatnosti { get; set; }
        public IEnumerable<Tag> ListaTagova { get; set; }
        public IEnumerable<string> SelectedDels { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ImageUpload { get; set; }
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


    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User name")]
        [Remote("DoesUserNameExist", "Validation", HttpMethod="POST", ErrorMessage="Korisnik već postoji. Izaberite drugo ime.")]
       
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Ovo polje je obavezno!")]
        [EmailAddress]
        [Remote("DoesEmailExist", "Validation", HttpMethod="POST", ErrorMessage="Nalog sa ovom mejl adresom već postoji. Izaberite drugu mejl adresu.")]
        public string Email { get; set; }

        public string Firma { get; set; }
        [Phone]
        [DataType(DataType.PhoneNumber)]
        public string Telefon { get; set; }
        public string Adresa { get; set; }

        public string Broj { get; set; }
        public string Mesto { get; set; }

        public string Sajt { get; set; }
        public IEnumerable<SelectListItem> ListaDelatnosti { get; set; }
        public IEnumerable<Tag> ListaTagova { get; set; }
        public IEnumerable<string> SelectedDels { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ImageUpload { get; set; }
    }
}
 