﻿namespace BookPoint.Models
{
    public class UserModel
    {
        public int Id { get; set; }               
        public string Email { get; set; }         
        public string Password { get; set; }      
        public string Role { get; set; }          
        public bool IsActive { get; set; }        
        public DateTime CreatedAt { get; set; }   
    }
}
