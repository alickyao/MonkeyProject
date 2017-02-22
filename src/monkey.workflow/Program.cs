using System;
using System.Linq;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Threading;

namespace monkey.workflow
{

    class Program
    {
        static void Main(string[] args)
        {

            AutoResetEvent syncEvent = new AutoResetEvent(false);
            AutoResetEvent idleEvent = new AutoResetEvent(false);

            //参数
            var inputs = new Dictionary<string, object>() { { "MaxNumber", 100 } };
            //WorkflowApplication 为执行工作流（包括生命周期事件通知、执行控制、书签恢复和持久性）提供更丰富的模型。 此示例使用书签并且将 WorkflowApplication 用于承载工作流
            WorkflowApplication wfApp = new WorkflowApplication(new FlowchartNumberGuessWorkflow(), inputs);

            wfApp.Completed = delegate (WorkflowApplicationCompletedEventArgs e)
            {
                int Turns = Convert.ToInt32(e.Outputs["Turns"]);
                Console.WriteLine("Congratulations, you guessed the number in {0} turns.", Turns);
                syncEvent.Set();
            };


            wfApp.Aborted = delegate (WorkflowApplicationAbortedEventArgs e)
            {
                Console.WriteLine(e.Reason);
                syncEvent.Set();
            };

            wfApp.OnUnhandledException = delegate (WorkflowApplicationUnhandledExceptionEventArgs e)
            {
                Console.WriteLine(e.UnhandledException.ToString());
                return UnhandledExceptionAction.Terminate;
            };

            wfApp.Idle = delegate (WorkflowApplicationIdleEventArgs e)
            {
                idleEvent.Set();
            };


            wfApp.Run();

            // Loop until the workflow completes.
            //WaitHandle[] handles = new WaitHandle[] { syncEvent, idleEvent };
            //while (WaitHandle.WaitAny(handles) != 0)
            //{
            //    // Gather the user input and resume the bookmark.

            //}

            while (true)
            {
                bool validEntry = false;
                while (!validEntry)
                {
                    int Guess;
                    if (!Int32.TryParse(Console.ReadLine(), out Guess))
                    {
                        Console.WriteLine("Please enter an integer.");
                    }
                    else
                    {
                        validEntry = true;
                        wfApp.ResumeBookmark("EnterGuess", Guess);
                    }
                }
            }

        }
    }
}
