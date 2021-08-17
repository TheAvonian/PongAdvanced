using System.Data;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using System.IO;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

class Program {
    static bool running = false;
    static UserEventLogic userEvent;
    const int MIN_HEIGHT = 8;
    const int MIN_WIDTH = 64;
    static Config config;
    static string dataFolder;
    static Pong player;
    static Pong enemy;
    static void Main(string[] args) {

        //////////////////// START 
        Console.Title = "POONG";
        Console.CursorVisible = false;
        /// CONFIG START
        Console.Write("Loading...");
        dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AdvancedPong");
        if(!Directory.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);
        dataFolder = Path.Combine(dataFolder, "save.dat");
        if(File.Exists(dataFolder)) {
            var fileDataString = File.ReadAllText(dataFolder);
            config = JsonSerializer.Deserialize<Config>(fileDataString);
        } else {
            config = new();
            config.Height = Console.WindowHeight - 1;
            config.Width = Console.WindowWidth - 1;
            SaveConfig();
        }
        /// CONFIG END
        do {
            ShowMenu();
        } while(true);
        //////////////////// END

    }

    readonly static string[] PONG_MESSAGE = {
        "----------------------------------------------------------------",
        "      :::::::::       ::::::::       ::::    :::       :::::::: ",
        "     :+:    :+:     :+:    :+:      :+:+:   :+:      :+:    :+: ",
        "    +:+    +:+     +:+    +:+      :+:+:+  +:+      +:+         ",
        "   +#++:++#+      +#+    +:+      +#+ +:+ +#+      :#:          ",
        "  +#+            +#+    +#+      +#+  +#+#+#      +#+   +#+#    ",
        " #+#            #+#    #+#      #+#   #+#+#      #+#    #+#     ",
        "###             ########       ###    ####       ########       ",
        "----------------------------------------------------------------"};
    readonly static int[] LOW_QUAL_PONG_INDICES = {0, 1, 2, 4, 5, 7, 8};
    const int MIN_MENU_GOOD_HEIGHT = 12;

    static void ShowMenu() {
        Console.Clear();
        for(int i = 0; i < PONG_MESSAGE.Length; i++) {
            if(config.Height > MIN_MENU_GOOD_HEIGHT || LOW_QUAL_PONG_INDICES.Any(x => x == i)) {
                Console.WriteLine(PONG_MESSAGE[i]);
            }
        }
        Console.WriteLine();
        Console.WriteLine($"[1] Start Pong | High Score: {config.Score} |");
        Console.WriteLine("[2] Options");
        Console.WriteLine("[3] Quit");

        ConsoleKey userInput = SelectOption(ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3);
        switch(userInput) {
            case ConsoleKey.D1:
                RunGame();
                break;
            case ConsoleKey.D2:
                ShowOptions();
                break;
            case ConsoleKey.D3:
                Environment.Exit(0);
                break;
        }
    }

    static void ShowOptions() {
        Console.Clear();
        Console.WriteLine($"[1] Resolution: {config.Height}x{config.Width}");
        Console.WriteLine($"CWIDTH: {Console.WindowWidth} CHEIGHT: {Console.WindowHeight}");
        
        Console.WriteLine("[3] Back");
        ConsoleKey userInput = SelectOption(ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3);
        switch(userInput) {
            case ConsoleKey.D1:
                ShowResolution();
                break;
            case ConsoleKey.D2:
                break;
            case ConsoleKey.D3:
                return;
        }
    }

