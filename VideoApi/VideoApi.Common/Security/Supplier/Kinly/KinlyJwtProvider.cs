using VideoApi.Common.Security.Supplier.Base;
namespace VideoApi.Common.Security.Supplier.Kinly;

public interface IKinlyJwtProvider: ICustomJwtTokenProvider;

public class KinlyJwtProvider(KinlyConfiguration config) : CustomJwtTokenProvider(config), IKinlyJwtProvider;
