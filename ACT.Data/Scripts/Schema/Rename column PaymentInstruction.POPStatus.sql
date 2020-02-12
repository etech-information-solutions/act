use DAPRS
go
Alter table PaymentInstructions drop column POPStatus
Alter table PaymentInstructions add POPType int

Alter table PaymentInstructions add PaymentType int