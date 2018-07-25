using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("Sections")]
    public class Section : ISection
    {
        public int Id { get; set; }

        public int SurveyId { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        [ForeignKey("SurveyId")]
        public virtual Survey Survey { get; set; }

        public virtual ICollection<Page> Pages { get; set; }
    }
}