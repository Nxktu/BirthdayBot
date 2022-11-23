﻿namespace BirthdayBot;
class Program {
    private static ShardManager? _bot;
    private static readonly DateTimeOffset _botStartTime = DateTimeOffset.UtcNow;

    /// <summary>
    /// Returns the amount of time the program has been running in a human-readable format.
    /// </summary>
    public static string BotUptime => (DateTimeOffset.UtcNow - _botStartTime).ToString("d' days, 'hh':'mm':'ss");

    static async Task Main() {
        Configuration? cfg = null;
        try {
            cfg = new Configuration();
        } catch (Exception ex) {
            Console.WriteLine(ex);
            Environment.Exit((int)ExitCodes.ConfigError);
        }

        Console.CancelKeyPress += OnCancelKeyPressed;
        _bot = new ShardManager(cfg);

        await Task.Delay(-1);
    }

    /// <summary>
    /// Sends a formatted message to console.
    /// </summary>
    public static void Log(string source, string message) {
        var ts = DateTime.Now;
        var ls = new string[] { "\r\n", "\n" };
        foreach (var item in message.Split(ls, StringSplitOptions.None))
            Console.WriteLine($"{ts:s} [{source}] {item}");
    }

    private static void OnCancelKeyPressed(object? sender, ConsoleCancelEventArgs e) {
        e.Cancel = true;
        Log("Shutdown", "Captured cancel key; sending shutdown.");
        ProgramStop();
    }

    private static bool _stopping = false;
    public static void ProgramStop() {
        if (_stopping) return;
        _stopping = true;
        Log(nameof(Program), "Shutting down...");

        var dispose = Task.Run(_bot!.Dispose);
        if (!dispose.Wait(30000)) {
            Log(nameof(Program), "Disconnection is taking too long. Will force exit.");
            Environment.ExitCode &= (int)ExitCodes.ForcedExit;
        }
        Log(nameof(Program), $"Uptime: {BotUptime}");
        Environment.Exit(Environment.ExitCode);
    }

    [Flags]
    public enum ExitCodes {
        Normal = 0x0,
        ForcedExit = 0x1,
        ConfigError = 0x2,
        DatabaseError = 0x4,
        DeadShardThreshold = 0x8,
        BadCommand = 0x10,
    }
}
