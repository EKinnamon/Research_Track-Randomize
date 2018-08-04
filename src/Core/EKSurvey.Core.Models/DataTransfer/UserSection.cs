using System;

namespace EKSurvey.Core.Models.DataTransfer
{
    public class UserSection : IUserSection
    {
        public string UserId { get; set; }

        public int SurveyId { get; set; }

        public Guid TestId { get; set; }

        public int Order { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Completed { get; set; }
        public int Id { get; set; }

        public string Name { get; set; }

        public bool? IsSelected { get; set; }
    }
}
