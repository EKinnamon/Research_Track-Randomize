using System.Collections.Generic;

namespace EKSurvey.Core.Models.Entities
{
    public interface ISection
    {
        int Id { get; set; }

        string Name { get; set; }

        ICollection<Survey> Surveys { get; set; }
    }
}