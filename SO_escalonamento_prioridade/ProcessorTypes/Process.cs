using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO_escalonamento_prioridade
{
    public class Process
    {
        public int id { get; set; }
        public int submission_time { get; set; }
        public int priority { get; set; }
        public int execution_time { get; set; }
        public int block_time { get; set; }

        private int consumedTime;
        private int startTime;

        public int turn_around;
        public int response_time;
        public int wait_time;
        private int blocked_time;
        private int last_blocked_time;

        public Process(int id, int submission_time, int priority, int execution_time, int block_time)
        {
            this.id = id;
            this.submission_time = submission_time;
            this.priority = priority;
            this.execution_time = execution_time;
            this.block_time = block_time;
            this.consumedTime = 0;
            this.startTime = -1;
            this.last_blocked_time = -1;
            this.blocked_time = 0;
        }

        public void Start(int startTime)
        {
            this.startTime = startTime;
            this.response_time = startTime - submission_time;
        }

        public void ConsumeTime(int consumeTime)
        {
            if (this.last_blocked_time >= 0)
            {
                blocked_time += consumeTime - last_blocked_time - 1;
            }

            last_blocked_time = consumeTime;
            consumedTime++;
        }

        public void Finish(int finishTime)
        {
            wait_time = startTime - submission_time + blocked_time;
            turn_around = finishTime - submission_time;
        }

        public float HRRN(int time)
        {
            return ((time - submission_time) + execution_time + block_time) / (float)execution_time;
        }

        public bool IsProcessFinished
        {
            get
            {
                return consumedTime >= execution_time;
            }
        }

        public bool IsStarted
        {
            get
            {
                return startTime >= 0;
            }
        }
    }
}
