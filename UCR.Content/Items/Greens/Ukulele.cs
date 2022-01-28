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
    public static class Ukulele
    {
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(RoR2.Util).GetMethod("CheckRoll", new Type[] { typeof(float), typeof(CharacterMaster) } )),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdcR4(0.8f)
            );
            c.Index += 2;
            c.Next.Operand = Main.UkuleleDamage.Value;
            // oh wow util.checkroll is stupid why tf are there two methods named the same
            // thank you harb :)
        }
        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "ChainLightning"),
                x => x.MatchCallOrCallvirt<RoR2.Inventory>("GetItemCount"),
                x => x.MatchStloc(out _),
                x => x.MatchLdcR4(25f)
            );
            c.Index += 3;
            c.Next.Operand = Main.UkuleleChance.Value;
        }
        public static void ChangeTargetCountStack(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStfld<RoR2.Orbs.LightningOrb>("isCrit"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(2)
            );
            c.Index += 2;
            c.Next.Operand = Main.UkuleleTargetsStack.Value;
        }

        public static void ChangeTargetCountBase()
        {
            On.RoR2.Orbs.LightningOrb.Begin += (orig, self) =>
            {
                orig(self);
                if (self.lightningType is LightningOrb.LightningType.Ukulele)
                {
                    self.bouncesRemaining = Main.UkuleleTargets.Value;
                }
            };
        }

        public static void ChangeRangeBase()
        {
            On.RoR2.Orbs.LightningOrb.Begin += (orig, self) =>
            {
                orig(self);
                if (self.lightningType is LightningOrb.LightningType.Ukulele)
                {
                    self.range = Main.UkuleleRange.Value;
                    // self.canBounceOnSameTarget = Main.UkuleleCanBounceOnSameTarget.Value;
                    // im scared of the warning :IL:
                }
                
            };
        }

        public static void ChangeRangeStack(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<RoR2.Orbs.LightningOrb>("range"),
                x => x.MatchLdcI4(2)
            );
            c.Index += 1;
            c.Next.Operand = Main.UkuleleRangeStack.Value;
        }

        public static void ChangeProcCo(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdflda<RoR2.Orbs.LightningOrb>("procChainMask"),
                x => x.MatchLdcI4(3),
                x => x.MatchCallOrCallvirt<RoR2.ProcChainMask>("AddProc"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.2f)

            );
            c.Index += 4;
            c.Next.Operand = Main.UkuleleProcCo.Value;
        }
    }
}
