using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("Surveys")]
    public class Survey
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Section> Sections { get; set; } = new HashSet<Section>();
    }
}
