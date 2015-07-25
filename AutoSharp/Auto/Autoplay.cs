using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSharp.Auto.HowlingAbyss;
using AutoSharp.Auto.SummonersRift;
using LeagueSharp;

namespace AutoSharp.Auto
{
    public static class Autoplay
    {
        public static void Load()
        {
            switch (Game.MapId)
            {
                case GameMapId.SummonersRift:
                {
                    return; //#TODO: Disabled untill fixed;
                    Game.OnUpdate += UpdateMyTeam => { MyTeam.Update(); };
                    SRManager.Load();
                    break;
                }
                case GameMapId.HowlingAbyss:
                {
                    HAManager.Load();
                    break;
                }
            }
        }

        public static void Unload()
        {
            switch (Game.MapId)
            {
                case GameMapId.SummonersRift:
                {
                    return; //#TODO: Disabled untill fixed;
                    SRManager.Unload();
                    break;
                }
                case GameMapId.HowlingAbyss:
                {
                    HAManager.Unload();
                    break;
                }
            }
        }
    }
}
