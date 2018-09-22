using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("TestResponses")]
    public class TestResponse
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid TestId { get; set; }

        public int PageId { get; set; }

        public string Response { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Responded { get; set; }

        [ForeignKey("PageId")]
        public virtual Page Page { get; set; }

        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
    }
}