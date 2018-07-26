using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("Sections")]
    public class Section
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SurveyId { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }

        public int Order { get; set; }

        [ForeignKey("SurveyId")]
        public virtual Survey Survey { get; set; }

        public virtual ICollection<Page> Pages { get; set; } = new HashSet<Page>();

        public virtual ICollection<TestResponse> TestResponses { get; set; } = new HashSet<TestResponse>();
    }
}