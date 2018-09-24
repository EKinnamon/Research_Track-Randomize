namespace EKSurvey.Core.Models.Entities
{
    public class TrueFalseQuestion : Page, IQuestion
    {
        public string True { get; set; }

        public string False { get; set; }
    }
}