using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace VideoApi.Common
{
    /// <summary>
    /// Exception to throw when input data passed downstream from the api input is in an invalid format
    /// </summary>
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }

        public static void ThrowIfNull([NotNull] object argument,
            [CallerArgumentExpression(nameof(argument))] string paramName = null)
        {
            if (argument is null)
            {
                throw new BadRequestException($"Value cannot be null {paramName}");
            }
        }
    }
}
