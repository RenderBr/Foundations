using IL.Terraria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using System.Timers;
using Terraria;

namespace Foundations.Api
{
    public class TimeFreeze
    {
        public bool enabled = false;
        public System.Timers.Timer FreezeTimer;
        public TimeOfDay time;
        public double customTime = 0;
        public void Start(TimeOfDay _t)
        {
            enabled = true;
            customTime = Terraria.Main.time;
            time = _t;


            FreezeTimer = new(1000)
            {
                AutoReset = true
            };
            FreezeTimer.Elapsed += async (_, x)
                => await FreezeTimerCallback(x);
            FreezeTimer.Start();
        }

        public void Stop()
        {
            enabled = false;
            FreezeTimer.Stop();
        }

        private async Task FreezeTimerCallback(ElapsedEventArgs _)
        {
            switch (time)
            {
                case TimeOfDay.Day:
                    TSPlayer.Server.SetTime(true, 0.0);
                    break;
                case TimeOfDay.Night:
                    TSPlayer.Server.SetTime(false, 0.0);
                    break;
                case TimeOfDay.Midnight:
                    TSPlayer.Server.SetTime(false, 16200.0);
                    break;
                case TimeOfDay.Noon:
                    TSPlayer.Server.SetTime(true, 27000.0);
                    break;
                case TimeOfDay.Custom:
                    TSPlayer.Server.SetTime(false, 27000.0);
                    break;
            }

        }

        public enum TimeOfDay {
            Day = 0,
            Night = 1,
            Midnight = 2,
            Noon = 3,
            Custom = 4,
        }
    }

}
