using System.ComponentModel.DataAnnotations;

namespace BookPoint.Models
{
    public class UserAgentViewModel
    {
       
        public UserModel User { get; set; } = new UserModel();
        public AgentModel Agent { get; set; } = new AgentModel();
        public List<AgentModel> Agents { get; set; } = new List<AgentModel>();
        public List<UserModel> Users { get; set; } = new List<UserModel>();

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
