using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CF.VRent.Common;
using CF.VRent.Job;
using CF.VRent.Log;
using CF.VRent.Log.ConcreteLog;
using System.Threading.Tasks;

namespace VRentDJ.Utility
{
    internal class TimePending
    {
        /// <summary>
        /// Execute timing
        /// </summary>
        /// <param name="baseTime"></param>
        /// <param name="baseDate"></param>
        /// <param name="baseKey"></param>
        /// <param name="IsJobRunning"></param>
        /// <returns></returns>
        public static bool Pending(TimeSpan baseTime, DateTime baseDate, JobMessage baseKey, bool IsJobRunning = false)
        {
            //When job is running, stop timing
            if (!IsJobRunning)
            {
                TimeSpan interval = DateTime.Now.Subtract(baseDate);

                var subtract = interval.Subtract(baseTime);

                //set accuracy rating to second, as it is impossible to do something under nanosecond
                var secondTicks = Math.Floor((subtract.Ticks / 10000000).ToDouble());

                Task.Factory.StartNew(r =>
                {
                    LogInfor.GetLogger<TimerLogger>().WriteInfo(baseKey.Format() + " - Left time[" + (secondTicks * -1).ToString() + "s]", "", "");
                }, TaskCreationOptions.PreferFairness);
                
                //when job is running at the first time, interval could greater than 0
                if (secondTicks >= 0)
                {
                    Task.Factory.StartNew(r =>
                    {
                        LogInfor.GetLogger<TimerLogger>().WriteInfo(baseKey.Format() + " - Start job", "", "");
                    }, TaskCreationOptions.PreferFairness);
                    
                    return false;
                }
                else
                {
                    var halfofsubstract = Math.Abs(subtract.Ticks / 20000);

                    //pending for half of the diffentce between interval and baseTimes to free the CPU resouces for this thread
                    Thread.Sleep(Convert.ToInt32(halfofsubstract));
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
    }

    internal class JobMessage
    {
        public string Category { get; set; }

        public Guid BaseKey { get; set; }

        public string Name { get; set; }

        public string Format()
        {
            return Category + "|" + Name + "|" + BaseKey.ToString("N");
        }
    }
}
