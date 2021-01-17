using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proiect2.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required(ErrorMessage ="Numele categoriei este obligatoriu")]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}