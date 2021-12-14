namespace MTCG_Server.Routing
{
    using System;
    using System.Collections.Generic;
    using MTCG_Server.Controller;
    using MTCG_Server.Enum;
    public class RouteInitializer : IInitializer<List<Route>>
    {
        private IController controller;

        public List<Route> Initialize()
        {
            return new List<Route>()
            {
                this.InitializeRegisterRoute(),
                this.InitializeLoginRoute(),
                this.InitializeSessionRoute()
            };
        }

        private Route InitializeRegisterRoute()
        {
            this.controller = new RegisterController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/Register",
                Method = HttpMethod.POST
            };
        }

        private Route InitializeLoginRoute()
        {
            this.controller = new LoginController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/Login",
                Method = HttpMethod.POST
            };
        }

        private Route InitializeSessionRoute()
        {
            this.controller = new SessionController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/Session",
                Method = HttpMethod.GET
            };
        }
    }
}
