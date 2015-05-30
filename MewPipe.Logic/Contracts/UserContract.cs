using System;
using System.Runtime.Serialization;
using MewPipe.Logic.Models;

namespace MewPipe.Logic.Contracts
{
    public class UserContract
    {
        public UserContract()
        {
        }
        public UserContract(User user)
        {
            Id = user.Id;
            Email = user.Email;
            Username = user.UserName;
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}