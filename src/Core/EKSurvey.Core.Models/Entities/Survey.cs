using System;
using System.Collections.Generic;
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
        [StringLength(256)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [StringLength(64)]
        public string Version { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime Created { get; set; } 

        public DateTime? Modified { get; set; }

        public DateTime? Deleted { get; set; }

        public virtual ICollection<Section> Sections { get; set; } = new HashSet<Section>();

        public virtual ICollection<Test> Tests { get; set; } = new HashSet<Test>();
    }
}
