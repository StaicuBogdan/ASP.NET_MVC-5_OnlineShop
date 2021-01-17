using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proiect2.Models
{
    public class Quantity
    {
        [Key]
        public int QuantityId { get; set; }
        public int Id { get; set; }
        public int CartId { get; set; }
        public int Cantitate { get; set; }

        public virtual Product Product { get; set; }
        public virtual Cart Cart { get; set; }
    }
}