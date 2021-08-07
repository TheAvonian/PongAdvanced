using System.Text;
using System.Text.Json;
using System.Diagnostics;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;

const int FPS = 60;

//////////////////// START 

/// CONFIG START
Config config;
Console.Write("Loading...");
string dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AdvancedPong");
if(!Directory.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);
dataFolder = Path.Combine(dataFolder, "save.dat");
if(File.Exists(dataFolder)) {
    var fileDataString = File.ReadAllText(dataFolder);
    config = JsonSerializer.Deserialize<Config>(fileDataString);
} else {
    config = new();
    File.WriteAllText(dataFolder, JsonSerializer.Serialize(config));
}
/// CONFIG END

ShowMenu(ref config);

//////////////////// END

static void ShowMenu(ref Config config) {
    Console.Clear();
    Console.WriteLine("----------------------------------------------------------------");
    Console.WriteLine("      :::::::::       ::::::::       ::::    :::       :::::::: ");
    Console.WriteLine("     :+:    :+:     :+:    :+:      :+:+:   :+:      :+:    :+: ");
    Console.WriteLine("    +:+    +:+     +:+    +:+      :+:+:+  +:+      +:+         ");
    Console.WriteLine("   +#++:++#+      +#+    +:+      +#+ +:+ +#+      :#:          ");
    Console.WriteLine("  +#+            +#+    +#+      +#+  +#+#+#      +#+   +#+#    ");
    Console.WriteLine(" #+#            #+#    #+#      #+#   #+#+#      #+#    #+#     ");
    Console.WriteLine("###             ########       ###    ####       ########       ");
    Console.WriteLine("----------------------------------------------------------------\n");
    Console.WriteLine("[1] Start Pong");
    Console.WriteLine("[2] Options");
    Console.WriteLine("[3] Quit");
    ConsoleKey userInput = SelectOption(ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3);
    switch(userInput) {
        case ConsoleKey.D1:
            RunGame(ref config);
            break;
        case ConsoleKey.D2:
            ShowOptions(ref config);
            break;
        case ConsoleKey.D3:
            Environment.Exit(0);
            break;
        default:
            Environment.Exit(0);
            break;
    }
}

static void ShowOptions(ref Config config) {
    
}

static void RunGame(ref Config config) {

    // SET UP GAME WITH CONFIG

    Stopwatch timer = new();
    timer.Start();
    do {
        if(timer.ElapsedTicks > FPS * 10) {
            timer.Restart();
            // GET USER INPUT
            // GET AI INPUT
            // UPDATE MOVEMENT
            // 
        }
    } while(true);   
}

static ConsoleKey SelectOption(params ConsoleKey[] acceptableKeys) {
    ConsoleKeyInfo userInput;
    do {
        userInput = Console.ReadKey(true);
        if(userInput.Key == ConsoleKey.Escape)
            Environment.Exit(0);
    } while(acceptableKeys.All(key => key != userInput.Key));
    return userInput.Key;
}
enum GameState {
    ERROR = -1,
    NOT_RUNNING,
    RUNNING
}

class Config {
    public int Width {get; set;}  = 100;
    public int Height {get; set;} = 30;

}