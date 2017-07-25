using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO_escalonamento_prioridade.ProcessorTypes
{
    public class PriorityQueueProcessor : Processor
    {
        public PriorityQueueProcessor(int max_threads) : base(max_threads) { }

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
                if (CanAddProcessToThreadList() && processes.Count > 0 && processes[0].submission_time <= time)
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
                    //ativa a preempção
                    executingProcesses = executingProcesses.OrderByDescending(p => p.priority).OrderByDescending(p => p.submission_time).ToList();
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
            if (executingProcesses.Count > 0)
            {
                return executingProcesses[0];
            }

            return null;
        }
    }
}
