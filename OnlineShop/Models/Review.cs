using Proiect2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proiect2.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        [Required(ErrorMessage = "Campul nu poate fi necompletat")]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating-ul are valori cuprinse intre 1 si 5")]
        public int Rating { get; set; }

        public string UserId { get; set; }


        public virtual ApplicationUser User { get; set; }
        public virtual Product Product { get; set; }
    }
}