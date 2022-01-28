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

namespace UltimateCustomRun
{
    public static class BrilliantBehemoth
    {
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.6f)
            );
            c.Next.Operand = Main.BehemothDamage.Value;
        }
        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.5f),
                x => x.MatchLdcR4(2.5f)
            );
            c.Next.Operand = Main.BehemothAoe.Value;
            c.Index += 1;
            c.Next.Operand = Main.BehemothAoeStack.Value;
        }
    }
}
