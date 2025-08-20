namespace BookPoint.Models
{
    public class AgentModel
    {
        public int Id { get; set; }
        public int UID { get; set; }
        public UserModel User { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
    }
}
