using RoR2.Projectile;
using UnityEngine;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    public class Fireworks : ItemBase
    {
        public static int count;
        public static int countstack;
        public static float damage;
        public static float procco;

        public override string Name => ":: Items : Whites :: Bundle of Fireworks";
        public override string InternalPickupToken => "firework";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public static float actualCount = count + countstack;

        public override string DescText => "Activating an interactable <style=cIsDamage>launches " + actualCount + " <style=cStack>(+" + countstack + " per stack)</style> fireworks</style> that deal <style=cIsDamage>" + damage + "</style> base damage.";
        public override void Init()
        {
            count = ConfigOption(8, "Base Count", "Vanilla is 8");
            countstack = ConfigOption(4, "Stack Count", "Per Stack. Vanilla is 4");
            damage = ConfigOption(3f, "Damage Coefficient", "Decimal. Vanilla is 3");
            procco = ConfigOption(0.2f, "Proc Coefficient", "Vanilla is 0.2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnInteractionBegin += ChangeCount;
            Changes();
        }
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
                return count - countstack + ((val - 4) / 4) * countstack;
            });

        }

        public static void Changes()
        {
            var croppa = Resources.Load<GameObject>("prefabs/projectiles/FireworkProjectile");
            var msm = croppa.GetComponent<ProjectileImpactExplosion>();
            var skm = croppa.GetComponent<ProjectileController>();
            msm.blastDamageCoefficient = damage / 3f;
            skm.procCoefficient = procco;
            // this is probably the wrong way of doing this but i cant figure out another
        }
    }
}
