using System.Collections.Generic;

namespace EKSurvey.Core.Models.Entities
{
    public interface ISection
    {
        int Id { get; set; }
        int SurveyId { get; set; }
        string Name { get; set; }
        Survey Survey { get; set; }
        ICollection<Page> Pages { get; set; }
    }
}