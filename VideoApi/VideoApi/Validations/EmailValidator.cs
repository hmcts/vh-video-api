using System;
using System.Net.Mail;

namespace VideoApi.Validations
{
    /// <summary>Simple validator to check email formats</summary>
    public static class EmailValidator
    {
        /// <summary>
        /// Test if the given string is specified and a valid email address
        /// </summary>
        /// <remarks>
        /// This was recommended one of the simplest way to manage email validation.
        /// </remarks>
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
#pragma warning disable S1481
                var address = new MailAddress(email);
#pragma warning restore S1481
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
