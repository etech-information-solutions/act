USE [DAPRS]
GO
INSERT [dbo].[MailChimpIntegration] ([Id], [ChimpAPIKey], [ChimpTemplateApprover], [ChimpTemplateDecline], [ChimpTemplateUrgent], [ChimpTemplateUnsuccessful], [ChimpTemplatePayment], [ChimpFromEmail], [ChimpFromName], [ChimpHeader], [ChimpAttachmentPath], [TimeforFundingEmail], [FundingEmail]) VALUES (1, N'2yjmlX2FxL86h575U06IKA', N'DA Approval', N'Decline', N'DA Urgent Approval', N'Failed Payment', N'Notice Payment', N'heather.sadie@agritransact.com', N'Heather Sadie', N'DA Purchase Requisition', N'C:\DA emails\', CAST(N'11:00:00' AS Time), N'h.sadie@etechsolutions.co.za')
GO
