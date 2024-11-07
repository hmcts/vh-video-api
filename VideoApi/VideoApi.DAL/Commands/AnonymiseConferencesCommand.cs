using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;

namespace VideoApi.DAL.Commands
{
    public class AnonymiseConferencesCommand : ICommand
    {
        public int RecordsUpdated { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("csharpsquid", "S1192:String literals should not be duplicated")]
    public class AnonymiseConferencesCommandHandler(VideoApiDbContext context)
        : ICommandHandler<AnonymiseConferencesCommand>
    {
        public async Task Handle(AnonymiseConferencesCommand command)
        {
            const string query = "DECLARE " +
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
            command.RecordsUpdated = await context.Database.ExecuteSqlRawAsync(query);
        }
    }
}
