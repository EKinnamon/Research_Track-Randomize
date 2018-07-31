using System;

namespace EKSurvey.Core.Services.Exceptions
{
    public class InvalidSectionConfiguration : Exception
    {
        public string SurveyName  { get; set; }

        public InvalidSectionConfiguration(string surveyName) : base($"Survey `{surveyName}` has an invalid section configuration.")
        {
            SurveyName = surveyName;
        }

        public InvalidSectionConfiguration(string surveyName, string message) : base(message)
        {
            SurveyName = surveyName;
        }

        public InvalidSectionConfiguration(string surveyName, string message, Exception innerException) : base(message, innerException)
        {
            SurveyName = surveyName;
        }
    }
}
