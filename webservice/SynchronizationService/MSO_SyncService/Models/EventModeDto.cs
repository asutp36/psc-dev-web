namespace MSO.SyncService.Models
{
    public class EventModeDto
    {
        public string Device { get; set; } // по нему ищу IDPost
        public int IDEventPost { get; set; }
        public string Mode { get; set; }
        public DateTime DTimeStart { get; set; }
        public DateTime DTimeFinish { get; set; }
        public int? Duration { get; set; }
        public int? PaymentSign { get; set; }
        public decimal? Cost { get; set; }
        public string? CardTypeCode { get; set; }
        public string? CardNum { get; set; }
        public int? Discount { get; set; }
    }
}
