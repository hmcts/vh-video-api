using VideoApi.Common.Security.Supplier.Kinly;

namespace VideoApi.Common.Security.HashGen;

public class HashGeneratorKinly : HashGeneratorBase
{
    public HashGeneratorKinly(KinlyConfiguration config) : base(config)
    {
    }
}
