namespace MSO.SyncService.Exceptions
{
    public class CustomStatusCodeException : Exception
    {
        public int ResponseStatusCode { get; set; }

        public CustomStatusCodeException(string message, int statusCode) : base(message)
        {
            ResponseStatusCode = statusCode;
        }
    }
}
