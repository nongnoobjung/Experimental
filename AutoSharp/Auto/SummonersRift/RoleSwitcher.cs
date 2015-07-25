using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSharp.Utils;

namespace AutoSharp.Auto.SummonersRift
{
    public static class RoleSwitcher
    {
        public static void Load()
        {
            if (MyTeam.Support == null)
            {
                Role.Support.Load();
                return;
            }
        }

        public static void UnloadAll()
        {
            Role.Support.Unload();
        }

        public static void ChooseBest()
        {
            if (MyTeam.Support == null)
            {
                if (Role.Support.State == Enums.BehaviorStates.Paused)
                {
                    Role.Support.Resume();
                }
                else
                {
                    Role.Support.Load();
                }
                return;
            }

        }

        public static void PauseAll()
        {
            Role.Support.Pause();
        }
    }
}
