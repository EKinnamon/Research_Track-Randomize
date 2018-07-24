namespace EKSurvey.Core.Models.Entities
{
    public interface IQuestion
    {
        int Id { get; set; }
        int Order { get; set; }
        string Text { get; set; }
    }
}