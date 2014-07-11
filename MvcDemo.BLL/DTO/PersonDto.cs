using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcDemo.BLL
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public bool Sex { get; set; }
    }
}
