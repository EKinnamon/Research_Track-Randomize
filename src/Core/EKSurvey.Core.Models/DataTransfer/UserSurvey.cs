using System;

namespace EKSurvey.Core.Models.DataTransfer
{
    public class UserSurvey
    {
        public string UserId { get; set; }

        public int Id { get; set; }

        public Guid TestId { get; set; }

        public string Name { get; set; }

        private string _url;

        public string Url
        {
            get => IsMonkeySurvey ? _url : null;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _url = null;
                    IsMonkeySurvey = false;
                }

                _url = value;
                IsMonkeySurvey = true;
            }
        }

        public string Description { get; set; }

        public string Version { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }

        public bool IsMonkeySurvey { get; set; }
        public bool IsStarted => Started.HasValue;
        public bool IsCompleted => Completed.HasValue;
    }
}
