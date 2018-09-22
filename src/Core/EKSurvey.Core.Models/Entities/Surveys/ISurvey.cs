using System;
using System.Collections.Generic;

namespace EKSurvey.Core.Models.Entities.Surveys
{
    public interface ISurvey
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string Version { get; set; }
        bool IsActive { get; set; }
        DateTime Created { get; set; }
        DateTime? Modified { get; set; }
        DateTime? Deleted { get; set; }
        ICollection<Test> Tests { get; set; }
    }
}