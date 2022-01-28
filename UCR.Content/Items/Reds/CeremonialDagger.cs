using RoR2;
using RoR2.Projectile;
using UnityEngine;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    public class CeremonialDagger
    {
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchBle(out ILLabel IL_067A),
                x => x.MatchLdcR4(1.5f)
            );
            c.Index += 1;
            c.Next.Operand = Main.CeremonialDamage.Value;
        }
        public static void ChangeCount(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(RoR2.Util).GetMethod("CheckRoll", new Type[] { typeof(float), typeof(CharacterMaster) })),
                x => x.MatchLdcI4(3)
            );
            c.Index += 1;
            c.Next.Operand = Main.CeremonialCount.Value;
            // PLEASE HELP TO FIX
        }
        public static void ChangeProc()
        {
            var c = Resources.Load<GameObject>("prefabs/projectiles/daggerprojectile").GetComponent<ProjectileController>();
            c.procCoefficient = Main.CeremonialProcCo.Value;
        }
    }
}
