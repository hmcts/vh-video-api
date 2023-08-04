SET XACT_ABORT ON;
GO;
BEGIN TRANSACTION;

SELECT DISTINCT HearingVenueName from Conference
GO;

CREATE OR ALTER PROC #Conference_UpdateHearingVenueName @oldVenueName nvarchar(max), @newVenueName nvarchar(max)
As
BEGIN
    IF EXISTS (SELECT * FROM dbo.Conference WHERE HearingVenueName = TRIM(@oldVenueName))
        BEGIN
            Print ('FOUND venue with the name: ' + @oldVenueName);
            Update Conference Set HearingVenueName = @newVenueName Where HearingVenueName = @oldVenueName;
        END
    ELSE
        BEGIN
            Print ('WARNING! Could not find venue with the name: ' + @oldVenueName);
        END
END
GO;

EXEC #Conference_UpdateHearingVenueName @oldVenueName='Ayr', @newVenueName='Ayr Social Security and Child Support Tribunal';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Bath Law Courts (Civil and Family)', @newVenueName='Bath Magistrates Court and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Birkenhead County Court', @newVenueName='Birkenhead County Court and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Birmingham Employment Tribunal', @newVenueName='Centre City Tower';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Blackburn Family Court', @newVenueName='Blackburn County Court and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Blackpool Family Court', @newVenueName='Blackpool County Court and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Bodmin Law Courts', @newVenueName='Bodmin County Court and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Bolton Crown Court', @newVenueName='Bolton Combined Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Bridlington Magistrates Court and Hearing Centre', @newVenueName='Bridlington Magistrates Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Brighton County Court', @newVenueName='Brighton County and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Brighton Social Security and Child Support Tribunal', @newVenueName='Brighton Tribunal Hearing Centre';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Bristol Magistrates Court and Tribunals Hearing Centre', @newVenueName='Bristol Magistrates Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Carmarthen County Court and Family Court', @newVenueName='Carmarthen County Court and Tribunal Hearing Centre';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Central Family Court', @newVenueName='Central Family Court (First Avenue House)';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Chelmsford Justice Centre', @newVenueName='Chelmsford County and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Chesterfield Justice Centre', @newVenueName='Chesterfield Magistrates';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Crewe Magistrates Court', @newVenueName='Crewe (South Cheshire) Magistrates Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Derby Magistrates Court', @newVenueName='Derby Magistrates';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Dundee Tribunal Hearing Centre', @newVenueName='Dundee Tribunal Hearing Centre - Endeavour House';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Durham County Court and Family Court', @newVenueName='Durham Justice Centre';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='East Berkshire Magistrates Court', @newVenueName='East Berkshire Magistrates Court, Maidenhead';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Exeter Law Courts', @newVenueName='Exeter Combined Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Hull and Holderness Magistrates Court and Hearing Centre', @newVenueName='Hull Magistrates Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Lavender Hill Magistrates Court', @newVenueName='Lavender Hill Magistrates Court (Formerly South Western Magistrates Court)';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Leicester County Court and Family Court', @newVenueName='Leicester County Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Leyland Family Hearing Centre', @newVenueName='Leyland Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Luton and South Bedfordshire Magistrates Court', @newVenueName='Luton and South Bedfordshire Magistrates Court and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Maidstone Magistrates Court', @newVenueName='Maidstone Magistrates Court and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Manchester Civil and Family Justice Centre', @newVenueName='Manchester County and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Manchester Employment Tribunal', @newVenueName='Manchester Tribunal Hearing Centre - Alexandra House';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Manchester Tribunal Hearing Centre', @newVenueName='Manchester Tribunal Hearing Centre - Piccadilly Exchange';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Medway Magistrates Court and Family Court', @newVenueName='Medway Magistrates Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Mold Justice Centre (Mold Law Courts)', @newVenueName='Mold Justice Centre';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Newton Aycliffe Magistrates Court', @newVenueName='Newton Aycliffe Magistrates Court and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Northampton Crown, County and Family Court', @newVenueName='Northampton Crown Court, County Court and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Oxford Magistrates Court', @newVenueName='Oxford and Southern Oxfordshire Magistrates Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Plymouth (St Catherine''s House)', @newVenueName='Plymouth As St Catherine''s House';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Preston Crown Court and Family Court (Sessions House)', @newVenueName='Preston Crown Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Reedley Family Hearing Centre', @newVenueName='Reedley Magistrates Court and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Sheffield Designated Family Court', @newVenueName='Sheffield Family Hearing Centre';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Shrewsbury Crown Court', @newVenueName='Shrewsbury Justice Centre';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Skipton County Court and Family Court', @newVenueName='Skipton Magistrates and County Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='South Tyneside County Court and Family Court', @newVenueName='South Tyneside Magistrates Court and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Southampton Hearing Centre', @newVenueName='Southampton Combined Court Centre';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Southend Crown Court', @newVenueName='Southend Combined - Crown, Mags, County and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Southern Property Tribunal', @newVenueName='Southern House';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Stirling Wallace House', @newVenueName='Stirling Tribunal Hearing Centre';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Stockport Magistrates Court', @newVenueName='Stockport Magistrates Court and Famiy Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Swansea Civil Justice Centre', @newVenueName='Swansea Civil and Family Justice Centre';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Taunton Combined Court', @newVenueName='Taunton Crown, County and Family Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='West Hampshire (Southampton) Magistrates Court', @newVenueName='West Hampshire Magistrates Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Wolverhampton Social Security and Child Support Tribunal', @newVenueName='Wolverhampton Ast - Norwich Union House, Wolverhampton';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Worcester Justice Centre', @newVenueName='Worcester Magistrates Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Worthing County Court and Family Court', @newVenueName='Worthing Magistrates and County Court';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Wrexham County and Family Court', @newVenueName='Wrexham Law Courts';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='King''s Lynn Crown Court', @newVenueName='King''s Lynn Crown Court (& Magistrates)';
EXEC #Conference_UpdateHearingVenueName @oldVenueName='Hereford Magistrates Court', @newVenueName='Hereford Justice Centre';
GO;

SELECT DISTINCT HearingVenueName from Conference
GO;

COMMIT;
SET XACT_ABORT OFF
GO;