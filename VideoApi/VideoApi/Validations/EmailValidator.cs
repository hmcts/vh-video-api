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
#pragma warning disable S1848 // Objects should not be created to be dropped immediately without being used
                new MailAddress(email);
#pragma warning restore S1848 // Objects should not be created to be dropped immediately without being used
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
