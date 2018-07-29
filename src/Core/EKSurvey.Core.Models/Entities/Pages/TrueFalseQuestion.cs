using EKSurvey.Core.Models.Attributes;

namespace EKSurvey.Core.Models.Entities
{
    [Question]
    public class TrueFalseQuestion : Page
    {
        public string True { get; set; }

        public string False { get; set; }
    }
}