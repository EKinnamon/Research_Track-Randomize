using System;
using System.Web.Mvc;
using EKSurvey.Core.Models.DataTransfer;

namespace EKSurvey.UI.Extensions
{
    public static class PageExtensions
    {
        public static MvcHtmlString PageUserResponse(this UserResponse response)
        {
            if (!response.IsQuestion || !response.IsLikert)
                return new MvcHtmlString(response.Response);

            var likertBaseCount = Core.Constants.LikertScaleValues.Count;
            var scaler = (response.Range.GetValueOrDefault(likertBaseCount) - likertBaseCount) >> 1;
            var responseValue = Convert.ToInt32(response.Response);
            var convertedValueRaw = responseValue / (float) scaler;
            var convertedValue = (int) (responseValue >= 0 
                ? Math.Ceiling(convertedValueRaw) 
                : Math.Floor(convertedValueRaw));

            var index = convertedValue + (int) Math.Floor(likertBaseCount / 2.0);

            var responseString = $"{response.Response} ( {Core.Constants.LikertScaleValues[index]} )";

            return new MvcHtmlString(responseString);

        }
    }
}