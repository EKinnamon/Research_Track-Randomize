using EKSurvey.Core.Models.Entities;

namespace EKSurvey.Core.Models.ViewModels.Test
{
    public class ResponseViewModel
    {
        public string UserId { get; set; }
        public int SurveyId { get; set; }
        public IPage Page { get; set; }
        public int? PriorPageId { get; set; }
    }
}
