namespace EKSurvey.Core.Models.Entities
{
    public interface IQuestion
    {
        int Id { get; set; }
        int SectionId { get; set; }
        int Order { get; set; }
    }
}