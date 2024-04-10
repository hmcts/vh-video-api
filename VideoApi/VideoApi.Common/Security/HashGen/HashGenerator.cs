using System;
using System.Security.Cryptography;
using System.Text;
using VideoApi.Common.Security.Supplier.Base;

namespace VideoApi.Common.Security.HashGen;

public abstract class HashGeneratorBase
{
    private readonly SupplierConfiguration _supplierConfiguration;

    public HashGeneratorBase(SupplierConfiguration supplierConfiguration)
    {
        _supplierConfiguration = supplierConfiguration;
    }
    public virtual string GenerateHash(DateTime expiresOnUtc, string data)
    {
        var key = Convert.FromBase64String(_supplierConfiguration.ApiSecret);
        var stringToHash = $"{expiresOnUtc}{data}";

        var request = Encoding.UTF8.GetBytes(stringToHash);
        using (var hmac = new HMACSHA256(key))
        {
            var computedHash = hmac.ComputeHash(request);
            return Convert.ToBase64String(computedHash);
        }
    }
}
