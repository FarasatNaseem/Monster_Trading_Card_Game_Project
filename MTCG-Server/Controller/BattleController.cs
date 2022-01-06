namespace MTCG_Server.Controller
{
    using MTCG_Battle;
    using MTCG_Server.DB;
    using MTCG_Server.Enum;
    using MTCG_Server.Handler.RequestHandler;
    using MTCG_Server.Handler.ResponseHandler;
    using MTCG_Server.Service;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class BattleController : IController
    {
        private static Dictionary<UserSchema, List<CardSchemaWithUserToken>> battleRequest;

        public BattleController()
        {
            this.DbInstance = new Database();
            battleRequest = new Dictionary<UserSchema, List<CardSchemaWithUserToken>>();
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
                "/battles" => this.ControlBattleRequest(request),
                "/score" => this.ControlScoreRequest(request),
                "/stats" => this.ControlStatsRequest(request),
                _ => new HttpResponse()
            };
        }

        private HttpResponse ControlScoreRequest(HttpRequest request)
        {
            throw new NotImplementedException();
        }

        private HttpResponse ControlStatsRequest(HttpRequest request)
        {
            throw new NotImplementedException();
        }

        private HttpResponse ControlBattleRequest(HttpRequest request)
        {
            string message = null;
            string content = null;
            UserSchema player = this.DbInstance.FetchSpecificUser(request.Token);
            List<CardSchemaWithUserToken> playerCards = this.DbInstance.FetchAllDeckCardsOfSpecificUser(request.Token);
            battleRequest.Add(player, playerCards);

            if (battleRequest.Count == 2)
            {

                UserSchema playerA = null;
                List<CardSchemaWithUserToken> playerACards = null;
                UserSchema playerB = null;
                List<CardSchemaWithUserToken> playerBCards = null;

                for (int i = 0; i < battleRequest.Count; i++)
                {
                    if (i == 0)
                    {
                        playerA = battleRequest.ElementAt(i).Key;
                        playerACards = battleRequest.ElementAt(i).Value;
                    }
                    else
                    {
                        playerB = battleRequest.ElementAt(i).Key;
                        playerBCards = battleRequest.ElementAt(i).Value;
                    }
                }

                if (playerA.Name != playerB.Name)
                {
                    Result battleResult = new BattleCoordinator(playerA, playerACards, playerB, playerBCards).ProcessBattle();

                    if (AddStats(playerA.Token, battleResult.PlayerAElo, battleResult.PlayerARoundWinningSteak, battleResult.PlayerBRoundWinningSteak, battleResult.DrawSteak, battleResult.WinnerName, battleResult.LooserName, battleResult.Status) &&
                        (AddStats(playerB.Token, battleResult.PlayerBElo, battleResult.PlayerBRoundWinningSteak, battleResult.PlayerARoundWinningSteak, battleResult.DrawSteak, battleResult.WinnerName, battleResult.LooserName, battleResult.Status)))
                    {
                        content = "Battle Result\n";
                        content += $"Winner name is {battleResult.WinnerName}\n";
                        content += $"Loose name is {battleResult.LooserName}\n";
                        content += $"Player A won {battleResult.PlayerARoundWinningSteak} rounds\n";
                        content += $"Player B won {battleResult.PlayerBRoundWinningSteak} rounds\n";
                        content += $"Total draw rounds {battleResult.DrawSteak}\n";
                    }
                }
                else
                {
                    content = "Battle is not possible";
                }
            }
            else
            {
                content = "Battle cant be started because there is only one player in the pool";
            }

            int code = (int)HttpStatusCode.Ok;

            message = "{";
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
                ReasonPhrase = HttpStatusCode.Ok.ToString(),
                Status = HttpStatusCode.Ok,
                ContentAsUTF8 = message,
                Path = request.Path
            };

            throw new NotImplementedException();
        }


        private bool AddStats(string userToken, int elo, int winSteak, int loseSteak, int drawSteak, string winnerName, string looserName, string status)
        {
            if (!userToken.Contains(winnerName) && status != "Draw")
            {
                status = "Lost";
            }
            StatsSchema statsSchema = new StatsSchema(userToken, elo, winSteak, loseSteak, drawSteak, winnerName, looserName, status);
            return this.DbInstance.AddUserStats(userToken, statsSchema) ? true : false;
        }
    }
}
