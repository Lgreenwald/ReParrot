using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.OrganizationMember
{
    public class OrgMemberRoleUpdate
    {
        [Required]
        public List<int> RoleIds { get; set; }
        [Required]
        public int UserToUpdateId { get; set; }
    }
}
