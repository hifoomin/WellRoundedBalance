using RoR2.Projectile;
using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class AtGMissileMk1
    {
        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(10f)
            );
            c.Next.Operand = Main.AtGChance.Value;
        }
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f)
            );
            c.Next.Operand = Main.AtGDamage.Value;
        }
        public static void ChangeProc()
        {
            var mp = Resources.Load<GameObject>("prefabs/projectiles/MissileProjectile").GetComponent<ProjectileController>().procCoefficient;
            mp = Main.AtGProcCo.Value;
        }
    }
}
