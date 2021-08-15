using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;

class Program {
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
        player = new();
        enemy = new();
        player.Draw();
        while(true) {
            ConsoleKey userInput = SelectOption(ConsoleKey.UpArrow, ConsoleKey.DownArrow);
            int direction = -1;
            if(userInput == ConsoleKey.UpArrow && player.Y != 1)
                direction = 0;
            if(userInput == ConsoleKey.DownArrow && player.Y != config.Height - 1) 
                direction = 1;
            
            player.Update(direction);
            //await Task.Delay(8);
        } 
    }

    static ConsoleKey SelectOption(params ConsoleKey[] acceptableKeys) {

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
    enum GameState {
        ERROR = -1,
        NOT_RUNNING,
        RUNNING
    }

    class Config {

        public int Width {get; set;}  = 64;
        public int Height {get; set;} = 8;
        public int FPS {get; set;} = 60;
        public int Score {get; set;} = 0;
    }

    class Pong {
        public int LastDirection {get; set;} = -1;
        public int X {get; set;} = 2;
        public int Y {get; set;} = 5;

        public void Draw() {
            Console.SetCursorPosition(X, Y - 1);
            Console.Write("#");
            Console.SetCursorPosition(X, Y);
            Console.Write("#");
            Console.SetCursorPosition(X, Y + 1);
            Console.Write("#");
        }
        public void Update(int direction) {
            if(direction == 0) {
                Y--;
                Console.SetCursorPosition(X, Y + 2);
                Console.Write(" ");
                Console.SetCursorPosition(X, Y - 1);
                Console.Write("#");
            } else if(direction == 1) {
                Y++;
                Console.SetCursorPosition(X, Y - 2);
                Console.Write(" ");
                Console.SetCursorPosition(X, Y + 1);
                Console.Write("#");
            }
        }
    }
}


