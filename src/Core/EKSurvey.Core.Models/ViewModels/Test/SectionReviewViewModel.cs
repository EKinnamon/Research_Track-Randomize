using System.Collections.Generic;
using EKSurvey.Core.Models.DataTransfer;

namespace EKSurvey.Core.Models.ViewModels.Test
{
    public class SectionReviewViewModel
    {
        public int SurveyId { get; set; }
        public int SectionId { get; set; }
        public ICollection<UserResponse> Responses { get; set; }
        public bool IsLastSection { get; set; }
    }
}
