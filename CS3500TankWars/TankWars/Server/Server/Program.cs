using NetworkUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;  // StopWatch
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;  // Sleep
using System.Threading.Tasks;

namespace TankWars
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server is running...");

            GameController gameController = new GameController(); 
            ServerNetworkingController networkingController = new ServerNetworkingController(gameController);
            DatabaseController databaseController = new DatabaseController();

            ServerWebView webViewController = new ServerWebView(databaseController);
            TcpListener gameServerListener = networkingController.StartGameServer();
            TcpListener webViewListener = webViewController.StartServerWebView();

            // Initialize the world through the gameController
            gameController.InitializeWorld();

            // Spawn frame thread loop
            BackgroundWorker frameLoopThread = new BackgroundWorker();
            frameLoopThread.WorkerSupportsCancellation = true;
            frameLoopThread.DoWork += GameFrameLoop;
            object[] parameters = new object[] { gameController, networkingController };
            frameLoopThread.RunWorkerAsync(parameters);

            Console.WriteLine("Type anything into the console to end the game");
            // Terminate the server when a character is recieved 
            Console.ReadLine();

            Console.WriteLine("Game complete");
            // A game is considered completed when the user types anything into the console. 
            databaseController.SaveGameToDatabase(gameController.GameWorld, gameController.GameDurationInSeconds);

            // properly shut down the server event loop
            Networking.StopServer(gameServerListener);
            // Clean up our frame loop thread once user prompts to terminate
            frameLoopThread.CancelAsync();

            Console.WriteLine("Web server is still running so you can view game reports.");
            Console.WriteLine("Type anything into the console to shut down the web server");
            Console.ReadLine();
            Console.WriteLine("Killing server");
            Networking.StopServer(webViewListener);
        }


        private static void GameFrameLoop(object sender, DoWorkEventArgs e)
        {
            object[] parameters = e.Argument as object[];
            GameController gameController = (GameController)parameters[0];
            ServerNetworkingController networkingController = (ServerNetworkingController)parameters[1];

            Stopwatch stopwatch = new Stopwatch();
            int millisecondsPerFrame = gameController.GameSettings.MillisecondsPerFrame;  // just testing
            while (true) {

                // Wait for millisecondsPerFrame
                stopwatch.Start();
                while (stopwatch.ElapsedMilliseconds < millisecondsPerFrame) {
                    Thread.Sleep(1);
                }
                stopwatch.Reset();

                // Update the world through the gameController
                lock (gameController) {
                    gameController.UpdateWorld();
                }

                // Send the JSON reperesenting the game world to the clients
                string latestGameWorldJson = gameController.SerializeGameWorld();
                networkingController.SendMessageToEveryClient(latestGameWorldJson);

                // check if cancellation is pending. if so, exit method
                if (e.Cancel) {
                    return;
                }
            }

        }

    }
}
