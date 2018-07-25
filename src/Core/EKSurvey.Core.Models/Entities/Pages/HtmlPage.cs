using System.Web.Mvc;

namespace EKSurvey.Core.Models.Entities
{
    public class HtmlPage : Page
    {
        public MvcHtmlString Text { get; set; }
    }
}