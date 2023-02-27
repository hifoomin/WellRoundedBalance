using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class BundleOfFireworks : ItemBase
    {
        public override string Name => ":: Items : Whites :: Bundle of Fireworks";
        public override string InternalPickupToken => "firework";

        public override string PickupText => "Activating an interactable launches fireworks at nearby enemies.";

        public override string DescText => "Activating an interactable <style=cIsDamage>launches 8 <style=cStack>(+" + fireworksPerStack + " per stack)</style> fireworks</style> that deal <style=cIsDamage>300%</style> base damage.";

        [ConfigField("Fireworks Per Stack", "", 8)]
        public static int fireworksPerStack;

        [ConfigField("Improve targeting?", "", true)]
        public static bool improveTargeting;

        [ConfigField("Blast Radius", "", 6f)]
        public static float blastRadius;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnInteractionBegin += GlobalEventManager_OnInteractionBegin;
            Changes();
        }

        private void GlobalEventManager_OnInteractionBegin(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchStfld<FireworkLauncher>("remaining")))
            {
                c.EmitDelegate<Func<int, int>>((val) =>
                {
                    return 8 - fireworksPerStack + ((val - fireworksPerStack) / fireworksPerStack) * fireworksPerStack;
                    // 8 - 4 + ((val - 4) / 4) * 4;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Bundle Of Fireworks Count hook");
            }
        }

        private void Changes()
        {
            var firework = Utils.Paths.GameObject.FireworkProjectile.Load<GameObject>();
            var projectileImpactExplosion = firework.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.blastRadius = blastRadius; // vanilla 5f

            if (improveTargeting)
            {
                var fireworkController = firework.GetComponent<MissileController>();
                fireworkController.acceleration = 3f;
                fireworkController.giveupTimer = 30f;
                fireworkController.deathTimer = 30f;
                fireworkController.turbulence = 0f;
                fireworkController.maxSeekDistance = 10000f;
            }
        }
    }
}