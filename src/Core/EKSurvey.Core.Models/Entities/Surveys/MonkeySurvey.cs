using System;

namespace EKSurvey.Core.Models.Entities.Surveys
{
    public class MonkeySurvey : ISurvey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public string Url { get; set; }
    }
}