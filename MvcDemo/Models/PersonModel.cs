using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcDemo
{
    public class PersonModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(0, 150)]
        public int Age { get; set; }

        [Required(ErrorMessage = "*")]
        public bool Sex { get; set; }
    }
}
