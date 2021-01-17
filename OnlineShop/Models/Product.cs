using Proiect2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect2.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(60, ErrorMessage = "Numele nu poate contine mai mult de 60 caractere")]
        public string Nume { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie")]
        [DataType(DataType.MultilineText)]
        public string Descriere { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Pretul nu poate fi negativ")]
        public float Pret { get; set; }

        [Required(ErrorMessage ="Poza e obligatorie")]
        public string Poza { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int CategoryId { get; set; }

        public float Rating { get; set; }

        public bool Cerere { get; set; }

        public DateTime Date { get; set; }
        public string UserId { get; set; }

        public IEnumerable<SelectListItem> Categ { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<Quantity> Quantities { get; set; }
    }
}