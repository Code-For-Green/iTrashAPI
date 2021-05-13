using System;
using System.Text;
using System.Security.Cryptography;


namespace TrashServer.API
{
    public record User
    {
        public string Login { get; init; }
        public string Password { get; init; }

        public User Hashed() => this with
        {
            Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Password)))
        };
    }
}
