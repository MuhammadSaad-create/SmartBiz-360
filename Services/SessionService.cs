using SmartBiz_360.Models;

namespace SmartBiz_360.Services
{
    public class SessionService
    {
        public User? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;
        public bool IsAdmin => CurrentUser?.Role == "Admin";
        public bool IsManager => CurrentUser?.Role == "Manager";

        public void Login(User user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}