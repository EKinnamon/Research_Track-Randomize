using System;

namespace EKSurvey.Core.Services.Exceptions
{
    public class SectionMarkerNotFoundException : Exception
    {
        public Guid TestId { get; set; }
        public int SectionId { get; set; }

        public SectionMarkerNotFoundException(Guid testId, int sectionId) : base($"Section for test `{testId}`, section `{sectionId}` not found.")
        {
            TestId = testId;
            SectionId = sectionId;
        }

        public SectionMarkerNotFoundException(Guid testId, int sectionId, string message) : base(message)
        {
            TestId = testId;
            SectionId = sectionId;
        }

        public SectionMarkerNotFoundException(Guid testId, int sectionId, string message, Exception innerException) : base(message, innerException)
        {
            TestId = testId;
            SectionId = sectionId;
        }
    }
}
