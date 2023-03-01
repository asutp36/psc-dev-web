namespace MSO.SyncService.Models
{
    public class EventIncreaseDto
    {
        public string Device { get; set; } // по нему ищу IDPost
        public int IDEventPost { get; set; }
        public DateTime DTime { get; set; }
        public string Kind { get; set; }
        public int Amount { get; set; }
        public int m10 { get; set; }
        public int b10 { get; set; }
        public int b50 { get; set; }
        public int b100 { get; set; }
        public int b200 { get; set; }
        public int Balance { get; set; }
        public int IDPostSession { get; set; }
        public string? CardNum { get; set; }
    }
}
