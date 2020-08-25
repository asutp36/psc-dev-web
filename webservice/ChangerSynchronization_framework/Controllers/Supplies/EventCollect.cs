using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChangerSynchronization_framework.Controllers.Supplies
{
    public class EventCollect
    {
        [Required]
        public string changer { get; set; }
        [Required]
        public DateTime dtime { get; set; }
        [Required]
        public int m10 { get; set; }
        [Required]
        public int b50 { get; set; }
        [Required]
        public int b100 { get; set; }
        [Required]
        public int b200 { get; set; }
        [Required]
        public int b500 { get; set; }
        [Required]
        public int b1000 { get; set; }
        [Required]
        public int b2000 { get; set; }
        [Required]
        public int box1_50 { get; set; }
        [Required]
        public int box2_100 { get; set; }
        [Required]
        public int box3_50 { get; set; }
        [Required]
        public int box4_100 { get; set; }
        public int box5_10 { get; set; }
        [Required]
        public int badCards { get; set; }
        [Required]
        public int availableCards { get; set; }
    }

}