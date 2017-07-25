using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO_escalonamento_prioridade.ProcessorTypes
{
    public class RoundRobinProcessor : Processor
    {
        protected int currentExecutingProcessIndex;
        protected int quantum;
        protected int currentQuantum = 0;

        public RoundRobinProcessor(int max_threads, int quantum) : base(max_threads)
        {
            currentExecutingProcessIndex = 0;
            this.quantum = quantum;
        }

        public override void OnExecute(List<Process> processes)
        {
            time = processes[0].submission_time;
            consumedTime = time - 1;
            executingProcesses.Add(processes[0]);
            processes.RemoveAt(0);
            while (processes.Count > 0 || executingProcesses.Count > 0)
            {
                //adiciona os processos que foram submetidos nesse tempo para a lista
                //de processos em execução
                if (CanAddProcessToThreadList() && processes.Count > 0 && 
                    processes[0].submission_time <= time)
                {
                    Process startingProcess = processes[0];
                    processes.Remove(startingProcess);
                    executingProcesses.Add(startingProcess);
                    while (time >= startingProcess.submission_time &&
                           CanAddProcessToThreadList())
                    {
                        startingProcess = processes[0];
                        processes.Remove(startingProcess);
                        executingProcesses.Add(startingProcess);
                    }
                }

                Process processExecuting = GetProcessToExecute();
                if (processExecuting != null)
                {
                    if (!processExecuting.IsStarted)
                    {
                        processExecuting.Start(time);
                    }
                    processExecuting.ConsumeTime(time);
                    consumedTime++;
                    if (processExecuting.IsProcessFinished)
                    {
                        base.FinishThread(processExecuting, time);
                    }
                }

                time++;
            }
        }

        public Process GetProcessToExecute()
        {
            if (executingProcesses.Count == 0)
            {
                return null;
            }
            if (currentQuantum >= quantum) //ativa a preempção
            {
                currentExecutingProcessIndex = (currentExecutingProcessIndex + 1);
                currentQuantum = 0;
            }
            else
            {
                currentQuantum++;
            }
            currentExecutingProcessIndex = currentExecutingProcessIndex % executingProcesses.Count;
            Process p = executingProcesses[currentExecutingProcessIndex];
            return p;
        }
    }
}
