﻿#region LICENSE

// Copyright 2014-2015 LeagueSharp
// Annie.cs is part of AutoSharp.
// 
// AutoSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AutoSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AutoSharp. If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

#endregion

using System;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Annie : PluginBase
    {
        public Annie()
        {
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 625);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 600);

            Q.SetTargetted(250, 1400);
            W.SetSkillshot(600, (float)(50 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(250, 200, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target);
                }
                if (W.CastCheck(Target, "ComboW"))
                {
                    W.Cast(Target);
                }

                if (R.CastCheck(Target, "ComboR"))
                {
                    R.Cast(Target);
                }
                CastE();
            }
        }

        public override void OnPossibleToInterrupt(Obj_AI_Hero unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.High || unit.IsAlly)
            {
                return;
            }
            if (GetPassiveStacks() >= 4)
            {
                if (Q.CastCheck(unit, "Interrupt.Q"))
                {
                    Q.Cast(unit);
                    return;
                }
                if (W.CastCheck(unit, "Interrupt.W"))
                {
                    W.Cast(unit);
                    return;
                }
            }
            if (GetPassiveStacks() == 3)
            {
                if (E.IsReady())
                {
                    E.Cast();
                    if (Q.CastCheck(unit, "Interrupt.Q"))
                    {
                        Q.Cast(unit);
                        return;
                    }
                    if (W.CastCheck(unit, "Interrupt.W"))
                    {
                        W.Cast(unit);
                        return;
                    }
                }
                if (Q.CastCheck(unit, "Interrupt.Q") && W.CastCheck(unit, "Interrupt.W"))
                {
                    Q.Cast(unit);
                    W.Cast(unit);
                }
            }
        }

        private void CastE()
        {
            if (GetPassiveStacks() < 4 && !ObjectManager.Player.IsRecalling())
            {
                E.Cast();
            }
        }

        //sosharp love xSalice
        private int GetPassiveStacks()
        {
            var buffs =
                ObjectManager.Player.Buffs.Where(
                    buff => (buff.Name.ToLower() == "pyromania" || buff.Name.ToLower() == "pyromania_particle"));
            var buffInstances = buffs as BuffInstance[] ?? buffs.ToArray();
            if (!buffInstances.Any())
            {
                return 0;
            }
            var buf = buffInstances.First();
            var count = buf.Count >= 4 ? 4 : buf.Count;
            return buf.Name.ToLower() == "pyromania_particle" ? 4 : count;
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboR", "Use R", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Interrupt.Q", "Use Q to Interrupt Spells", true);
            config.AddBool("Interrupt.W", "Use W to Interrupt Spells", true);
        }
    }
}