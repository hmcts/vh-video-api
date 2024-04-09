using VideoApi.Common.Security.Supplier.Base;

namespace VideoApi.Common.Security.Supplier.Kinly;

public interface IKinlyJwtTokenHandler: ICustomJwtTokenHandler{}

public class KinlyJwtHandler(KinlyConfiguration config) : CustomJwtTokenHandler(config), IKinlyJwtTokenHandler{}
