using System;

namespace EKSurvey.Core.Models.DataTransfer
{
    public class UserSurvey
    {
        public string UserId { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public bool IsActive { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Completed { get; set; }

    }
}
