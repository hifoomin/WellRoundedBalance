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
    public static class Shared
    {
        public static void ChangeDangerDelay(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<CharacterBody>("outOfDangerStopwatch"),
                x => x.MatchLdcR4(7f)
            );
            c.Index += 1;
            c.Next.Operand = Main.SharedDangerDelay.Value;
        }
        public static void ChangeCombatDelay(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
               x => x.MatchLdfld<CharacterBody>("outOfCombatStopwatch"),
               x => x.MatchLdcR4(5f)
            );
            c.Index += 1;
            c.Next.Operand = Main.SharedCombatDelay.Value;
        }
    }
}
