﻿using BirthdayBot.BackgroundServices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BirthdayBot
{
    /// <summary>
    /// Handles the execution of periodic background tasks.
    /// </summary>
    class BackgroundServiceRunner
    {
        // Amount of idle time between each round of task execution, in seconds.
        const int Interval = 8 * 60;
        // Amount of time between start and first round of processing, in seconds.
        const int StartDelay = 60;

        const string LogName = nameof(BackgroundServiceRunner);

        private List<BackgroundService> _workers;
        private readonly CancellationTokenSource _workerCancel;
        private Task _workerTask;

        internal BirthdayRoleUpdate BirthdayUpdater { get; }

        public BackgroundServiceRunner(BirthdayBot instance)
        {
            _workerCancel = new CancellationTokenSource();
            BirthdayUpdater = new BirthdayRoleUpdate(instance);
            _workers = new List<BackgroundService>()
            {
                {new GuildStatistics(instance)},
                {new Heartbeat(instance)},
                {BirthdayUpdater}
            };
        }

        public void Start()
        {
            _workerTask = Task.Factory.StartNew(WorkerLoop, _workerCancel.Token,
                                                TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task Cancel()
        {
            _workerCancel.Cancel();
            await _workerTask;
        }

        /// <summary>
        /// *The* background task. Executes service tasks and handles errors.
        /// </summary>
        private async Task WorkerLoop()
        {
#if !DEBUG
            // Start an initial delay before tasks begin running
            Program.Log(LogName, $"Delaying first background execution by {StartDelay} seconds.");
            try { await Task.Delay(StartDelay * 1000, _workerCancel.Token); }
            catch (TaskCanceledException) { return; }
#else
            Program.Log(LogName, "Debug build - skipping initial processing delay.");
#endif
            while (!_workerCancel.IsCancellationRequested)
            {
                // Initiate background tasks
                var tasks = new List<Task>();
                foreach (var service in _workers) tasks.Add(service.OnTick());
                var alltasks = Task.WhenAll(tasks);

                // Await and check result
                // Cancellation token not checked at this point...
                try
                {
                    await alltasks;
                }
                catch (Exception ex)
                {
                    var exs = alltasks.Exception;
                    if (exs != null)
                    {
                        Program.Log(LogName, $"{exs.InnerExceptions.Count} exception(s) during background task execution:");
                        foreach (var iex in exs.InnerExceptions)
                        {
                            Program.Log(LogName, iex.Message);
                        }
                    }
                    else
                    {
                        Program.Log(LogName, ex.ToString());
                    }
                }

                try { await Task.Delay(Interval * 1000, _workerCancel.Token); }
                catch (TaskCanceledException) { return; }
            }
        }
    }
}