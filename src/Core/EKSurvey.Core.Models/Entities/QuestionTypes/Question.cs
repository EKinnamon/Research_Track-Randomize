using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("Questions")]
    public abstract class Question : IQuestion
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Text { get; set; }
    }
}