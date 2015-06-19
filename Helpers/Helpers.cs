using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers
{
    public static class Helper
    {
        public static bool LoopAttempt<T1>(int maxAttempts, int timeoutSeconds, T1 state, Func<T1, bool> haltCondition, Func<T1> attemptedAction, Func<Exception, T1> exceptionAction = null, bool forceFirstTry = false)
        {
            int attempts = maxAttempts;
            bool halt = haltCondition(state);
            while ((!halt && attempts > 0) || forceFirstTry)
            {
                forceFirstTry = false;
                attempts--;
                Console.WriteLine("\tAttempt {0}", maxAttempts - attempts);
                var lastattempt = DateTime.UtcNow;

                try
                {
                    state = attemptedAction();
                }
                catch (Exception e)
                {
                    if (exceptionAction != null) state = exceptionAction(e);
                }
                finally
                {
                    var duration = DateTime.UtcNow - lastattempt;
                    var secondsLeft = timeoutSeconds - duration.Seconds;
                    halt = haltCondition(state);
                    if (!halt && secondsLeft > 0)
                    {
                        Thread.Sleep(secondsLeft * 1000);
                    }
                }
            }

            return halt;
        }

        public static bool StartTask(Action action, TimeSpan timeout)
        {
            var actualTask = new Task<bool>(() =>
            {
                var longRunningTask = new Task(() =>
                {
                        action();
                }, TaskCreationOptions.LongRunning);

                longRunningTask.Start();

                if (longRunningTask.Wait(timeout)) return true;

                return false;
            });

            actualTask.Start();

            actualTask.Wait();
            return actualTask.Result;
        }

        public static void StartProcess(string name, string args, int timeoutInMS)
        {
            var p = StartProcess(name, args);
            Task.Run(async delegate  { 
                await Task.Delay(timeoutInMS);
                p.Kill();
                Console.WriteLine("Killed {0} {1} {2}", p.Id, name, args);
            });
        }
        public static Process StartProcess(string name, string args = null)
        {
            Console.WriteLine("{0} {1}", name, args ?? string.Empty);
            var p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                FileName = name,
                Arguments = args ?? string.Empty,
                //CreateNoWindow = true,
                //WindowStyle = ProcessWindowStyle.Normal,
                CreateNoWindow = false,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Minimized
            };
            try
            {
                p.Start();
                return p;
            }
            catch (Exception e) { return null; }
        }
    }
}
