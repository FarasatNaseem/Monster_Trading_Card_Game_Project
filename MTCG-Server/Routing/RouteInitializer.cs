namespace MTCG_Server.Routing
{
    using System.Collections.Generic;
    using MTCG_Server.Controller;
    using MTCG_Server.Enum;
    public class RouteInitializer : IInitializer<List<Route>>
    {
        private IController controller;
        private List<Route> routes;

        public RouteInitializer()
        {
            this.routes = new List<Route>();
            this.InitializeLoggedInUserRoute();
        }

        public List<Route> Initialize()
        {
            this.routes.Add(this.InitializeRegisterRoute());
            this.routes.Add(this.InitializeLoginRoute());
            this.routes.Add(this.InitializeCreatePackagesRoute());
            this.routes.Add(this.InitializeAcquirePackagesRoute());
            this.routes.Add(this.InitializeGetCardRoute());
            this.routes.Add(this.InitializeGetDeckCardRoute());
            this.routes.Add(this.InitializeUpdateDeckCardRoute());
            this.routes.Add(this.InitializeGetStatsRoute());
            this.routes.Add(this.InitializeBattleRoute());
            this.routes.Add(this.InitializeGetScoreRoute());
            this.routes.Add(this.InitializeTradeCardRoute());

            return this.routes;
        }

        private Route InitializeTradeCardRoute()
        {
            this.controller = new CardController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/tradings",
                Method = HttpMethod.POST
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

        private Route InitializeCreatePackagesRoute()
        {
            this.controller = new CardController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/packages",
                Method = HttpMethod.POST
            };
        }

        private Route InitializeAcquirePackagesRoute()
        {
            this.controller = new CardController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/transactions/packages",
                Method = HttpMethod.POST
            };
        }

        private Route InitializeGetCardRoute()
        {
            this.controller = new CardController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/cards",
                Method = HttpMethod.GET
            };
        }

        private Route InitializeGetDeckCardRoute()
        {
            this.controller = new CardController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/deck",
                Method = HttpMethod.GET
            };
        }

        private Route InitializeUpdateDeckCardRoute()
        {
            this.controller = new CardController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/deck",
                Method = HttpMethod.PUT
            };
        }

        private Route InitializeGetStatsRoute()
        {
            this.controller = new BattleController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/stats",
                Method = HttpMethod.GET
            };
        }

        private Route InitializeGetScoreRoute()
        {
            this.controller = new BattleController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/score",
                Method = HttpMethod.GET
            };
        }

        private Route InitializeBattleRoute()
        {
            this.controller = new BattleController();

            return new Route()
            {
                Callable = this.controller.Control,
                Url = "/battles",
                Method = HttpMethod.POST
            };
        }

        private void InitializeLoggedInUserRoute()
        {
            this.controller = new UserController();

            foreach (var name in this.controller.DbInstance.FetchAllLoggedInUsername())
            {
                this.routes.Add(new Route()
                {
                    Callable = this.controller.Control,
                    Url = $"/users/{name}",
                    Method = HttpMethod.GET
                });

                this.routes.Add(new Route()
                {
                    Callable = this.controller.Control,
                    Url = $"/users/{name}",
                    Method = HttpMethod.PUT
                });
            }
        }
    }
}
