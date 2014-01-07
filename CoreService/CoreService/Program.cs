using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;
using System.Timers;


namespace CoreService
{
    public class RunCore
    {
        readonly Timer timer1;

        public RunCore()
        {
            timer1 = new Timer(1000) {AutoReset = true};
            
            timer1.Elapsed += (sender, EventArgs) => Console.WriteLine("Timer is {0} and system clock is {1}", '0', DateTime.Now);
        }
        public void Start() {timer1.Start();}
        public void Stop() {timer1.Stop();}

    }

    public class Program
    {
        public static void Main()
        {
            HostFactory.Run(x =>                                 //1
            {
                x.Service<RunCore>(s =>                        //2
                {
                    s.ConstructUsing(name => new RunCore());     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());               //5
                });
                x.RunAsLocalSystem();                            //6

                x.SetDescription("Sample Topshelf Host");        //7
                x.SetDisplayName("Stuff");                       //8
                x.SetServiceName("stuff");                       //9
            });                                                  //10
        }
    }
}
