using VideoApi.Common.Security.Supplier.Base;

namespace VideoApi.Common.Security.Supplier.Kinly;

public interface IKinlyJwtTokenHandler: ICustomJwtTokenHandler{}

public class KinlyJwtHandler : CustomJwtTokenHandler, IKinlyJwtTokenHandler
{
    public KinlyJwtHandler(KinlyConfiguration config) : base(config)
    {
    }
}
