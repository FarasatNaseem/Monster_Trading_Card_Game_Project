using MTCG_Server.DB;
using MTCG_Server.Enum;
using MTCG_Server.Handler.RequestHandler;
using MTCG_Server.Handler.ResponseHandler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MTCG_Server.Controller
{
    public class CardController : IController
    {
        public CardController()
        {
            this.DbInstance = new Database();
        }
        public Database DbInstance
        {
            get;
            private set;
        }

        public HttpResponse Control(HttpRequest request)
        {
            return request.Path switch
            {
                // Create packages.
                "/packages" => this.ControlCreatePackageRequest(request),
                // Acquire package.
                "/transactions/packages" => this.ControlAcquirePackageRequest(request),
                // Get Cards by specific user token.
                "/cards" => this.ControlGetCardsRequest(request),
                // Get Deck cards or update Deck cards.
                "/deck" => request.HttpMethod switch
                {
                    HttpMethod.GET => this.ControlGetDeckCardRequest(request),
                    HttpMethod.PUT => this.ControlUpdateDeckCardRequest(request),
                    _ => new HttpResponse()
                },
                "/tradings" => this.ControlCardTradeRequest(request),
                _ => new HttpResponse()
            };
        }

        private HttpResponse ControlCardTradeRequest(HttpRequest request)
        {
            var jObject = JObject.Parse(request.Content);
            var tradeCardSchema = new TradeCardSchema(jObject["Id"].ToString(), "", jObject["MinimumDamage"].ToString(), jObject["Type"].ToString(), jObject["CardToTrade"].ToString());
            string content = null;
            int code;

            if (this.DbInstance.Trade(request.Token, tradeCardSchema))
            {
                content = "Card is traded successfully";
                code = ((int)HttpStatusCode.Ok);
            }
            else
            {
                content = "Due to some error trading is not successfully";
                code = ((int)HttpStatusCode.BadRequest);
            }

            string message = "{";
            message += "\n";
            message += "    \"Content\":";
            message += $" \"{content}\",";
            message += "\n";
            message += "    \"Status\":";
            message += $" \"{code}\"";
            message += "\n";
            message += "}";


            return new HttpResponse()
            {
                ReasonPhrase = ((HttpStatusCode)code).ToString(),
                Status = (HttpStatusCode)code,
                ContentAsUTF8 = message,

                Path = request.Path
            };
        }

        private HttpResponse ControlGetCardsRequest(HttpRequest request)
        {
            var cards = this.DbInstance.FetchAllCardsOfSpecificUser("usercards", request.Token);
            string message = JsonConvert.SerializeObject(cards, Formatting.Indented);
            int code = 0;

            if (message == null)
            {
                string content = "Due to some error package can't be acquired";
                code = ((int)HttpStatusCode.BadRequest);

                message = "{";
                message += "\n";
                message += "    \"Content\":";
                message += $" \"{content}\",";
                message += "\n";
                message += "    \"Status\":";
                message += $" \"{code}\"";
                message += "\n";
                message += "}";
            }
            else
            {
                code = ((int)HttpStatusCode.Ok);
            }

            return new HttpResponse()
            {
                ReasonPhrase = ((HttpStatusCode)code).ToString(),
                Status = (HttpStatusCode)code,
                ContentAsUTF8 = message,

                Path = request.Path
            };
        }

        private HttpResponse ControlAcquirePackageRequest(HttpRequest request)
        {
            string content = null;
            int code;
            if (this.DbInstance.AcquirePackage(request.Token))
            {
                content = "Package is acquired";
                code = ((int)HttpStatusCode.Ok);
            }
            else
            {
                content = "Due to some error package can't be acquired";
                code = ((int)HttpStatusCode.BadRequest);
            }


            string message = "{";
            message += "\n";
            message += "    \"Content\":";
            message += $" \"{content}\",";
            message += "\n";
            message += "    \"Status\":";
            message += $" \"{code}\"";
            message += "\n";
            message += "}";


            return new HttpResponse()
            {
                ReasonPhrase = ((HttpStatusCode)code).ToString(),
                Status = (HttpStatusCode)code,
                ContentAsUTF8 = message,

                Path = request.Path
            };
        }

        private HttpResponse ControlCreatePackageRequest(HttpRequest request)
        {
            string content = null;
            int code;

            if (this.DbInstance.CreatePackage(request.Content, request.Token))
            {
                content = "Packages are created";
                code = ((int)HttpStatusCode.Ok);
            }
            else
            {
                content = "Due to some error packages can't be created";
                code = ((int)HttpStatusCode.BadRequest);
            }


            string message = "{";
            message += "\n";
            message += "    \"Content\":";
            message += $" \"{content}\",";
            message += "\n";
            message += "    \"Status\":";
            message += $" \"{code}\"";
            message += "\n";
            message += "}";


            return new HttpResponse()
            {
                ReasonPhrase = ((HttpStatusCode)code).ToString(),
                Status = (HttpStatusCode)code,
                ContentAsUTF8 = message,

                Path = request.Path
            };
        }

        private HttpResponse ControlGetDeckCardRequest(HttpRequest request)
        {
            string message = null;
            var cards = this.DbInstance.FetchAllCardsOfSpecificUser("userdeck", request.Token);

            if (cards.Count != 0)
            {
                if (request.Content == null)
                    message = JsonConvert.SerializeObject(cards, Formatting.Indented);
                else
                {
                    message += "[\n";
                    foreach (var card in cards)
                    {
                        message += card.ToString();
                    }
                    message += "]\n";
                }
            }

            int code = 0;

            if (message == null)
            {
                string content = "Deck is empty";
                code = ((int)HttpStatusCode.BadRequest);

                message = "{";
                message += "\n";
                message += "    \"Content\":";
                message += $" \"{content}\",";
                message += "\n";
                message += "    \"Status\":";
                message += $" \"{code}\"";
                message += "\n";
                message += "}";
            }
            else
            {
                code = ((int)HttpStatusCode.Ok);
            }

            return new HttpResponse()
            {
                ReasonPhrase = ((HttpStatusCode)code).ToString(),
                Status = (HttpStatusCode)code,
                ContentAsUTF8 = message,

                Path = request.Path
            };
        }

        private HttpResponse ControlUpdateDeckCardRequest(HttpRequest request)
        {
            string content = null;
            int code;

            if (this.DbInstance.ConfigureDeck(request.Token, request.Content))
            {
                content = "Deck is configured";
                code = ((int)HttpStatusCode.Ok);
            }
            else
            {
                content = "Due to some error deck can't be configured";
                code = ((int)HttpStatusCode.BadRequest);
            }

            string message = "{";
            message += "\n";
            message += "    \"Content\":";
            message += $" \"{content}\",";
            message += "\n";
            message += "    \"Status\":";
            message += $" \"{code}\"";
            message += "\n";
            message += "}";


            return new HttpResponse()
            {
                ReasonPhrase = ((HttpStatusCode)code).ToString(),
                Status = (HttpStatusCode)code,
                ContentAsUTF8 = message,

                Path = request.Path
            };
        }
    }
}
