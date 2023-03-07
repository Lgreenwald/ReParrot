using Sabio.Models.Domain.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Specialties
{
    public class Specialty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Organization Organization { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set;}
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
