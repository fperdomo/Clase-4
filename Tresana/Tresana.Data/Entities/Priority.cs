using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tresana.Data.Entities
{
    public class Priority 
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }

        public Priority()
        {
            
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
