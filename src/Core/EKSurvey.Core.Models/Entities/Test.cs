using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EKSurvey.Core.Models.Entities.Surveys;

namespace EKSurvey.Core.Models.Entities
{
    [Table("Tests")]
    public class Test
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [NotMapped]
        public string UserIdSimple => UserId.Replace("-", string.Empty);

        public int SurveyId { get; set; }

        public DateTime Started { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Completed { get; set; }

        public virtual Survey Survey { get; set; }

        public virtual ICollection<TestResponse> TestResponses { get; set; } = new HashSet<TestResponse>();

        public virtual ICollection<TestSectionMarker> TestSectionMarkers { get; set; } = new HashSet<TestSectionMarker>();
    }
}
