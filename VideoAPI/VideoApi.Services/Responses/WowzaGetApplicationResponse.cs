namespace VideoApi.Services.Responses
{
    public class WowzaGetApplicationResponse
    {
        public bool HttpOptimizeFileReads { get; set; }
        public string CaptionLiveIngestType { get; set; }
        public Streamconfig StreamConfig { get; set; }
        public string ServerName { get; set; }
        public string Description { get; set; }
        public Webrtcconfig WebRtcConfig { get; set; }
        public string[] MediaCacheSourceList { get; set; }
        public string RepeaterOriginUrl { get; set; }
        public string ClientStreamReadAccess { get; set; }
        public string AppType { get; set; }
        public int PingTimeout { get; set; }
        public string[] VodTimedTextProviders { get; set; }
        public string[] SaveFieldList { get; set; }
        public string MediaReaderRandomAccessReaderClass { get; set; }
        public bool MediaReaderBufferSeekIo { get; set; }
        public string[] HttpStreamers { get; set; }
        public bool HttpCorsHeadersEnabled { get; set; }
        public string AvSyncMethod { get; set; }
        public Transcoderconfig TranscoderConfig { get; set; }
        public string ClientStreamWriteAccess { get; set; }
        public string RepeaterQueryString { get; set; }
        public Drmconfig DrmConfig { get; set; }
        public string Version { get; set; }
        public Modules Modules { get; set; }
        public int MaxRtcpWaitTime { get; set; }
        public Securityconfig SecurityConfig { get; set; }
        public Dvrconfig DvrConfig { get; set; }
        public int ApplicationTimeout { get; set; }
        public string Name { get; set; }
    }

    public class Streamconfig
    {
        public string StreamType { get; set; }
        public bool StorageDirExists { get; set; }
        public string KeyDir { get; set; }
        public bool CreateStorageDir { get; set; }
        public string[] LiveStreamPacketizer { get; set; }
        public string ServerName { get; set; }
        public string StorageDir { get; set; }
        public string[] SaveFieldList { get; set; }
        public string Version { get; set; }
        public bool HttpRandomizeMediaName { get; set; }
    }

    public class Webrtcconfig
    {
        public string UdpBindAddress { get; set; }
        public bool EnablePlay { get; set; }
        public string PreferredCodecsAudio { get; set; }
        public string PreferredCodecsVideo { get; set; }
        public bool EnableQuery { get; set; }
        public bool DebugLog { get; set; }
        public string ServerName { get; set; }
        public string[] SaveFieldList { get; set; }
        public string IceCandidateIpAddresses { get; set; }
        public string Version { get; set; }
        public bool EnablePublish { get; set; }
    }

    public class Transcoderconfig
    {
        public string ProfileDir { get; set; }
        public bool Licensed { get; set; }
        public TemplateList Templates { get; set; }
        public bool Available { get; set; }
        public string ServerName { get; set; }
        public string TemplateDir { get; set; }
        public string Version { get; set; }
        public bool CreateTemplateDir { get; set; }
        public int Licenses { get; set; }
        public string LiveStreamTranscoder { get; set; }
        public string TemplatesInUse { get; set; }
        public int LicensesInUse { get; set; }
        public string[] SaveFieldList { get; set; }
    }

    public class TemplateList
    {
        public string VhostName { get; set; }
        public Template[] Templates { get; set; }
        public string ServerName { get; set; }
        public string[] SaveFieldList { get; set; }
        public string Version { get; set; }
    }

    public class Template
    {
        public string Id { get; set; }
        public string Href { get; set; }
    }

    public class Drmconfig
    {
        public bool BuyDrmProtectMpegDashStreaming { get; set; }
        public string ServerName { get; set; }
        public bool BuyDrmProtectCupertinoStreaming { get; set; }
        public string Version { get; set; }
        public int VerimatrixCupertinoKeyServerPort { get; set; }
        public int VerimatrixSmoothKeyServerPort { get; set; }
        public Verimatrixstreammaps VerimatrixStreamMaps { get; set; }
        public string VerimatrixSmoothKeyServerIpAddress { get; set; }
        public string LicenseType { get; set; }
        public string VerimatrixCupertinoKeyServerIpAddress { get; set; }
        public bool BuyDrmProtectSmoothStreaming { get; set; }
        public string BuyDrmUserKey { get; set; }
        public bool InUse { get; set; }
        public string EzDrmUsername { get; set; }
        public bool VerimatrixProtectSmoothStreaming { get; set; }
        public Buydrmstreammaps BuyDrmStreamMaps { get; set; }
        public bool VerimatrixCupertinoVodPerSessionKeys { get; set; }
        public string[] SaveFieldList { get; set; }
        public string EzDrmPassword { get; set; }
        public bool VerimatrixProtectCupertinoStreaming { get; set; }
        public bool CupertinoEncryptionApiBased { get; set; }
    }

    public class Verimatrixstreammaps
    {
        public string Filename { get; set; }
        public string ServerName { get; set; }
        public string[] SaveFieldList { get; set; }
        public string Version { get; set; }
        public string[] VerimatrixStreamMaps { get; set; }
    }

    public class Buydrmstreammaps
    {
        public string BuyDrmStreamNameMapFile { get; set; }
        public string ServerName { get; set; }
        public string[] BuyDrmStreamMaps { get; set; }
        public string[] SaveFieldList { get; set; }
        public string Version { get; set; }
    }

    public class Modules
    {
        public Modulelist[] ModuleList { get; set; }
        public string ServerName { get; set; }
        public string[] SaveFieldList { get; set; }
        public string Version { get; set; }
    }

    public class Modulelist
    {
        public string Name { get; set; }
        public string ServerName { get; set; }
        public string Description { get; set; }
        public string[] SaveFieldList { get; set; }
        public string Version { get; set; }
        public string Class { get; set; }
        public int Order { get; set; }
    }

    public class Securityconfig
    {
        public int PlayMaximumConnections { get; set; }
        public bool PublishBlockDuplicateStreamNames { get; set; }
        public string PublishIpWhiteList { get; set; }
        public string PlayAuthenticationMethod { get; set; }
        public string ServerName { get; set; }
        public string ClientStreamWriteAccess { get; set; }
        public string PlayIpWhiteList { get; set; }
        public bool PublishRequirePassword { get; set; }
        public string PlayIpBlackList { get; set; }
        public string Version { get; set; }
        public int SecureTokenVersion { get; set; }
        public string PublishPasswordFile { get; set; }
        public string PublishValidEncoders { get; set; }
        public string SecureTokenQueryParametersPrefix { get; set; }
        public bool SecureTokenUseTeaForRtmp { get; set; }
        public string PublishAuthenticationMethod { get; set; }
        public string SecureTokenHashAlgorithm { get; set; }
        public string PublishIpBlackList { get; set; }
        public bool PlayRequireSecureConnection { get; set; }
        public string SecureTokenOriginSharedSecret { get; set; }
        public string[] SaveFieldList { get; set; }
        public bool SecureTokenIncludeClientIpInHash { get; set; }
        public string PublishRtmpSecureUrl { get; set; }
        public string SecureTokenSharedSecret { get; set; }
    }

    public class Dvrconfig
    {
        public int WindowDuration { get; set; }
        public bool DvrMediaCacheEnabled { get; set; }
        public bool DvrEnable { get; set; }
        public bool StartRecordingOnStartup { get; set; }
        public string ServerName { get; set; }
        public string Store { get; set; }
        public string Version { get; set; }
        public string Recorders { get; set; }
        public bool DvrOnlyStreaming { get; set; }
        public bool HttpRandomizeMediaName { get; set; }
        public string LicenseType { get; set; }
        public string DvrEncryptionSharedSecret { get; set; }
        public bool InUse { get; set; }
        public string ArchiveStrategy { get; set; }
        public string StorageDir { get; set; }
        public string[] SaveFieldList { get; set; }
    }
}
