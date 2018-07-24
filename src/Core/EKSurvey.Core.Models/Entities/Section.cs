using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("Sections")]
    public abstract class Section : ISection
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Survey> Surveys { get; set; } = new HashSet<Survey>();
    }
}