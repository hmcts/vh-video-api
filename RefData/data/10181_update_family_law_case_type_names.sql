SET XACT_ABORT ON;
GO;

BEGIN TRANSACTION

UPDATE Conference SET CaseType = 'Family Public Law' WHERE CaseType = 'Public Law - Care'
UPDATE Conference SET CaseType = 'Family Private Law' WHERE CaseType = 'Private Law'

SELECT * FROM Conference WHERE CaseType = 'Family Public Law' OR  CaseType = 'Family Private Law'

COMMIT
SET XACT_ABORT OFF
GO;