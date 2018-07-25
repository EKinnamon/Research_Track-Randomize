using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("Tests")]
    public class Test
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public int SurveyId { get; set; }

        public DateTime Started { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Completed { get; set; }

        [ForeignKey("SurveyId")]
        public virtual Survey Survey { get; set; }
    }
}
