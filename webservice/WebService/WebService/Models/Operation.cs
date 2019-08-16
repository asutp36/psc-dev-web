namespace WebService.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Operation
    {
        public int OperationID { get; set; }

        public int PscID { get; set; }

        public int OperationTypeID { get; set; }

        public int CardID { get; set; }

        public DateTime DTime { get; set; }

        public int Amount { get; set; }

        public int Balance { get; set; }

        public int ServerID { get; set; }

        public virtual Card Card { get; set; }

        public virtual OperationType OperationType { get; set; }

        public virtual Psce Psce { get; set; }
    }
}
