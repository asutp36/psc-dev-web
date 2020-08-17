using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChangerSynchronization_framework.Controllers.Supplies
{
    public class InsertFullEventChangerResult : DbInsertResult
    {
        public List<DbInsertResult> eventsAcquiring { get; set; }
        public List<DbInsertResult> eventsNominals { get; set; }
        public List<DbInsertResult> eventsCard { get; set; }
        public List<DbInsertResult> eventsCollect { get; set; }
    }
}