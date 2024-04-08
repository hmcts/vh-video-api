using VideoApi.Common.Security.Supplier.Kinly;

namespace VideoApi.Common.Security.HashGen;

public class HashGeneratorKinly(KinlyConfiguration config) 
    :  HashGeneratorBase(config);
