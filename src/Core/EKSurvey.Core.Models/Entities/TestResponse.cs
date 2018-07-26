using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("TestResponses")]
    public class TestResponse
    {
        [Key, Column(Order=0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid TestId { get; set; }

        [Key, Column(Order=1), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SectionId { get; set; }

        [Key, Column(Order=2), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PageId { get; set; }

        public string Response { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }

        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }

        [ForeignKey("PageId")]
        public virtual Page Page { get; set; }
    }
}