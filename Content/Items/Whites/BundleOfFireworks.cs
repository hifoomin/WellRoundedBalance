using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace WellRoundedBalance.Items.Whites
{
    public class BundleOfFireworks : ItemBase
    {
        public override string Name => ":: Items : Whites :: Bundle of Fireworks";
        public override string InternalPickupToken => "firework";

        public override string PickupText => "Activating an interactable launches fireworks at nearby enemies.";

        public override string DescText => "Activating an interactable <style=cIsDamage>launches 8 <style=cStack>(+8 per stack)</style> fireworks</style> that deal <style=cIsDamage>300%</style> base damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnInteractionBegin += ChangeCount;
            Changes();
        }

        public static void ChangeCount(ILContext il)
        {
            ILCursor c = new(il);
            //c.GotoNext(MoveType.Before,
            //x => x.MatchLdcI4(4),
            //x => x.MatchLdloc(out _),
            //x => x.MatchLdcI4(4)
            if (c.TryGotoNext(x => x.MatchStfld<FireworkLauncher>("remaining")))
            {
                c.EmitDelegate<Func<int, int>>((val) =>
                {
                    return 8 - 8 + ((val - 4) / 4) * 8;
                    // 8 - 4 + ((val - 4) / 4) * 4;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Bundle Of Fireworks Count hook");
            }
        }

        public static void Changes()
        {
            var croppa = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/FireworkProjectile");
            var msm = croppa.GetComponent<ProjectileImpactExplosion>();
            msm.blastRadius = 6f; // vanilla 5f
        }
    }
}