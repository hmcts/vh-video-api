using System;

namespace VideoApi.Extensions
{
    public static class ContractExtensions
    {
        public static Contract.Enums.ConferenceState MapToContractEnum(this Domain.Enums.ConferenceState status)
        {
            return Enum.Parse<Contract.Enums.ConferenceState>(status.ToString());
        }
        
        public static Contract.Enums.ParticipantState MapToContractEnum(this Domain.Enums.ParticipantState status)
        {
            return Enum.Parse<Contract.Enums.ParticipantState>(status.ToString());
        }
        
        public static Contract.Enums.UserRole MapToContractEnum(this Domain.Enums.UserRole role)
        {
            return Enum.Parse<Contract.Enums.UserRole>(role.ToString());
        }
        
        public static Contract.Enums.EndpointState MapToContractEnum(this Domain.Enums.EndpointState status)
        {
            return Enum.Parse<Contract.Enums.EndpointState>(status.ToString());
        }
        
        public static Contract.Enums.TestScore MapToContractEnum(this Domain.Enums.TestScore score)
        {
            return Enum.Parse<Contract.Enums.TestScore>(score.ToString());
        }
        
        public static Contract.Enums.TaskType MapToContractEnum(this Domain.Enums.TaskType taskType)
        {
            return Enum.Parse<Contract.Enums.TaskType>(taskType.ToString());
        }
        
        public static Contract.Enums.TaskStatus MapToContractEnum(this Domain.Enums.TaskStatus taskStatus)
        {
            return Enum.Parse<Contract.Enums.TaskStatus>(taskStatus.ToString());
        }
        
        public static Contract.Enums.LinkedParticipantType MapToContractEnum(this Domain.Enums.LinkedParticipantType taskType)
        {
            return Enum.Parse<Contract.Enums.LinkedParticipantType>(taskType.ToString());
        }
        
        public static Contract.Enums.RoomType MapToContractEnum(this Domain.Enums.RoomType roomType)
        {
            return Enum.Parse<Contract.Enums.RoomType>(roomType.ToString());
        }

        public static Domain.Enums.UserRole MapToDomainEnum(this Contract.Enums.UserRole role)
        {
            return Enum.Parse<Domain.Enums.UserRole>(role.ToString());
        }
        
        public static Domain.Enums.ConferenceRole MapToDomainEnum(this Contract.Enums.ConferenceRole role)
        {
            return Enum.Parse<Domain.Enums.ConferenceRole>(role.ToString());
        }
        
        public static Domain.Enums.EventType MapToDomainEnum(this Contract.Enums.EventType eventType)
        {
            return Enum.Parse<Domain.Enums.EventType>(eventType.ToString());
        }
        
        public static Domain.Enums.ConferenceRoomType MapToDomainEnum(this Contract.Enums.ConferenceRoomType roomType)
        {
            return Enum.Parse<Domain.Enums.ConferenceRoomType>(roomType.ToString());
        }
        
        public static Domain.Enums.AudioPlaybackLanguage MapToDomainEnum(this Contract.Enums.AudioPlaybackLanguage audioRecordingStatus)
        {
            return Enum.Parse<Domain.Enums.AudioPlaybackLanguage>(audioRecordingStatus.ToString());
        }
        
        public static Domain.Enums.Supplier MapToDomainEnum(this Contract.Enums.Supplier supplier)
        {
            return Enum.Parse<Domain.Enums.Supplier>(supplier.ToString());
        }
        
        public static Domain.Enums.VirtualCourtRoomType MapToDomainEnum(this Contract.Enums.VirtualCourtRoomType roomType)
        {
            return Enum.Parse<Domain.Enums.VirtualCourtRoomType>(roomType.ToString());
        }

        public static Domain.Enums.TaskType MapToDomainEnum(this Contract.Enums.TaskType taskType)
        {
            return Enum.Parse<Domain.Enums.TaskType>(taskType.ToString());
        }
        
        public static Domain.Enums.LinkedParticipantType MapToDomainEnum(this Contract.Enums.LinkedParticipantType taskType)
        {
            return Enum.Parse<Domain.Enums.LinkedParticipantType>(taskType.ToString());
        }
    }
}
