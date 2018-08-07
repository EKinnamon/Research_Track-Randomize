using System;

namespace EKSurvey.Core.Models.DataTransfer
{
    public class UserResponse
    {
        public string UserId { get; set; }

        public int SurveyId { get; set; }

        public int SectionId { get; set; }

        public bool IsQuestion { get; set; }

        public Type QuestionType { get; set; }

        public int PageId { get; set; }

        public bool IsHtml { get; set; }

        public bool IsLikert { get; set; }

        public int? Range { get; set; }

        public string Text { get; set; }

        public Guid TestId { get; set; }

        public int Order { get; set; }

        public string Response { get; set; }
    }
}
