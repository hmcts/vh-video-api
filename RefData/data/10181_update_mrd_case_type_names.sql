SET XACT_ABORT ON;
GO;

BEGIN TRANSACTION

--family
UPDATE Conference SET CaseType = 'Family Public Law' WHERE CaseType = 'Public Law - Care'
UPDATE Conference SET CaseType = 'Family Private Law' WHERE CaseType = 'Private Law'
--civil
UPDATE Conference SET CaseType = 'Specified Money Claims' WHERE CaseType = 'Civil Money Claims'
--tribunal
UPDATE Conference SET CaseType = 'Environment' WHERE CaseType = 'GRC - Environment'
UPDATE Conference SET CaseType = 'Estate Agents' WHERE CaseType = 'GRC - Estate Agents'
UPDATE Conference SET CaseType = 'Immigration Services' WHERE CaseType = 'GRC - Immigration Services'
UPDATE Conference SET CaseType = 'Gambling' WHERE CaseType = 'GRC - Gambling'
UPDATE Conference SET CaseType = 'Information Rights' WHERE CaseType = 'GRC - Information Rights'
UPDATE Conference SET CaseType = 'Transport' WHERE CaseType = 'GRC - DVSA'
UPDATE Conference SET CaseType = 'Community Right to Bid' WHERE CaseType = 'GRC - CRB'
UPDATE Conference SET CaseType = 'Food Safety' WHERE CaseType = 'GRC - Food'
UPDATE Conference SET CaseType = 'Pensions' WHERE CaseType = 'GRC - Pensions Regulation'
UPDATE Conference SET CaseType = 'Welfare of Animals' WHERE CaseType = 'GRC - Welfare of Animals'
UPDATE Conference SET CaseType = 'Social Security and Child Support' WHERE CaseType = 'SSCS Tribunal'
UPDATE Conference SET CaseType = 'Special Educational Needs' WHERE CaseType = 'Special Educational Needs and Disability'
UPDATE Conference SET CaseType = 'Tax Appeals' WHERE CaseType = 'Tax'
UPDATE Conference SET CaseType = 'Immigration and Asylum Appeals' WHERE CaseType = 'Immigration and Asylum'
UPDATE Conference SET CaseType = 'Employment Claims' WHERE CaseType = 'Employment Tribunal'
COMMIT
SET XACT_ABORT OFF
GO;