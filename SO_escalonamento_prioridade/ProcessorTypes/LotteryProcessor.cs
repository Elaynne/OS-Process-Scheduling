using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO_escalonamento_prioridade.ProcessorTypes
{
    public class LotteryProcessor : Processor
    {
        protected int currentExecutingProcessIndex;
        protected int quantum;
        protected int currentQuantum = 0;

        private Process selectedProcess = null;

        public LotteryProcessor(int max_threads) : base(max_threads)
        {
            currentExecutingProcessIndex = -1;
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

            if (selectedProcess == null)
            {
                int ticketNumber = (new Random()).Next(0, GetTotalTickets() + 1);
                selectedProcess = PickProcessByTicketNumber(ticketNumber);
                currentQuantum = 0;
                quantum = (selectedProcess.priority + 1) * 10;
            }

            Process retProcess = selectedProcess;
            currentQuantum++;

            if (currentQuantum >= quantum)
            {
                selectedProcess = null;
            }
            return retProcess;
        }

        private int GetTotalTickets()
        {
            int sum = 0;
            foreach(Process p in executingProcesses)
            {
                sum += (p.priority + 1) * 10;
            }
            return sum;
        }

        private Process PickProcessByTicketNumber(int ticket)
        {
            foreach(Process p in executingProcesses)
            {
                int ticketQuantity = (p.priority + 1) * 10;
                if (ticket <= ticketQuantity)
                {
                    return p;
                }
                else
                {
                    ticket -= ticketQuantity;
                }
            }

            Console.WriteLine("Ticket doesnt exist: " + ticket);
            return null;
        }
    }
}
