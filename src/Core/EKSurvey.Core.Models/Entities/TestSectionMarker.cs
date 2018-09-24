using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("TestSectionMarkers")]
    public class TestSectionMarker
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid TestId { get; set; }

        public int SectionId { get; set; }

        public DateTime Started { get; set; }

        public DateTime? Completed { get; set; }

        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }

        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
    }
}
