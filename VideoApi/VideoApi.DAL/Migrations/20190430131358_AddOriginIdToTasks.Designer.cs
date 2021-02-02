﻿// <auto-generated />
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
    [Migration("20190430131358_AddOriginIdToTasks")]
    partial class AddOriginIdToTasks
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
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

                    b.Property<int>("ScheduledDuration");

                    b.Property<int>("State");

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

            modelBuilder.Entity("VideoApi.Domain.Task", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body");

                    b.Property<Guid?>("ConferenceId");

                    b.Property<DateTime>("Created");

                    b.Property<Guid>("OriginId");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("ConferenceId");

                    b.ToTable("Task");
                });

            modelBuilder.Entity("VideoApi.Domain.Conference", b =>
                {
                    b.OwnsOne("VideoApi.Domain.MeetingRoom", "MeetingRoom", b1 =>
                        {
                            b1.Property<Guid>("ConferenceId");

                            b1.Property<string>("AdminUri")
                                .HasColumnName("AdminUri");

                            b1.Property<string>("JudgeUri")
                                .HasColumnName("JudgeUri");

                            b1.Property<string>("ParticipantUri")
                                .HasColumnName("ParticipantUri");

                            b1.Property<string>("PexipNode")
                                .HasColumnName("PexipNode");

                            b1.HasKey("ConferenceId");

                            b1.ToTable("Conference");

                            b1.HasOne("VideoApi.Domain.Conference")
                                .WithOne("MeetingRoom")
                                .HasForeignKey("VideoApi.Domain.MeetingRoom", "ConferenceId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("VideoApi.Domain.ConferenceStatus", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference")
                        .WithMany("ConferenceStatuses")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VideoApi.Domain.Participant", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference")
                        .WithMany("Participants")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantStatus", b =>
                {
                    b.HasOne("VideoApi.Domain.Participant")
                        .WithMany("ParticipantStatuses")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VideoApi.Domain.Task", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference")
                        .WithMany("Tasks")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}