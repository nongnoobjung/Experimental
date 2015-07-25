using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AutoSharp.Auto.SummonersRift.Role
{
    public static class Support
    {
        private static Obj_AI_Hero Carry;
        private static Obj_AI_Hero TempCarry;
        private static Obj_AI_Hero Jungler;
        private static Obj_AI_Hero Bot = Heroes.Player;
        private static int TimeSinceLastCarrySwitch = 0;

        #region RoleManager
        //if true will pause role, not unload it!
        private static bool paused = false;
        public static Enums.BehaviorStates State = Enums.BehaviorStates.Unknown;

        /// <summary>
        /// Loads role and sets it's state to running.
        /// </summary>
        public static void Load()
        {
            State = Enums.BehaviorStates.Running;
            Game.OnUpdate += OnUpdate;
        }

        /// <summary>
        /// Unloads role and sets it's state back to stopped.
        /// </summary>
        public static void Unload()
        {
            State = Enums.BehaviorStates.Stopped;
            Game.OnUpdate -= OnUpdate;
        }

        /// <summary>
        /// Pauses a role and sets it's state to paused.
        /// </summary>
        public static void Pause()
        {
            State = Enums.BehaviorStates.Paused;
            paused = true;
        }

        /// <summary>
        /// Resumes a role, and sets it's state to running.
        /// </summary>
        public static void Resume()
        {
            State = Enums.BehaviorStates.Running;
            paused = false;
        }
        #endregion

        public static void OnUpdate(EventArgs args)
        {
            //stop orbwalking if dead/game ended/role is paused
            if (Bot.IsDead || HeadQuarters.EnemyHQ.Health < 200 || paused) 
            {
                Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.None;
                return;
            }
            if (Environment.TickCount - TimeSinceLastCarrySwitch > 120000)
            {
                SmartCarrySwitch();
            }
            if (Carry == null || Carry == Jungler)
            {
                FindCarry();
                return;
            }
            if (Carry.InFountain() || Carry.IsDead)
            {
                Roam();
                return;
            }

            SupportCarry();
        }

        /// <summary>
        /// Switching carries based on kda from highest to lowest
        /// </summary>
        public static void SmartCarrySwitch()
        {
            if (Carry.Level < Bot.Level || Bot.Level >= 7)
            {
                Carry = Heroes.AllyHeroes.OrderByDescending(
                    hero => (hero.ChampionsKilled / ((hero.Deaths != 0) ? hero.Deaths : 1))).FirstOrDefault();
            }
            TimeSinceLastCarrySwitch = Environment.TickCount;
        }

        /// <summary>
        /// This behavior will follow carry around in combo mode :D
        /// </summary>
        public static void SupportCarry()
        {
            Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.Combo;
            Program.Orbwalker.SetOrbwalkingPoint(Carry.ServerPosition.RandomizePosition());
        }

        /// <summary>
        /// This behavior will make the bot follow the tempcarry, the closest ally in range, as long as he's not the jungler.
        /// </summary>
        public static void Roam()
        {
            TempCarry = Heroes.AllyHeroes.OrderBy(h => h.Distance(Bot)).FirstOrDefault(h => h != Jungler && h != Carry);
            if (TempCarry == null)
            {
                //should probably go base or switch role here
                return;
            }
            Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.Combo;
            Program.Orbwalker.SetOrbwalkingPoint(TempCarry.ServerPosition.RandomizePosition());
        }

        /// <summary>
        /// This behavior will make the bot go to bottom lane in and wait safely at the outer turret for a carry to support.
        /// </summary>
        public static void FindCarry()
        {
            Jungler = MyTeam.Jungler;
            Carry = MyTeam.ADC;
            var BotLaneTurretPos = Bot.Team == GameObjectTeam.Order ? Map.BottomLane.Blue_Outer_Turret.RandomizePosition() : Map.BottomLane.Red_Outer_Turret.RandomizePosition();

            if (Bot.GoldTotal >= 400 && Bot.GoldTotal <= 600)
            {
                if (Bot.InFountain())
                {
                    Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.Combo;
                    Program.Orbwalker.SetOrbwalkingPoint(BotLaneTurretPos);
                }
            }
        }
    }
}
