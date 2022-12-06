using System.ComponentModel.DataAnnotations; //Grants access to annotations for validation

namespace PersonalSite.UI.MVC.Models
{
   public class ContactViewModel
        {
            //We can use Data Annotations to add validation to our model.
            //This is useful when we have required fields or need certain types of info.

            [Required(ErrorMessage = "*Name is required")]
            public string Name { get; set; } //Must be alphabetic characters

            [Required(ErrorMessage = "*Email is required")]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; } //Must contain an @ symbol and an extension (.com, .net, etc)

            [Required(ErrorMessage = "*Subject is required")]
            public string Subject { get; set; } //Must contain text

            [Required(ErrorMessage = "*Message is required")]
            [DataType(DataType.MultilineText)] //Notes that the field should have a bigger textbox (textarea)
            public string Message { get; set; } //Must contain text
        }
}
