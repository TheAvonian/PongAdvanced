using System;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

long timeSinceLastCallMS = DateTimeOffset.Now.ToUnixTimeMilliseconds();
GameState gameState = GameState.NOT_RUNNING;

int fpsVal; 
Stopwatch fpsStopwatch = new();
fpsStopwatch.Start();
do {
    fpsVal = (int)Math.Round(1.0 / ((int)fpsStopwatch.ElapsedTicks / 10000000.0));
    fpsStopwatch.Restart();
   // var timeDiff = DateTimeOffset.Now.ToUnixTimeMilliseconds() - timeSinceLastCallMS;
   // fpsVal = (int)Math.Round( 1.0 / ( ((int)timeDiff == 0 ? 1000.0 : (int)timeDiff) / 1000.0 ));
    Console.Write($"\rFPS: {fpsVal}    ");
    //ConsoleKeyInfo key = Console.ReadKey(true);
    //Task.Run(GetNextInput);
    //timeSinceLastCallMS = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    await Task.Delay(60 / fpsVal);

} while(true);

/*static Task<ConsoleKeyInfo> GetNextInput() {
    
}*/

enum GameState {
    ERROR = -1,
    NOT_RUNNING,
    RUNNING
}