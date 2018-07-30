using System;
using System.ComponentModel.DataAnnotations;
using EKSurvey.Core.Models.Attributes;
using EKSurvey.Core.Models.Entities;

namespace EKSurvey.Core.Models.ViewModels.Test
{
    public class ResponseViewModel
    {
        public string UserId { get; set; }

        [Required]
        public int SurveyId { get; set; }

        [Required]
        public Guid TestId { get; set; }

        public int PageId { get; set; }

        [ResponseRequired("Page")]
        public string Response { get; set; }

        public int? PriorPageId { get; set; }

        private string _pageType;
        public string PageType
        {
            get => Page != null 
                ? Page.GetType().BaseType?.ToString() 
                : _pageType;

            set => _pageType = value;
        }

        public IPage Page { get; set; }
    }
}
