using System;
using System.Security.Cryptography;
using System.Text;

namespace VideoApi.Common.Helpers;

public static class ConferenceHelper
{
    public static string GenerateGlobalRareNumber()
    {
        var guid = Guid.NewGuid();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(guid.ToString()));
        var uniqueNumber = Math.Abs(BitConverter.ToInt32(hash, 0)) % 90000000 + 10000000;  // Ensures 8 digits (between 10,000,000 and 99,999,999)
        return uniqueNumber.ToString();
    }
}
