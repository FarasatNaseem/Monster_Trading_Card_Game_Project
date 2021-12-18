using MTCG_Client.UserSpecific;
using MTCG_Client.UserSpecific.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Client.GameSpecific
{
    public class Game
    {
        private IUnRegisteredUser unRegisteredUser;
        private IRegisteredUser registeredUser;
        private UI uI;
        private IReader<UserCredential> userCredentialReader;
        private IReader<char> mainMenuCommandReader;
        private bool isLoggedIn;
        public Game()
        {
            this.uI = new UI();
            this.isLoggedIn = false;
            this.userCredentialReader = new UserCredentialReader();
            this.mainMenuCommandReader = new MainMenuCommandReader();
        }

        public void Start()
        {
            this.uI.PrintMTCGHeader();

            while (!this.isLoggedIn)
            {
                this.uI.PrintMainMenu();

                char mainMenuCommand = this.mainMenuCommandReader.Read();

                Console.Clear();
                switch (mainMenuCommand)
                {
                    case 'L':
                        UserCredential userCredential = this.userCredentialReader.Read();
                        this.registeredUser = new MTCGUser("admin");
                        Response response = this.registeredUser.Login(userCredential);
                        
                        if (((LoginResponse)response).Token != null)
                        {
                            Console.Clear();
                            Console.WriteLine("-------------------------------------------------------");
                            Console.WriteLine("Content: " + ((LoginResponse)response).Content);
                            Console.WriteLine("Status: " + ((LoginResponse)response).StatusCode);
                            Console.WriteLine("Token: " + ((LoginResponse)response).Token);
                            isLoggedIn = true;
                        }
                        else
                        {
                            Console.WriteLine("Token: " + ((LoginResponse)response).Token);
                        }
                        Console.WriteLine("-------------------------------------------------------");


                        break;
                    case 'R':
                        userCredential = this.userCredentialReader.Read();
                        this.unRegisteredUser = new MTCGUser("user");
                        response = this.unRegisteredUser.Register(userCredential);
                        Console.WriteLine("-------------------------------------------------------");
                        Console.WriteLine("Content: " + ((RegistrationResponse)response).Content);
                        Console.WriteLine("Status: " + ((RegistrationResponse)response).StatusCode);
                        Console.WriteLine("-------------------------------------------------------");
                        break;
                    case 'C':
                        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                        proc.EnableRaisingEvents = false;
                        proc.StartInfo.FileName = @"C:\Users\Privat\Downloads\MonsterTradingCards.exercise.curl (2).bat";
                        proc.Start();
                       
                        Console.WriteLine("Curl Script is now testing APIs");
                        break;
                    case 'Q':
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            }

            Console.WriteLine("PLAY GAME :-)");
            while (this.isLoggedIn)
            {

            }
        }
    }
}
