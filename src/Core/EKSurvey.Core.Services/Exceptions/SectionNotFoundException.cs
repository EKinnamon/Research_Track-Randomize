using System;

namespace EKSurvey.Core.Services.Exceptions
{
    public class SectionNotFoundException : Exception
    {
        public int SectionId { get; set; }

        public SectionNotFoundException(int sectionId) : base($"Section `{sectionId}` could not be found.")
        {
            SectionId = sectionId;
        }

        public SectionNotFoundException(int sectionId, string message) : base(message)
        {
            SectionId = sectionId;
        }

        public SectionNotFoundException(int sectionId, string message, Exception innerException) : base(message, innerException)
        {
            SectionId = sectionId;
        }
    }
}
