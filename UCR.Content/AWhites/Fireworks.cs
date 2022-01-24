using RoR2.Projectile;
using UnityEngine;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    public static class Fireworks
    {
        public static void ChangeCount(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            //c.GotoNext(MoveType.Before,
            //x => x.MatchLdcI4(4),
            //x => x.MatchLdloc(out _),
            //x => x.MatchLdcI4(4)
            c.GotoNext(x => x.MatchStfld<RoR2.FireworkLauncher>("remaining")

            );
            //c.Next.Operand = FireworksCount.Value;
            //c.Index += 2;
            //c.Next.Operand = FireworksCountStack.Value;
            c.EmitDelegate<Func<int, int>>((val) =>
            {
                return Main.FireworksCount.Value + ((val - 4) / 4) * Main.FireworksCountStack.Value;
            });

        }

        public static void Changes()
        {
            var furrywork = Resources.Load<GameObject>("prefabs/projectiles/FireworkProjectile");
            var croppa = furrywork.GetComponent<ProjectileImpactExplosion>();
            var croppa2 = furrywork.GetComponent<ProjectileController>();
            croppa.blastDamageCoefficient = Main.FireworksDamage.Value / 3f;
            croppa2.procCoefficient = Main.FireworksProcCo.Value;
            // this is probably the wrong way of doing this but i cant figure out another
        }
    }
}
