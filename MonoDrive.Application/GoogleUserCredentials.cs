using System;

namespace MonoDrive.Application
{
    public class GoogleUserCredentials
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public string Scope { get; set; }
        public DateTimeOffset Issued { get; set; }
        public DateTimeOffset IssuedUtc { get; set; }
    }
}