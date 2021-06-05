using System;
using System.Text;
using System.Security.Cryptography;


namespace TrashServer.API
{
    public record User(int ID, string Login, string Password)
    {
        public User Hashed() => this with
        {
            Password = string.IsNullOrEmpty(Password) ? null : Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Password)))
        };

        public virtual bool Equals(User other) => this.Login == other.Login && this.Password == other.Password;

        public override int GetHashCode() => throw new NotImplementedException();
    }
}
