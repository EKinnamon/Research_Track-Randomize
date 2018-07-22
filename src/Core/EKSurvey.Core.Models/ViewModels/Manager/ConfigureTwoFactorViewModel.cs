using System.Collections.Generic;
using System.Web.Mvc;

namespace EKSurvey.Core.Models.ViewModels.Manager
{
    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; }
    }
}