using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("TestSectionMarkers")]
    public class TestSectionMarker
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid TestId { get; set; }
        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SectionId { get; set; }
        public DateTime Started { get; set; }
        public DateTime? Completed { get; set; }
        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }
    }
}
