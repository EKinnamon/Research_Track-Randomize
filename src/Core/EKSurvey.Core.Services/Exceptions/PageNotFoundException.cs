using System;

namespace EKSurvey.Core.Services.Exceptions
{
    public class PageNotFoundException : Exception
    {
        public int PageId { get; set; }

        public PageNotFoundException(int pageId) : base($"Page `{pageId}` could not be found.")
        {
            PageId = pageId;
        }

        public PageNotFoundException(int pageId, string message) : base(message)
        {
            PageId = pageId;
        }

        public PageNotFoundException(int pageId, string message, Exception innerException) : base(message, innerException)
        {
            PageId = pageId;
        }
    }
}
