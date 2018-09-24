using System.ComponentModel.DataAnnotations;

namespace EKSurvey.Core.Models.ViewModels.Manager
{
    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }
}