    static void RunGame() {
        // SET UP GAME WITH CONFIG
        Console.Clear();

        /*Console.SetCursorPosition(0,0);
        Console.WriteLine(new String('-', config.Width));
        Console.SetCursorPosition(0,config.Height);
        Console.WriteLine(new String('-', config.Width));*/
        running = true;
        player = new();
        enemy = new();

        Stopwatch timer = new();
        timer.Start();
        player.Draw();

        Ball ball = new() {X = config.Width - 5, Y = config.Height - 5, LastX = 0, LastY = 0, Speed = 10, Direction = 7.0 * Math.PI / 4.0};

        double deltaTime = 0;
        double lastTotalMilliseconds = timer.ElapsedMilliseconds / 1000.0;
        Task.Run(()=>{
            userEvent = new();
            userEvent.UserInputCompleted += UserInputHandler;
            userEvent.StartUserInput();
        });
        //userEvent = new();
        //userEvent.UserInputCompleted += UserInputHandler;
        //userEvent.StartUserInput();
        while(running) {
            deltaTime = timer.ElapsedMilliseconds / 1000.0 - lastTotalMilliseconds;
            
            player.Update();
            /// enemy


            /// ball
            
            var tempX = (int)Math.Round(ball.X + ball.Speed * Math.Cos(ball.Direction) * deltaTime);
            var tempY = (int)Math.Round(ball.Y + ball.Speed * Math.Sin(ball.Direction) * deltaTime);
            if((tempX < 0 || tempX > config.Width - 2) || (tempX == player.X && (tempY == player.Y - 1 || tempY == player.Y || tempY == player.Y + 1))) {
                ball.Direction = ball.Direction <= Math.PI ? .5 * Math.PI + (.5 * Math.PI - ball.Direction) : 1.5 * Math.PI + (1.5 * Math.PI - ball.Direction);
                tempX = ball.LastX;
            }
            if((tempY < 0 || tempY > config.Height) || (tempX == player.X && (tempY == player.Y - 1 || tempY == player.Y + 1))) {
                ball.Direction = ball.Direction <= Math.PI * 1.5 && ball.Direction >= Math.PI * .5 ? Math.PI + (Math.PI - ball.Direction) : -ball.Direction;
                tempY = ball.LastY;
            }
            if(tempX != ball.LastX || tempY != ball.LastY) {
                Console.SetCursorPosition(ball.LastX, ball.LastY);
                Console.Write(" ");
            }
            ball.LastX = tempX;
            ball.X += ball.Speed * Math.Cos(ball.Direction) * deltaTime;
            ball.LastY = tempY;
            ball.Y += ball.Speed * Math.Sin(ball.Direction) * deltaTime;
            Console.SetCursorPosition(ball.LastX, ball.LastY);
            Console.Write("x");
            /// game end

            lastTotalMilliseconds += deltaTime;
        } 
    }
    public static void UserInputHandler(object sender, ConsoleKey key) {
        if(key == ConsoleKey.UpArrow && player.Y != 1)
            player.Direction = 0;
        if(key == ConsoleKey.DownArrow && player.Y != config.Height - 1) 
            player.Direction = 1;
        if(key == ConsoleKey.R)  {running = false; return;}
        userEvent.StartUserInput();
    }
    public static ConsoleKey SelectOption(params ConsoleKey[] acceptableKeys) {

        ConsoleKeyInfo userInput;
        do {
            userInput = Console.ReadKey(true);
        } while(acceptableKeys.All(key => key != userInput.Key));
        return userInput.Key;
    }
    static void ShowResolution() {
        ConsoleKey userInput;
        do {
            Console.Clear();
            Console.WriteLine("[Z] to go back");
            for(int i = 1; i < config.Height; i++) {
                Console.WriteLine(new String('=', config.Width));
            }
            userInput = SelectOption(ConsoleKey.Z, ConsoleKey.RightArrow, ConsoleKey.LeftArrow, ConsoleKey.UpArrow, ConsoleKey.DownArrow);
            switch(userInput) {
                case ConsoleKey.RightArrow:
                    config.Width++;
                    break;
                case ConsoleKey.LeftArrow:
                    config.Width--;
                    break;
                case ConsoleKey.UpArrow:
                    config.Height--;
                    break;
                case ConsoleKey.DownArrow:
                    config.Height++;
                    break;
                case ConsoleKey.Z:
                    SaveConfig();
                    break;
            }
        } while(userInput != ConsoleKey.Z);
    }
    static void SaveConfig() {
        File.WriteAllText(dataFolder, JsonSerializer.Serialize(config));
    }

    class Config {

        public int Width {get; set;}  = 64;
        public int Height {get; set;} = 8;
        public bool CurveBall {get; set;} = false;
        public int FPS {get; set;} = 60;
        public int Score {get; set;} = 0;
    }

    class Pong {
        public int LastDirection {get; set;} = -1;
        public int Speed {get;set;} = 0;
        public int X {get; set;} = 2;
        public int Y {get; set;} = 5;

        public int Direction {get;set;} = -1;

        public void Draw() {
            Console.SetCursorPosition(X, Y - 1);
            Console.Write("#");
            Console.SetCursorPosition(X, Y);
            Console.Write("#");
            Console.SetCursorPosition(X, Y + 1);
            Console.Write("#");
        }
        public void Update() {
            if(Direction == 0) {
                Y--;
                Console.SetCursorPosition(X, Y + 2);
                Console.Write(" ");
                Console.SetCursorPosition(X, Y - 1);
                Console.Write("#");
            } else if(Direction == 1) {
                Y++;
                Console.SetCursorPosition(X, Y - 2);
                Console.Write(" ");
                Console.SetCursorPosition(X, Y + 1);
                Console.Write("#");
            }
            LastDirection = Direction;
            Direction = -1;
        }
    }
    class Ball {
        public double X {get;set;}
        public double Y {get;set;} 
        public int LastX {get;set;} 
        public int LastY {get;set;} 
        public double Speed {get;set;}
        public double Direction {get;set;} 
    }

    
}
public class UserEventLogic {
    public event EventHandler<ConsoleKey> UserInputCompleted;
    public void StartUserInput() {
        var key = Program.SelectOption(ConsoleKey.DownArrow, ConsoleKey.UpArrow, ConsoleKey.R);
        OnUserInputCompleted(key);
    }

    protected virtual void OnUserInputCompleted(ConsoleKey key) {
        UserInputCompleted?.Invoke(this, key);
    }           
}