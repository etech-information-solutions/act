USE [DAPRS]
GO

UPDATE [dbo].[MailChimpIntegration]
   SET [Id] = 1
      ,[ChimpTemplateApprover] = 'DA Approval'
      ,[ChimpTemplateDecline] = 'Decline'
      ,[ChimpTemplateUrgent] = 'DA Urgent Approval'
      ,[ChimpTemplateUnsuccessful] = 'Failed Payment'
      ,[ChimpTemplatePayment] = 'Notice of Payment'
      ,[ChimpFromEmail] = 'heather.sadie@agritransact.com'
      ,[ChimpFromName] = 'Heather007'
      ,[ChimpHeader] = ''
      ,[ChimpAttachmentPath] = ''
      ,[TimeforFundingEmail] = ''
      ,[FundingEmail] = ''
 WHERE ChimpAPIKey = '2yjmlX2FxL86h575U06IKA'
GO


