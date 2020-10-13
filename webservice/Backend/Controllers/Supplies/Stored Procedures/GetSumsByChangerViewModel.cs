using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Stored_Procedures
{
    public class GetSumsByChangerViewModel : GetSumsByChanger_Result
    {
        public string boxIncrease { get; set; }
        public string boxOut { get; set; }
        public string boxCards { get; set; }

        public GetSumsByChangerViewModel(GetSumsByChanger_Result storedProcedureResult, ChangerInfo info) 
        {
            this.IDChanger = storedProcedureResult.IDChanger;
            this.ChangerCode = storedProcedureResult.ChangerCode;
            this.IpAddress = storedProcedureResult.IpAddress;
            this.sincrease = storedProcedureResult.sincrease;
            this.sout = storedProcedureResult.sout;
            this.ccard = storedProcedureResult.ccard;

            this.boxIncrease = (info.m10 * 10 +
                        info.b50 * 50 +
                        info.b100 * 100 +
                        info.b200 * 200 +
                        info.b500 * 500 +
                        info.b1000 * 1000 +
                        info.b2000 * 2000).ToString();

            this.boxOut = (info.box1_50 * 50 +
                        info.box2_100 * 100 +
                        info.box3_50 * 50 +
                        info.box4_100 * 100).ToString();

            this.boxCards = info.availableCards.ToString();
        }

        public GetSumsByChangerViewModel(GetSumsByChanger_Result storedProcedureResult)
        {
            this.IDChanger = storedProcedureResult.IDChanger;
            this.ChangerCode = storedProcedureResult.ChangerCode;
            this.IpAddress = storedProcedureResult.IpAddress;
            this.sincrease = storedProcedureResult.sincrease;
            this.sout = storedProcedureResult.sout;
            this.ccard = storedProcedureResult.ccard;
            this.boxIncrease = "неизвестно";
            this.boxOut = "неизвестно";
            this.boxCards = "неизвестно";
        }
    }
}
