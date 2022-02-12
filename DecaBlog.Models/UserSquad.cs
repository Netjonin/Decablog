using System;

namespace DecaBlog.Models
{
    public class UserSquad
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string SquadId { get; set; }
        public Squad Squad { get; set; }

        public UserSquad()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
