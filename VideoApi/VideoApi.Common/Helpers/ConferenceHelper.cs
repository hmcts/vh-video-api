using System;
using System.Security.Cryptography;
using System.Text;

namespace VideoApi.Common.Helpers;

public static class ConferenceHelper
{
    public static string GenerateGlobalRareNumber()
    {
        Guid guid = Guid.NewGuid();
        using SHA256 sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(guid.ToString()));
        // Ensures 8 digits (between 10,000,000 and 99,999,999)
        var uniqueNumber = Math.Abs(BitConverter.ToInt32(hash, 0)) % 90000000 + 10000000; 
        return uniqueNumber.ToString(); 
    }
}
