using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using NetworkUtil;
using System.Text.RegularExpressions;

namespace TankWars
{
    public class ServerWebView
    {

        private const string httpOkHeader = "HTTP/1.1 200 OK\r\nConnection: close\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n";
        private const string httpBadHeader = "HTTP/1.1 404 Not Found\r\nConnection: close\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n";
        private const string htmlHeader = "<!DOCTYPE html><html><head><title>TankWars</title></head><body>";
        private const string htmlFooter = "</body></html>";

        private DatabaseController databaseController;

        public ServerWebView(DatabaseController databaseController)
        {
            this.databaseController = databaseController;
        }

        public TcpListener StartServerWebView()
        {
            return Networking.StartServer(HandleHttpConnection, 80);
        }

        private void HandleHttpConnection(SocketState state)
        {
            if (state.ErrorOccured) {
                return;
            }
            state.OnNetworkAction = ServeHttpRequest;
            Networking.GetData(state);
        }

        private void ServeHttpRequest(SocketState state)
        {
            if (state.ErrorOccured) {
                return;
            }
            string request = state.GetData();
            state.RemoveData(0, request.Length);
            string response = BuildHtmlResponse(request);
            Networking.SendAndClose(state.TheSocket, response);
        }

        private string BuildHtmlResponse(string request)
        {
            DatabaseController d = new DatabaseController();
            string response;
            if (IsRequestGetAllGames(request)) {
                // this is where we will call the database accessor 
                //Dictionary<uint, GameModel> allGames = GetDummyAllGamesDictionary();
                Dictionary<uint, DatabaseGameModel> allGames = d.GetAllGames();
                // ---
                response = BuildGetAllGamesHtml(allGames);
            } else if (IsRequestGetGamesForPlayer(request)) {
                string playerName = ExtractPlayerNameFromRequest(request);
                // this is where we will call the database accessor 
                //List<SessionModel> playersGames = GetDummyPlayerGamesList();
                List<DatabaseSessionModel> playersGames = d.GetPlayerSessions(playerName);
                // ---
                response = BuildGetGamesForPlayerHtml(playerName, playersGames);
            } else {
                response = httpBadHeader;
            }
            return response;
        }

        private bool IsRequestGetAllGames(string request)
        {
            // will match things that look like this: GET /games HTTP/1.1 ...
            string pattern = @"GET \/games(?!\?)";
            return Regex.IsMatch(request, pattern);
        }

        private bool IsRequestGetGamesForPlayer(string request)
        {
            // will match things that look like this: GET /games?player=luke HTTP/1.1 ...
            string pattern = @"GET \/games\?player=";
            return Regex.IsMatch(request, pattern);
        }

        private string ExtractPlayerNameFromRequest(string request)
        {
            string pattern = @"GET \/games\?player=(?<name>\w+)\ HTTP\/1.1";
            Match match = Regex.Match(request, pattern);
            string playerName = "";
            if (match.Success) {
                playerName = match.Groups["name"].Value;
            }
            return playerName;
        }

        private Dictionary<uint, DatabaseGameModel> GetDummyAllGamesDictionary()
        {
            // create a fake dictionary for now while we code sql stuff
            // delete all this stuff later
            Dictionary<uint, DatabaseGameModel> dummyGames = new Dictionary<uint, DatabaseGameModel>();
            // definitely delete this random uint typecast. very bad.
            DatabaseGameModel dummyGame1 = new DatabaseGameModel(111, 4201);
            dummyGame1.AddPlayer("dummy player 1.1", 420, 69);
            dummyGame1.AddPlayer("dummy player 1.2", 420, 69);
            dummyGame1.AddPlayer("dummy player 1.3", 420, 69);
            dummyGames[dummyGame1.ID] = dummyGame1;
            DatabaseGameModel dummyGame2 = new DatabaseGameModel(222, 4202);
            dummyGame2.AddPlayer("dummy player 2.1", 420, 69);
            dummyGame2.AddPlayer("dummy player 2.2", 420, 69);
            dummyGame2.AddPlayer("dummy player 2.3", 420, 69);
            dummyGames[dummyGame2.ID] = dummyGame2;
            DatabaseGameModel dummyGame3 = new DatabaseGameModel(333, 4203);
            dummyGame3.AddPlayer("dummy player 3.1", 420, 69);
            dummyGame3.AddPlayer("dummy player 3.2", 420, 69);
            dummyGame3.AddPlayer("dummy player 3.3", 420, 69);
            dummyGames[dummyGame3.ID] = dummyGame3;
            return dummyGames;
        }

        private List<DatabaseSessionModel> GetDummyPlayerGamesList()
        {
            List<DatabaseSessionModel> playersGames = new List<DatabaseSessionModel>();
            playersGames.Add(new DatabaseSessionModel(111, 4201, 420, 69));
            playersGames.Add(new DatabaseSessionModel(222, 4202, 420, 69));
            playersGames.Add(new DatabaseSessionModel(333, 4203, 420, 69));
            return playersGames;
        }


        /// <summary>
        /// Returns an HTTP response containing HTML tables representing the given games
        /// Query your database to construct a dictionary of games to pass to this method
        /// </summary>
        /// <param name="games">Information about all games known</param>
        /// <returns></returns>
        public static string BuildGetAllGamesHtml(Dictionary<uint, DatabaseGameModel> games)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<h1>all games</h1>");
            foreach (uint gid in games.Keys) {
                sb.Append("Game " + gid + " (" + games[gid].Duration + " seconds)<br>");
                sb.Append("<table border=\"1\">");
                sb.Append("<tr><th>Name</th><th>Score</th><th>Accuracy</th></tr>");
                foreach (DatabasePlayerModel p in games[gid].GetPlayers()) {
                    sb.Append("<tr>");
                    sb.Append("<td>" + p.Name + "</td>");
                    sb.Append("<td>" + p.Score + "</td>");
                    sb.Append("<td>" + p.Accuracy + "</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table><br><hr>");
            }

            return httpOkHeader + WrapHtml(sb.ToString());
        }

        /// <summary>
        /// Returns an HTTP response containing one HTML table representing the games
        /// that a certain player has played in
        /// Query your database for games played by the named player, then pass that name
        /// and the list of sessions to this method
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="games">The list of sessions the player has played</param>
        /// <returns></returns>
        public static string BuildGetGamesForPlayerHtml(string name, List<DatabaseSessionModel> games)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<h1>player's game history</h1>");
            sb.Append("Games for " + name + "<br>");
            sb.Append("<table border=\"1\">");
            sb.Append("<tr><th>GameID</th><th>Duration</th><th>Score</th><th>Accuracy</th></tr>");

            foreach (DatabaseSessionModel s in games) {
                sb.Append("<tr>");
                sb.Append("<td>" + s.GameID + "</td>");
                sb.Append("<td>" + s.Duration + "</td>");
                sb.Append("<td>" + s.Score + "</td>");
                sb.Append("<td>" + s.Accuracy + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table><br><hr>");

            return httpOkHeader + WrapHtml(sb.ToString());
        }



        /// <summary>
        /// Returns a simple HTTP greeting response
        /// </summary>
        public static string GetHomePage()
        {
            return httpOkHeader + WrapHtml("Welcome to TankWars");
        }

        /// <summary>
        /// Helper for wraping a string in an HTML header and footer
        /// </summary>
        private static string WrapHtml(string content)
        {
            return htmlHeader + content + htmlFooter;
        }


        /// <summary>
        /// Returns an HTTP response indicating the request was bad
        /// </summary>
        public static string Get404()
        {
            return httpBadHeader + WrapHtml("Bad http request");
        }


    }


}
