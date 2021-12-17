namespace MTCG_Client.GameSpecific
{
    using System;
    using MTCG_Client.UserSpecific;
    public class UserCredentialReader : IReader<UserCredential>
    {
        public UserCredentialReader() { }

        public UserCredential Read()
        {
            return new UserCredential(this.ReadName(), this.ReadPassword());
        }

        private string ReadName()
        {
            string name = null;

            try
            {
                Console.Write("please enter your name: ");
                name = Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return name;
        }

        private string ReadPassword()
        {
            string password = null;

            try
            {
                Console.Write("please enter your password: ");
                password = Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return password;
        }
    }
}
