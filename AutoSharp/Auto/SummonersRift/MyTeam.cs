using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Auto.SummonersRift
{
    public static class MyTeam
    {
        public enum Roles
        {
            Toplaner,
            Midlaner,
            Jungler,
            Support,
            ADC,
            Unknown
        }

        public static Obj_AI_Hero Toplaner;
        public static Obj_AI_Hero Midlaner;
        public static Obj_AI_Hero Jungler;
        public static Obj_AI_Hero Support;
        public static Obj_AI_Hero Player = Heroes.Player;
        public static Obj_AI_Hero ADC;
        public static Roles MyRole = Roles.Unknown;

        public static void Update()
        {
            if (Jungler == null)
            {
                Jungler = Heroes.AllyHeroes.FirstOrDefault(h => !h.IsMe && h.GetSpellSlot("summonersmite") != SpellSlot.Unknown || h.HasItem(ItemId.Hunters_Machete));
            }
            if (Support == null)
            {
                Support = Heroes.AllyHeroes.FirstOrDefault(h => !h.IsMe && h != Jungler && h != ADC && (h.Distance(Player.Team == GameObjectTeam.Order ? Map.BottomLane.Blue_Outer_Turret.To3D() : Map.BottomLane.Red_Outer_Turret.To3D()) < 4500));
            }
            if (Toplaner == null)
            {
                Toplaner = Heroes.AllyHeroes.FirstOrDefault(h => !h.IsMe && h != Jungler && h.Distance(Player.Team == GameObjectTeam.Order ? Map.TopLane.Blue_Outer_Turret.To3D() : Map.TopLane.Red_Outer_Turret.To3D()) < 4500) ?? Heroes.AllyHeroes.FirstOrDefault(h => h != Jungler && h.Distance(Player.Team == GameObjectTeam.Order ? Map.TopLane.Blue_Inner_Turret.To3D() : Map.TopLane.Red_Inner_Turret.To3D()) < 1500);
            }
            if (Midlaner == null)
            {
                Midlaner = Heroes.AllyHeroes.FirstOrDefault(h => !h.IsMe && h != Jungler && h.Distance(Player.Team == GameObjectTeam.Order ? Map.MidLane.Blue_Outer_Turret.To3D() : Map.MidLane.Red_Outer_Turret.To3D()) < 4500) ?? Heroes.AllyHeroes.FirstOrDefault(h => h != Jungler && h.Distance(Player.Team == GameObjectTeam.Order ? Map.MidLane.Blue_Inner_Turret.To3D() : Map.MidLane.Red_Inner_Turret.To3D()) < 1500);
            }
            if (ADC == null)
            {
                ADC = Heroes.AllyHeroes.FirstOrDefault(h => !h.IsMe && h != Jungler && h != Support && h.Distance(Player.Team == GameObjectTeam.Order ? Map.BottomLane.Blue_Outer_Turret.To3D() : Map.BottomLane.Red_Outer_Turret.To3D()) < 4500);
            }
        }
    }
}
