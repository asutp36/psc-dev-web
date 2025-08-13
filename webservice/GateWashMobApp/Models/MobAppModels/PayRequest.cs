
namespace GateWashMobApp.Models.MobAppModels
{
    public class PayRequest
    {
        public string time_send { get; set; }
        public string hash { get; set; }
        public string phone { get; set; }
        public string post { get; set; }
        public decimal amount { get; set; }
    }
}
