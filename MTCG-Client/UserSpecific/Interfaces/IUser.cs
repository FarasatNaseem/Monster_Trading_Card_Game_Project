namespace MTCG_Client.UserSpecific.Interfaces
{
    using MTCG_Client.UserSpecific.Enum;
    public interface IUser
    {
        public UserCredential UserCredential { get; }
        public Role Role { get; }
    }
}
