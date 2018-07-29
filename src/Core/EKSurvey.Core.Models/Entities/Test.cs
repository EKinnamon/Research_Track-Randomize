using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("Tests")]
    public class Test
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid Id { get; set; }

        [Key, Column(Order=0), Required, StringLength(128)]
        public string UserId { get; set; }
        [Key, Column(Order=1), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SurveyId { get; set; }

        public DateTime Started { get; set; }

        public DateTime? Modified { get; set; }

        public DateTime? Completed { get; set; }

        public virtual Survey Survey { get; set; }
    }
}
