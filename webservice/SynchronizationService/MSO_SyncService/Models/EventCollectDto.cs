﻿namespace MSO.SyncService.Models
{
    public class EventCollectDto
    {
        public string Device { get; set; } // по нему ищу IDPost
        public int IDEventPost { get; set; }
        public DateTime DTime { get; set; }
        public int Amount { get; set; }
        public int m10 { get; set; }
        public int b10 { get; set; }
        public int b50 { get; set; }
        public int b100 { get; set; }
        public int b200 { get; set; }
    }
}
