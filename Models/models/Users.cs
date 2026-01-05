using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models
{
    public abstract class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastLogin { get; set; }

        protected User()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }
        public virtual bool Login(string username, string password)
        {
            if (Username == username && Password == password && IsActive)
            {
                LastLogin = DateTime.Now;
                return true;
            }
            return false;
        }

        public virtual void Logout()
        {
            // kani nga method mag handle sa logout process
        }
        public abstract List<string> GetPermissions();
        public abstract void ShowDashboard();

    }
}
