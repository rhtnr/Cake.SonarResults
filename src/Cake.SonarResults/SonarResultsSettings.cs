using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SonarResults
{
    public class SonarResultsSettings
    {
        public string UserName { get; private set; }
        public  string Password { get; private set; }
        public string Token { get; private set; }
        public string Url { get; private set; }

        public bool IsAuthEnabled 
        { 
            get
            {
                return (!String.IsNullOrEmpty(UserName) || !String.IsNullOrEmpty(Token));
            } 
        }

        public HttpBasicAuthenticator GetAuthenticator
        {
            get
            {
                if (IsAuthEnabled) throw new NotSupportedException("Authenticator cannot be created with no username/password or token set");
                if(!string.IsNullOrEmpty(UserName))
                    return new HttpBasicAuthenticator(UserName, Password);
                return new HttpBasicAuthenticator(Token, "");
            }
        }

        public SonarResultsSettings(string url, string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                throw new ArgumentException("Username/Password invalid!");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("SonarQube URL is invalid!");
            UserName = userName;
            Password = password;
            Url = url;
        }

        public SonarResultsSettings(string url, string token)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("SonarQube URL is invalid!");
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is invalid!");
            Url = url;
            Token = token;
        }

        public SonarResultsSettings(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("SonarQube URL is invalid!");
            Url = url;
        }

    }
}
