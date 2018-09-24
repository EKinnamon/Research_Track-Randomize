using System.ComponentModel.DataAnnotations;

namespace EKSurvey.Core.Models.ViewModels.Test
{
    public class CreateViewModel
    {
        public string SurveyName { get; set; }

        [Required]
        public int SurveyId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
