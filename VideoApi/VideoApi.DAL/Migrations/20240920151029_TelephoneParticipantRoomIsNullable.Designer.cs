﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VideoApi.DAL;

#nullable disable

namespace VideoApi.DAL.Migrations
{
    [DbContext(typeof(VideoApiDbContext))]
    [Migration("20240920151029_TelephoneParticipantRoomIsNullable")]
    partial class TelephoneParticipantRoomIsNullable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("VideoApi.Domain.Conference", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ActualStartTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("AudioRecordingRequired")
                        .HasColumnType("bit");

                    b.Property<string>("CaseName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CaseNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CaseType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ClosedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("HearingRefId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("HearingVenueName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IngestUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ScheduledDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("ScheduledDuration")
                        .HasColumnType("int");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<int>("Supplier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Conference", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.ConferenceStatus", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<Guid>("ConferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ConferenceState")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ConferenceId");

                    b.ToTable("ConferenceStatus", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.Endpoint", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ConferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long?>("CurrentConsultationRoomId")
                        .HasColumnType("bigint");

                    b.Property<int?>("CurrentRoom")
                        .HasColumnType("int");

                    b.Property<string>("DefenceAdvocate")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SipAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ConferenceId");

                    b.HasIndex("CurrentConsultationRoomId");

                    b.HasIndex("SipAddress")
                        .IsUnique();

                    b.ToTable("Endpoint", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.Event", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<Guid>("ConferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("EndpointFlag")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<int>("EventType")
                        .HasColumnType("int");

                    b.Property<string>("ExternalEventId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExternalTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long?>("ParticipantRoomId")
                        .HasColumnType("bigint");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TransferredFrom")
                        .HasColumnType("int");

                    b.Property<string>("TransferredFromRoomLabel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TransferredTo")
                        .HasColumnType("int");

                    b.Property<string>("TransferredToRoomLabel")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Event", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.Heartbeat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("BrowserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BrowserVersion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ConferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Device")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IncomingAudioBitrate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IncomingAudioCodec")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("IncomingAudioPacketReceived")
                        .HasColumnType("int");

                    b.Property<int?>("IncomingAudioPacketsLost")
                        .HasColumnType("int");

                    b.Property<decimal>("IncomingAudioPercentageLost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("IncomingAudioPercentageLostRecent")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("IncomingVideoBitrate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IncomingVideoCodec")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("IncomingVideoPacketReceived")
                        .HasColumnType("int");

                    b.Property<int?>("IncomingVideoPacketsLost")
                        .HasColumnType("int");

                    b.Property<decimal>("IncomingVideoPercentageLost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("IncomingVideoPercentageLostRecent")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("IncomingVideoResolution")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OperatingSystem")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OperatingSystemVersion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OutgoingAudioBitrate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OutgoingAudioCodec")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("OutgoingAudioPacketSent")
                        .HasColumnType("int");

                    b.Property<int?>("OutgoingAudioPacketsLost")
                        .HasColumnType("int");

                    b.Property<decimal>("OutgoingAudioPercentageLost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("OutgoingAudioPercentageLostRecent")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("OutgoingVideoBitrate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OutgoingVideoCodec")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("OutgoingVideoFramerate")
                        .HasColumnType("int");

                    b.Property<int?>("OutgoingVideoPacketSent")
                        .HasColumnType("int");

                    b.Property<int?>("OutgoingVideoPacketsLost")
                        .HasColumnType("int");

                    b.Property<decimal>("OutgoingVideoPercentageLost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("OutgoingVideoPercentageLostRecent")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("OutgoingVideoResolution")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Heartbeat", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.InstantMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

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

                    b.ToTable("InstantMessage", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.LinkedParticipant", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("LinkedId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("LinkedId");

                    b.HasIndex("ParticipantId");

                    b.ToTable("LinkedParticipant", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantBase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ConferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long?>("CurrentConsultationRoomId")
                        .HasColumnType("bigint");

                    b.Property<int?>("CurrentRoom")
                        .HasColumnType("int");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("nvarchar(21)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HearingRole")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ParticipantRefId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("State")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<long?>("TestCallResultId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserRole")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ConferenceId");

                    b.HasIndex("CurrentConsultationRoomId");

                    b.HasIndex("TestCallResultId");

                    b.ToTable("Participant", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("ParticipantBase");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantStatus", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ParticipantState")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.ToTable("ParticipantStatus", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Jwt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId")
                        .IsUnique();

                    b.ToTable("ParticipantToken", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.Room", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<Guid>("ConferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("nvarchar(21)");

                    b.Property<string>("Label")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Locked")
                        .HasColumnType("bit");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ConferenceId");

                    b.ToTable("Room", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("Room");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("VideoApi.Domain.RoomEndpoint", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("EndpointId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long?>("RoomId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.ToTable("RoomEndpoint", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.RoomParticipant", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("RoomId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.HasIndex("RoomId");

                    b.ToTable("RoomParticipant", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.Task", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ConferenceId")
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

                    b.ToTable("Task", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.TelephoneParticipant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ConferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CurrentRoom")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int>("State")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<string>("TelephoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ConferenceId");

                    b.ToTable("TelephoneParticipant", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.TestCallResult", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<bool>("Passed")
                        .HasColumnType("bit");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("TestCallResult", (string)null);
                });

            modelBuilder.Entity("VideoApi.Domain.Participant", b =>
                {
                    b.HasBaseType("VideoApi.Domain.ParticipantBase");

                    b.Property<string>("CaseTypeGroup")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactTelephone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Representee")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Participant");
                });

            modelBuilder.Entity("VideoApi.Domain.QuickLinkParticipant", b =>
                {
                    b.HasBaseType("VideoApi.Domain.ParticipantBase");

                    b.HasDiscriminator().HasValue("QuickLinkParticipant");
                });

            modelBuilder.Entity("VideoApi.Domain.ConsultationRoom", b =>
                {
                    b.HasBaseType("VideoApi.Domain.Room");

                    b.HasDiscriminator().HasValue("ConsultationRoom");
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantRoom", b =>
                {
                    b.HasBaseType("VideoApi.Domain.Room");

                    b.Property<string>("IngestUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParticipantUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PexipNode")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("ParticipantRoom");
                });

            modelBuilder.Entity("VideoApi.Domain.Conference", b =>
                {
                    b.OwnsOne("VideoApi.Domain.MeetingRoom", "MeetingRoom", b1 =>
                        {
                            b1.Property<Guid>("ConferenceId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("AdminUri")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("AdminUri");

                            b1.Property<string>("JudgeUri")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("JudgeUri");

                            b1.Property<string>("ParticipantUri")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("ParticipantUri");

                            b1.Property<string>("PexipNode")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("PexipNode");

                            b1.Property<string>("TelephoneConferenceId")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("TelephoneConferenceId");

                            b1.HasKey("ConferenceId");

                            b1.ToTable("Conference");

                            b1.WithOwner()
                                .HasForeignKey("ConferenceId");
                        });

                    b.Navigation("MeetingRoom");
                });

            modelBuilder.Entity("VideoApi.Domain.ConferenceStatus", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference", null)
                        .WithMany("ConferenceStatuses")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VideoApi.Domain.Endpoint", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference", null)
                        .WithMany("Endpoints")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VideoApi.Domain.ConsultationRoom", "CurrentConsultationRoom")
                        .WithMany()
                        .HasForeignKey("CurrentConsultationRoomId");

                    b.Navigation("CurrentConsultationRoom");
                });

            modelBuilder.Entity("VideoApi.Domain.InstantMessage", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference", null)
                        .WithMany("InstantMessageHistory")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VideoApi.Domain.LinkedParticipant", b =>
                {
                    b.HasOne("VideoApi.Domain.ParticipantBase", "Linked")
                        .WithMany()
                        .HasForeignKey("LinkedId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("VideoApi.Domain.ParticipantBase", "Participant")
                        .WithMany("LinkedParticipants")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Linked");

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantBase", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference", null)
                        .WithMany("Participants")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VideoApi.Domain.ConsultationRoom", "CurrentConsultationRoom")
                        .WithMany()
                        .HasForeignKey("CurrentConsultationRoomId");

                    b.HasOne("VideoApi.Domain.TestCallResult", "TestCallResult")
                        .WithMany()
                        .HasForeignKey("TestCallResultId");

                    b.Navigation("CurrentConsultationRoom");

                    b.Navigation("TestCallResult");
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantStatus", b =>
                {
                    b.HasOne("VideoApi.Domain.ParticipantBase", "Participant")
                        .WithMany("ParticipantStatuses")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantToken", b =>
                {
                    b.HasOne("VideoApi.Domain.QuickLinkParticipant", "Participant")
                        .WithOne("Token")
                        .HasForeignKey("VideoApi.Domain.ParticipantToken", "ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("VideoApi.Domain.Room", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference", null)
                        .WithMany("Rooms")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VideoApi.Domain.RoomEndpoint", b =>
                {
                    b.HasOne("VideoApi.Domain.Room", null)
                        .WithMany("RoomEndpoints")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VideoApi.Domain.RoomParticipant", b =>
                {
                    b.HasOne("VideoApi.Domain.ParticipantBase", "Participant")
                        .WithMany("RoomParticipants")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VideoApi.Domain.Room", "Room")
                        .WithMany("RoomParticipants")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Participant");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("VideoApi.Domain.TelephoneParticipant", b =>
                {
                    b.HasOne("VideoApi.Domain.Conference", null)
                        .WithMany("TelephoneParticipants")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VideoApi.Domain.Conference", b =>
                {
                    b.Navigation("ConferenceStatuses");

                    b.Navigation("Endpoints");

                    b.Navigation("InstantMessageHistory");

                    b.Navigation("Participants");

                    b.Navigation("Rooms");

                    b.Navigation("TelephoneParticipants");
                });

            modelBuilder.Entity("VideoApi.Domain.ParticipantBase", b =>
                {
                    b.Navigation("LinkedParticipants");

                    b.Navigation("ParticipantStatuses");

                    b.Navigation("RoomParticipants");
                });

            modelBuilder.Entity("VideoApi.Domain.Room", b =>
                {
                    b.Navigation("RoomEndpoints");

                    b.Navigation("RoomParticipants");
                });

            modelBuilder.Entity("VideoApi.Domain.QuickLinkParticipant", b =>
                {
                    b.Navigation("Token");
                });
#pragma warning restore 612, 618
        }
    }
}
