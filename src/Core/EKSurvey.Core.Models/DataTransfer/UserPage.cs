using System;
using EKSurvey.Core.Models.Entities;

namespace EKSurvey.Core.Models.DataTransfer
{
    public class UserPage
    {
        public string UserId { get; set; }
        public int SurveyId { get; set; }
        public string SurveyName { get; set; }
        public Guid? TestResponseId { get; set; }
        public IPage Page { get; set; }
        public string Response { get; set; }
        public DateTime? Responded { get; set; }
        public DateTime? Modified { get; set; }
    }
}
