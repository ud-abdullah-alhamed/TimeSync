using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TimeSync.Core.DTO.BusinessDTO
{
    public class UserDTO
    {

        public int UserId { get; set; }

        public required string Email { get; set; }

    
        public required string Password { get; set; }
    }
}
