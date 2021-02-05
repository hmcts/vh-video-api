// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VideoApi.DAL;

namespace VideoApi.DAL.Migrations
{
    [DbContext(typeof(VideoApiDbContext))]
    [Migration("20190313195756_AddCaseNameToConference")]
    partial class AddCaseNameToConference
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VideoApi.Domain.Conference", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CaseName");

                    b.Property<string>("CaseNumber");

                    b.Property<string>("CaseType");

                    b.Property<Guid>("HearingRefId");

                    b.Property<DateTime>("ScheduledDateTime");

                    b.HasKey("Id");

                    b.ToTable("Conference");
                });

            modelBuilder.Entity("VideoApi.Domain.ConferenceStatus", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid?>("ConferenceId");

                    b.Property<int>("ConferenceState");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("Id");

                    b.HasIndex("ConferenceId");

                    b.ToTable("ConferenceStatus");
                });

            modelBuilder.Entity("VideoApi.Domain.Event", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("ConferenceId");

                    b.Property<int>("EventType");

                    b.Property<string>("ExternalEventId");

                    b.Property<DateTime>("ExternalTimestamp");

                    b.Property<Guid>("ParticipantId");

                    b.Property<string>("Reason");

                    b.Property<DateTime>("Timestamp");

                    b.Property<int?>("TransferredFrom");

                    b.Property<int?>("TransferredTo");

                    b.HasKey("Id");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("VideoApi.Domain.Participant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CaseTypeGroup");

                    b.Property<Guid?>("ConferenceId");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Name");

                    b.Property<Guid>("ParticipantRefId");

                    b.Property<int>("UserRole");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.HasIndex("ConferenceId");

                    b.ToTable("Participant");
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantStatus", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid?>("ParticipantId");

                    b.Property<int>("ParticipantState");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.ToTable("ParticipantStatus");
                });

            modelBuilder.Entity("VideoApi.Domain.ConferenceStatus", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference")
                        .WithMany("ConferenceStatuses")
                        .HasForeignKey("ConferenceId");
                });

            modelBuilder.Entity("VideoApi.Domain.Participant", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference")
                        .WithMany("Participants")
                        .HasForeignKey("ConferenceId");
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantStatus", b =>
                {
                    b.HasOne("VideoApi.Domain.Participant")
                        .WithMany("ParticipantStatuses")
                        .HasForeignKey("ParticipantId");
                });
#pragma warning restore 612, 618
        }
    }
}
