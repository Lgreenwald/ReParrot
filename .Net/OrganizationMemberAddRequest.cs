using Sabio.Models.Requests.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.OrganizationMember
{
    public class OrganizationMemberAddRequest : UserAddRequest
    {
        [Required, Range(1, Int32.MaxValue)]
        public int OrganizationId { get; set; }
        [Required, Range(1, Int32.MaxValue)]
        public int UserId { get; set; }
     
        [Required, Range(1, Int32.MaxValue)]
        public int PositionType { get; set; }
        [StringLength(100, MinimumLength = 1)]
        public string OrganizationEmail { get; set; }
        [Required]
        public int InviteId { get; set; }
        [Required]
        [Range(1, 5)]
        public int RoleId { get; set; }
    }
}
