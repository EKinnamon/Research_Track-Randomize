using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("Tests")]
    public class Test
    {
        public Guid Id { get; set; }

        [Key, Column(Order=0), StringLength(128)]
        public string UserId { get; set; }

        [Key, Column(Order=1)]
        public int SurveyId { get; set; }

        public DateTime Started { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Completed { get; set; }

        [ForeignKey("SurveyId")]
        public virtual Survey Survey { get; set; }

        public virtual ICollection<TestSectionMarker> TestSectionMarkers { get; set; } = new HashSet<TestSectionMarker>();

        public virtual ICollection<TestResponse> TestResponses { get; set; } = new HashSet<TestResponse>();
    }
}
