using System;
using System.Linq;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;

namespace monkey.workflow
{

    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("max", 100);
            dic.Add("turns", 10);
            WorkflowApplication my = new WorkflowApplication(new Workflow1(),dic);
            my.Run();
            Console.ReadLine();
        }
    }
}
