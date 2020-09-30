using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin_HelloApp.Models
{
    public class Credentials
    {
        public Uri ServerUrl { get; private set; }
        public string Username { get; private set; }
        public string ProtectedPassword { get; private set; }

        public bool UseWindowsAuth
        {
            get { return !string.IsNullOrEmpty(Username) && (Username.Contains("\\") || Username.Contains("@")); }
        }

        public string DatabaseName { get; private set; }

        public static Credentials GetConnectionCredentials(string database, string username, string password)
        {
            var credentials = new Credentials
            {
                ServerUrl = new Uri(@"http://ecm.ascon.ru:5545/"),
                Username = username,
                ProtectedPassword = password,
                DatabaseName = database
            };

            return credentials;
        }
    }
}