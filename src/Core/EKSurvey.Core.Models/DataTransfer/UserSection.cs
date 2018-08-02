using System;
using EKSurvey.Core.Models.Enums;

namespace EKSurvey.Core.Models.DataTransfer
{
    public class UserSection
    {
        public string UserId { get; set; }

        public int Id { get; set; }

        public int SurveyId { get; set; }

        public Guid TestId { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public SelectorType? SelectorType { get; set; }

        public bool? IsSelected { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Completed { get; set; }

    }
}
