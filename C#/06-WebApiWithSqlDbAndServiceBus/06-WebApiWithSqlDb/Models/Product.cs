using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace _06_WebApiWithSqlDb.Models
{
    public class Product
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage ="Name cannot be empty")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Price cannot be empty")]
        [Range(1, double.MaxValue)]
        public double Price { get; set; }

        [Required(ErrorMessage = "Quantity cannot be empty")]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
