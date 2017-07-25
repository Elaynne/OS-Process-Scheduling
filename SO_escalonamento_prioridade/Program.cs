using SO_escalonamento_prioridade.ProcessorTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO_escalonamento_prioridade
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] entradas = new string[]
            {
                "Entrada\\cenario1.txt",
                "Entrada\\cenario2.txt",
                "Entrada\\cenario3.txt",
                "Entrada\\cenario4.txt",
                "Entrada\\cenario5.txt",
            };
            foreach (string entrada in entradas)
            {
                Dictionary<Processor,string> processorsType = new Dictionary<Processor, string>()
                {
                    { new PriorityQueueProcessor(100), "Algoritmo: Priority Queue, 100 threads" },
                    { new PriorityQueueProcessor(200), "Algoritmo: Priority Queue, 200 threads" },
                    { new RoundRobinProcessor(100, 4), "Algoritmo: Round Robin, 100 threads, quantum 4" },
                    { new RoundRobinProcessor(200, 4), "Algoritmo: Round Robin, 200 threads, quantum 4" },
                    { new LotteryProcessor(100),       "Algoritmo: Lottery, 100 threads" },
                    { new LotteryProcessor(200),       "Algoritmo: Lottery, 200 threads" },
                    { new HRRNProcessor(100),          "Algoritmo: HRRN, 100 threads" },
                    { new HRRNProcessor(200),          "Algoritmo: HRRN, 200 threads" },
                };

                foreach(var kpprocessor in processorsType)
                {
                    Console.WriteLine(string.Format("Rodando para entrada: {0}", entrada));
                    List<Process> processes = new List<Process>();

                    using (StreamReader sr = new StreamReader(entrada))
                    {
                        String line = null;
                        while (!sr.EndOfStream)
                        {
                            line = sr.ReadLine();
                            string[] split = line.Split(',');
                            int process_id = int.Parse(split[0]);
                            int submission_time = int.Parse(split[1]);
                            int priority = int.Parse(split[2]);
                            int execution_time = int.Parse(split[3]);
                            int block_time = int.Parse(split[4]);
                            Process p = new Process(process_id, submission_time, priority, execution_time, block_time);
                            processes.Add(p);
                        }
                    }

                    processes = processes.OrderBy(p => p.submission_time).ToList();

                    //execução do programa começa aqui
                    Console.WriteLine(kpprocessor.Value);
                    Processor processor = kpprocessor.Key;
                    processor.Execute(processes);

                    Console.WriteLine("-------------------------------------------------");
                }
            }
            Console.WriteLine("Programa finalizou");
            //Console.ReadLine();
        }
    }
}
