using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proiect2.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Quantity> Quantities { get; set; }
    }
}