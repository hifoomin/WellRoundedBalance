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
    public static class WillOTheWisp
    {
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(3.5f),
                x => x.MatchLdcR4(1f),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(1),
                x => x.MatchSub(),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.8f)
                // ik this is guh huge but i wanted to uhh change all this stupid shit
            );
            c.Next.Operand = Main.WilloDamage.Value;
            c.Index += 6;
            c.Next.Operand = Main.WilloDamageStack.Value;

        }
        public static void ChangeRange(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(12f),
                x => x.MatchLdcR4(2.4f)
            );
            c.Next.Operand = Main.WilloRange.Value;
            c.Index += 1;
            c.Next.Operand = Main.WilloRangeStack.Value;
        }
        public static void ChangeProc()
        {
            var w = Resources.Load<GameObject>("prefabs/networkedobjects/WilloWispDelay").GetComponent<RoR2.DelayBlast>();
            w.procCoefficient = Main.WilloProcCo.Value;
        }
    }
}
