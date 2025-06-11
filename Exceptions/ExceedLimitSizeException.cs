namespace RentMateAPI.Exceptions
{
    public class ExceedLimitSizeException : Exception
    {
        public ExceedLimitSizeException(string message) : base(message)
        {
        }
    }
}
