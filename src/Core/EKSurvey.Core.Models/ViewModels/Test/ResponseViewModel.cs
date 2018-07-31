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

        private string _pageTypeName;
        public string PageTypeName
        {
            get => Page != null ? PageType.FullName : _pageTypeName;
            set => _pageTypeName = value;

        }

        public Type PageType
        {
            get
            {
                if (Page != null)
                    return Page.GetType().BaseType ?? Page.GetType();

                return !string.IsNullOrWhiteSpace(PageTypeName) 
                    ? Type.GetType(PageTypeName) 
                    : null;
            } 
        }

        public IPage Page { get; set; }
    }
}
