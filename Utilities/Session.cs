using BookingApp.Domain;
using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Utilities
{
    public class Session
    {
        public static User? CurrentUser { get; set; }


        public static bool IsLoggedIn => CurrentUser != null;


        public static void ClearSession()
        {
            CurrentUser = null;
        }


        public static void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }


        public static UserRole? GetCurrentUserRole()
        {
            return CurrentUser?.Role;
        }


        public static bool HasRole(UserRole role)
        {
            return CurrentUser?.Role == role;
        }

    }
}
