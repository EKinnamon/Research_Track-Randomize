using System;

namespace EKSurvey.Core.Services.Exceptions
{
    public class SurveyNotFoundException : Exception
    {
        public int SurveyId { get; set; }

        public SurveyNotFoundException(int surveyId) : base($"Survey `{surveyId}` could not be found.")
        {
            SurveyId = surveyId;
        }

        public SurveyNotFoundException(int surveyId, string message) : base(message)
        {
            SurveyId = surveyId;
        }

        public SurveyNotFoundException(int surveyId, string message, Exception innerException) : base(message, innerException)
        {
            SurveyId = surveyId;
        }
    }
}