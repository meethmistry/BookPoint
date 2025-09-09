using System.ComponentModel.DataAnnotations;

namespace BookPoint.Models
{
    public class AgentModel
    {
        public int Id { get; set; }
        public int UID { get; set; }

        public UserModel User { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string Phone { get; set; }
    }
}
