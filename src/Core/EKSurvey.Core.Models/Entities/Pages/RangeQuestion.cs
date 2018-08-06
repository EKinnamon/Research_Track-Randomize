namespace EKSurvey.Core.Models.Entities
{
    public class RangeQuestion : Page, IQuestion
    {
        public int Range { get; set; }
        public bool IsLickert { get; set; }
    }
}