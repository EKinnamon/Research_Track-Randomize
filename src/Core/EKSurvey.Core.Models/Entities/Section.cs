using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EKSurvey.Core.Models.Entities.Surveys;
using EKSurvey.Core.Models.Enums;

namespace EKSurvey.Core.Models.Entities
{
    [Table("Sections")]
    public class Section
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SurveyId { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        public int Order { get; set; }

        public SelectorType? SelectorType { get; set; }

        public virtual Survey Survey { get; set; }

        public virtual ICollection<Page> Pages { get; set; } = new HashSet<Page>();

        public virtual ICollection<TestSectionMarker> TestSectionMarkers { get; set; } = new HashSet<TestSectionMarker>();

    }
}