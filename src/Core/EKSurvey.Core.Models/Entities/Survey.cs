using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("Surveys")]
    public class Survey
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Version { get; set; }

        [DefaultValue("true")]
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Section> Sections { get; set; } = new HashSet<Section>();
    }
}
