using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;

namespace VideoApi.DAL.Commands
{
    [Obsolete("Please use AnonymiseConferenceWithHearingIdsCommand")]
    public class AnonymiseConferencesCommand : ICommand
    {
        public int RecordsUpdated { get; set; }
        public AnonymiseConferencesCommand()
        {
        }
    }

    [Obsolete("Please use AnonymiseConferenceWithHearingIdsCommand")]
    public class AnonymiseConferencesCommandHandler : ICommandHandler<AnonymiseConferencesCommand>
    {
        private readonly VideoApiDbContext _context;
        public AnonymiseConferencesCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AnonymiseConferencesCommand command)
        {
            var query = "DECLARE " +
                "@randomString AS VARCHAR(64), " +
                "@months AS INT, " +
                "@anonymiseBeforeDate AS DATETIME, " +
                "@hearingId AS uniqueidentifier, " +
                "@conferenceId AS uniqueidentifier, " +
                "@participantId AS uniqueidentifier, " +
                "@userRole AS INT,  " +
                "@caseTypeGroup AS NVARCHAR(10) " +

                "SET @months = -3 " +
                "SET @anonymiseBeforeDate = (SELECT DATEADD(MONTH, @months, GETUTCDATE())) " +

                "DECLARE conference_cursor CURSOR FOR " +
                "SELECT distinct c.Id FROM [dbo].[Participant] p JOIN [dbo].[Conference] c ON p.ConferenceId = c.Id " +
                "AND [ScheduledDateTime] < @anonymiseBeforeDate AND p.Username not like '%@email.net%' " +
                "OPEN conference_cursor " +
                "FETCH NEXT FROM conference_cursor " +
                "INTO @conferenceId " +

                "WHILE @@FETCH_STATUS = 0 " +
                "BEGIN " +
                    "SELECT @randomString = SUBSTRING(CONVERT(varchar(40), NEWID()), 0, 9); " +
                    "UPDATE [dbo].[Conference] SET CaseName = @randomString WHERE Id = @conferenceId " +

                    "FETCH NEXT FROM conference_cursor " +
                    "INTO @conferenceId " +
                "END " +
                "CLOSE conference_cursor; " +
                "DEALLOCATE conference_cursor; " +

                "DECLARE participant_cursor CURSOR FOR " +
                "SELECT p.Id, p.userrole, p.casetypegroup FROM [dbo].[Participant] p JOIN[dbo].[Conference] c ON p.ConferenceId = c.Id " +
                "AND [ScheduledDateTime] < @anonymiseBeforeDate AND p.Username not like '%@email.net%' " +

                "OPEN participant_cursor " +
                "FETCH NEXT FROM participant_cursor " +
                "INTO @participantId, @userRole, @caseTypeGroup " +

                "WHILE @@FETCH_STATUS = 0 " +
                "BEGIN " +
                    "SELECT @randomString = SUBSTRING(CONVERT(varchar(40), NEWID()), 0, 9); " +
                       " IF @userRole = 4 " +
                            "BEGIN " +
                                "UPDATE [dbo].[Participant] " +
                                "SET " +
                                    "[DisplayName] = @randomString + ' ' + @randomString, " +
                                    "[Username] = @randomString + '@email.net', " +
                                    "[FirstName] = @randomString, " +
                                    "[LastName] = @randomString, " +
                                    "[ContactEmail] = @randomString, " +
                                    "[ContactTelephone] = @randomString, " +
                                    "[Representee] = CASE WHEN Representee = '' THEN '' ELSE @randomString END " +
                               " WHERE Id = @participantId " +
                            "END " +

                        "IF @userRole <> 4 " +
                            "BEGIN " +
                                "UPDATE [dbo].[Participant] " +
                                "SET " +
                                    "[Name] = @randomString, " +
                                    "[DisplayName] = @randomString + ' ' + @randomString, " +
                                    "[Username] = @randomString + '@email.net', " +
                                    "[FirstName] = @randomString, " +
                                    "[LastName] = @randomString, " +
                                    "[ContactEmail] = @randomString, " +
                                    "[ContactTelephone] = @randomString, " +
                                    "[Representee] = CASE WHEN Representee = '' THEN '' ELSE @randomString END " +
                                "WHERE Id = @participantId " +
                            "END " +

                    "FETCH NEXT FROM participant_cursor " +
                    "INTO @participantId, @userRole, @caseTypeGroup " +
                "END " +

                "CLOSE participant_cursor; " +
                "DEALLOCATE participant_cursor;";            
            command.RecordsUpdated = await _context.Database.ExecuteSqlRawAsync(query);
        }
    }
}
