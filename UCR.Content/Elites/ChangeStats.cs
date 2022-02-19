using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UltimateCustomRun.Elites
{
    public static class ChangeStats
    {
        public static void ChangeTierOne(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(6f),
                x => x.MatchStsfld("RoR2.CombatDirector", "baseEliteCostMultiplier"),
                x => x.MatchLdcR4(2f),
                x => x.MatchStsfld("RoR2.CombatDirector", "baseEliteDamageBoostCoefficient"),
                x => x.MatchLdcR4(4f),
                x => x.MatchStsfld("RoR2.CombatDirector", "baseEliteHealthBoostCoefficient")
            );
            /*
            c.Next.Operand = Main.T1EliteCostMultiplier.Value;
            c.Index += 2;
            c.Next.Operand = Main.T1EliteDamageMultiplier.Value;
            c.Index += 2;
            c.Next.Operand = Main.T1EliteHealthMultiplier.Value;
            */
        }
        public static void ChangeTierTwo(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStsfld("RoR2.CombatDirector", "baseEliteCostMultiplier"),
                x => x.MatchLdcR4(6f),
                x => x.MatchMul()
            );
        }
    }
}
