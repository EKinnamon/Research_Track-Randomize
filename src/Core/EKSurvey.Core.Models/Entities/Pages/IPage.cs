namespace EKSurvey.Core.Models.Entities
{
    public interface IPage
    {
        int Id { get; set; }
        int SectionId { get; set; }
        int Order { get; set; }
        bool IsHtml { get; set; }
        string Text { get; set; }
    }
}