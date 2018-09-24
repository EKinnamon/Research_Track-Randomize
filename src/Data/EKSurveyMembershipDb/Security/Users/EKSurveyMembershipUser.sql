CREATE USER [EKSurveyMembershipUser]
	FROM LOGIN [EKSurveyMembershipUser]
	WITH DEFAULT_SCHEMA = dbo

GO

GRANT CONNECT TO [EKSurveyMembershipUser]

GO

ALTER ROLE [db_owner] ADD MEMBER [EKSurveyMembershipUser]
