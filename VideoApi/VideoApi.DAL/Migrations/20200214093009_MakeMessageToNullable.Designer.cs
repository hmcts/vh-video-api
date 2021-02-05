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
    [Migration("20200214093009_MakeMessageToNullable")]
    partial class MakeMessageToNullable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VideoApi.Domain.Conference", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CaseName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CaseNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CaseType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ClosedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("HearingRefId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("HearingVenueName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ScheduledDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("ScheduledDuration")
                        .HasColumnType("int");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Conference");
                });

            modelBuilder.Entity("VideoApi.Domain.ConferenceStatus", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid?>("ConferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ConferenceState")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ConferenceId");

                    b.ToTable("ConferenceStatus");
                });

            modelBuilder.Entity("VideoApi.Domain.Event", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("ConferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("EventType")
                        .HasColumnType("int");

                    b.Property<string>("ExternalEventId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExternalTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TransferredFrom")
                        .HasColumnType("int");

                    b.Property<int?>("TransferredTo")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("VideoApi.Domain.Message", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("ConferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MessageText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("To")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ConferenceId");

                    b.HasIndex("TimeStamp");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("VideoApi.Domain.Participant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CaseTypeGroup")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ConferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("CurrentRoom")
                        .HasColumnType("int");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ParticipantRefId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Representee")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("TestCallResultId")
                        .HasColumnType("bigint");

                    b.Property<int>("UserRole")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ConferenceId");

                    b.HasIndex("TestCallResultId");

                    b.ToTable("Participant");
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantStatus", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid?>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ParticipantState")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.ToTable("ParticipantStatus");
                });

            modelBuilder.Entity("VideoApi.Domain.Task", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ConferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OriginId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ConferenceId");

                    b.ToTable("Task");
                });

            modelBuilder.Entity("VideoApi.Domain.TestCallResult", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Passed")
                        .HasColumnType("bit");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TestCallResult");
                });

            modelBuilder.Entity("VideoApi.Domain.Conference", b =>
                {
                    b.OwnsOne("VideoApi.Domain.MeetingRoom", "MeetingRoom", b1 =>
                        {
                            b1.Property<Guid>("ConferenceId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("AdminUri")
                                .HasColumnName("AdminUri")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("JudgeUri")
                                .HasColumnName("JudgeUri")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ParticipantUri")
                                .HasColumnName("ParticipantUri")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PexipNode")
                                .HasColumnName("PexipNode")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ConferenceId");

                            b1.ToTable("Conference");

                            b1.WithOwner()
                                .HasForeignKey("ConferenceId");
                        });
                });

            modelBuilder.Entity("VideoApi.Domain.ConferenceStatus", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference", null)
                        .WithMany("ConferenceStatuses")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VideoApi.Domain.Message", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference", null)
                        .WithMany("Messages")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VideoApi.Domain.Participant", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference", null)
                        .WithMany("Participants")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VideoApi.Domain.TestCallResult", "TestCallResult")
                        .WithMany()
                        .HasForeignKey("TestCallResultId");
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantStatus", b =>
                {
                    b.HasOne("VideoApi.Domain.Participant", null)
                        .WithMany("ParticipantStatuses")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VideoApi.Domain.Task", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference", null)
                        .WithMany("Tasks")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
