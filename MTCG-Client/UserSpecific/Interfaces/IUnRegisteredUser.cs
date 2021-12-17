namespace MTCG_Client.UserSpecific.Interfaces
{
    public interface IUnRegisteredUser
    {
        Response Register(UserCredential userCredential);
    }
}
