using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models.models
{
    public class UserManager
    {
        private List<User> users = new List<User>();
        private User currentUser;

        public User Authenticate(string username, string password)
        {
            var user = users.FirstOrDefault(u => u.Username == username && u.IsActive);
            if (user != null && user.Login(username, password))
            {
                currentUser = user;
                return user;
            }
            return null;
        }

        public void AddUser(User user)
        {
            if (!users.Any(u => u.Username == user.Username))
            {
                users.Add(user);
            }
        }

        public void UpdateUser(User user)
        {
            var existing = users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existing != null)
            {
                // Update logic
            }
        }

        public void DeactivateUser(int userId)
        {
            var user = users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                user.IsActive = false;
            }
        }

        public List<User> GetAllUsers()
        {
            return users.Where(u => u.IsActive).ToList();
        }

        public User GetCurrentUser()
        {
            return currentUser;
        }
    }
}

