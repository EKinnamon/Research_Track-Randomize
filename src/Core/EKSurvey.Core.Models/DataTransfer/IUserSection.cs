using System;

namespace EKSurvey.Core.Models.DataTransfer
{
    public interface IUserSection
    {
        string UserId { get; }

        int SurveyId { get; }

        Guid? TestId { get; }

        Guid? TestSectionMarkerId { get; }

        int? Id { get; set; }

        int Order { get; }

        DateTime? Started { get; }

        DateTime? Modified { get; }

        DateTime? Completed { get; }

    }
}