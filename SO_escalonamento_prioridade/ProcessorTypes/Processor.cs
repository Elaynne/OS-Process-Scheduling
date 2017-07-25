using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO_escalonamento_prioridade
{
    public abstract class Processor
    {
        protected int max_threads;
        protected List<Process> executingProcesses;
        private List<Process> finishedProcesses;
        protected int time;
        protected int consumedTime;

        public Processor(int max_threads)
        {
            this.max_threads = max_threads;
            executingProcesses = new List<Process>(this.max_threads);
            finishedProcesses = new List<Process>();
            time = 0;
            consumedTime = 0;
        }

        public abstract void OnExecute(List<Process> processes);

        public void Execute(List<Process> processes)
        {
            OnExecute(processes);
            PrintStatistics();
        }

        public bool CanAddProcessToThreadList()
        {
            return executingProcesses.Count < max_threads;
        }

        public void FinishThread(Process p, int time)
        {
            p.Finish(time);
            executingProcesses.Remove(p);
            finishedProcesses.Add(p);
        }

        private float Avg(Func<Process, int> processProperty)
        {
            float avg = 0;
            foreach (Process p in finishedProcesses)
            {
                avg += processProperty(p);
            }

            return avg / (float)finishedProcesses.Count;
        }

        public void PrintStatistics()
        {
            Console.WriteLine(string.Format("Avg Turn Around {0}", Avg(p => p.turn_around).ToString("#.##")));
            Console.WriteLine(string.Format("Avg Wait Time {0}", Avg(p => p.wait_time).ToString("#.##")));
            Console.WriteLine(string.Format("Avg Response Time {0}", Avg(p => p.response_time).ToString("#.##")));
            Console.WriteLine(string.Format("Avg Service Time {0}", Avg(p => p.execution_time).ToString("#.##")));
            Console.WriteLine(string.Format("Processor Usage {0}%", (((float)consumedTime / (float)time)*100).ToString("#.##")));
            Console.WriteLine(string.Format("Simulation Duration {0}", time.ToString("#.##")));
        }
    }
}
