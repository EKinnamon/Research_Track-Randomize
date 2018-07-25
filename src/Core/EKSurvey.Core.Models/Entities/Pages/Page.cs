using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("Pages")]
    public abstract class Page : IPage
    {
        [Key]
        public int Id { get; set; }

        public int SectionId { get; set; }

        public int Order { get; set; }

        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }

        public virtual ICollection<TestResponse> TestResponses { get; set; } = new HashSet<TestResponse>();
    }
}