using System;

namespace EKSurvey.Core.Models.Exceptions
{
    public class InvalidSectionGroupConfigurationException : Exception
    {
        public InvalidSectionGroupConfigurationException() : base() { }

        public InvalidSectionGroupConfigurationException(string message) : base(message) { }

        public InvalidSectionGroupConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
