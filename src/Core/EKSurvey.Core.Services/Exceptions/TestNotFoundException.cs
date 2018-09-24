using System;

namespace EKSurvey.Core.Services.Exceptions
{
    public class TestNotFoundException : Exception
    {
        public string UserId { get; set; }
        public int SurveyId { get; set; }

        public TestNotFoundException(string userId, int surveyId) : base($"Test could not be found for user `{userId}` and survey `{surveyId}`.")
        {
            UserId = userId;
            SurveyId = surveyId;
        }

        public TestNotFoundException(string userId, int surveyId, string message) : base(message)
        {
            UserId = userId;
            SurveyId = surveyId;
        }

        public TestNotFoundException(string userId, int surveyId, string message, Exception innerException) : base(message, innerException)
        {
            UserId = userId;
            SurveyId = surveyId;
        }
    }
}
