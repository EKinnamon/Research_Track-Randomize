using System.Collections.Generic;

namespace EKSurvey.Core.Models.Entities
{
    public class Questionnaire : Section
    {
        public virtual ICollection<Question> Questions { get; set; } = new HashSet<Question>();
    }
}