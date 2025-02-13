using System;
using VideoApi.Events.Models;

namespace VideoApi.Events.Exceptions;

public class UnexpectedEventOrderException(CallbackEvent callbackEvent, Exception innerException)
    : Exception("Event received in an unexpected order", innerException)
{
    public CallbackEvent CallbackEvent { get; } = callbackEvent;
}
