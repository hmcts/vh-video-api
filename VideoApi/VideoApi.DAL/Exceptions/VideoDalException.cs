using System;

namespace VideoApi.DAL.Exceptions
{
    public abstract class VideoDalException : Exception
    {
        protected VideoDalException(string message) : base(message)
        {
        }
    }
    
    public abstract class EntityNotFoundException : VideoDalException
    {
        protected EntityNotFoundException(string message) : base(message)
        {
        }
    }
}
