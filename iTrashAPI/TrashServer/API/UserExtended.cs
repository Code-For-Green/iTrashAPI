namespace TrashServer.API
{
    public record UserExtended(int ID, string Login, string Password, string Email) : User(ID,Login,Password)
    {

    }
}
