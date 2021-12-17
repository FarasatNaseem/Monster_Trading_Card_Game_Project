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
                this.InitializePackagesRoute()
            };
        }

        private Route InitializeRegisterRoute()
        {
            this.controller = new UserController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/users",
                Method = HttpMethod.POST
            };
        }

        private Route InitializeLoginRoute()
        {
            this.controller = new UserController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/sessions",
                Method = HttpMethod.POST
            };
        }

        private Route InitializePackagesRoute()
        {
            this.controller = new CardController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/packages",
                Method = HttpMethod.POST
            };
        }
    }
}
