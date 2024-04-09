using VideoApi.Common.Security.Supplier.Base;

namespace VideoApi.Common.Security.Supplier.Vodafone;
public interface IVodafoneJwtTokenHandler: ICustomJwtTokenHandler{}

public class VodafoneJwtHandler(VodafoneConfiguration config) : CustomJwtTokenHandler(config), IVodafoneJwtTokenHandler{}
