using System.Collections.Generic;

namespace EKSurvey.Core.Models.ViewModels.Manager
{
    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}