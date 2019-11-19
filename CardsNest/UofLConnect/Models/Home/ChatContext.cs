using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace UofLConnect.Models.Home
{
    public class ChatContext : DbContext
    {
        public ChatContext() : base("UofLConnectDb")
        {
        }

        public static ChatContext Create()
        {
            return new ChatContext();
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
    }
}