using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tresana.Data.Entities
{
    public class Team 
    {
        [Key]
        public int Id{ get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public virtual List<User> Members { get; set; }

        public Team()
        {
            Members = new List<User>();
        }
    }
}
