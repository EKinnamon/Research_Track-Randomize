using System.Collections.Generic;
using EKSurvey.Core.Models.DataTransfer;

namespace EKSurvey.Core.Models.ViewModels.Survey
{
    public class IndexViewModel
    {
        public UserSurvey NextSurvey { get; set; }

        public ICollection<UserSurvey> AvailableSurveys { get; set; }

        public ICollection<UserSurvey> CompletedSurveys { get; set; }

    }
}
