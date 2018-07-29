using EKSurvey.Core.Models.Attributes;

namespace EKSurvey.Core.Models.Entities
{
    [Question]
    public class RangeQuestion : Page
    {
        public int Range { get; set; }
    }
}