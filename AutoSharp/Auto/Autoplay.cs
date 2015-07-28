using System.Diagnostics;
using System.Security.Permissions;
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
                    Game.OnUpdate += args => { MyTeam.Update(); };
                    Game.OnEnd += args => Exit();
                    SRManager.Load();
                    break;
                }
                case GameMapId.CrystalScar:
                {
                    break;
                }
                case GameMapId.TwistedTreeline:
                {
                    break;
                }
                case GameMapId.HowlingAbyss:
                {
                    HAManager.Load();
                    break;
                }
                default:
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
                    SRManager.Unload();
                    break;
                }
                case GameMapId.HowlingAbyss:
                {
                    HAManager.Unload();
                    break;
                }
                default:
                {
                    HAManager.Unload();
                    break;
                }
            }
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static void Exit()
        {
            Process.Start("taskkill /f /im \"LeagueSharp of Legends.exe\"");
        }
    }
}
