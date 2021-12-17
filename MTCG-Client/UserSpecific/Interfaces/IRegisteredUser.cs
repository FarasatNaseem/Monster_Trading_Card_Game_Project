namespace MTCG_Client.UserSpecific.Interfaces
{
    public interface IRegisteredUser
    {
        Response Login(UserCredential userCredential);
    }
}
