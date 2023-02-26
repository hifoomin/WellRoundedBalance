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

        public override string DescText => StackDesc(fireworks, fireworksStack,
            init => $"Activating an interactable <style=cIsDamage>launches {s(fireworks, "{Stack} firework")} that deal <style=cIsDamage>{d(blastDamageCoefficient)}</style> base damage.</style>",
            stack => stack.ToString());

        [ConfigField("Fireworks", "", 8)]
        public static int fireworks;

        [ConfigField("Fireworks Per Stack", "", 8)]
        public static int fireworksStack;

        [ConfigField("Improve targeting?", "", true)]
        public static bool improveTargeting;

        [ConfigField("Blast Radius", "", 6f)]
        public static float blastRadius;

        [ConfigField("Blast Damage Coefficient", "", 3f)]
        public static float blastDamageCoefficient;

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
            if (c.TryGotoNext(x => x.MatchStfld<FireworkLauncher>(nameof(FireworkLauncher.remaining))))
            {
                c.EmitDelegate<Func<int, int>>((val) =>
                {
                    return fireworks - fireworksStack + ((val - fireworksStack) / fireworksStack) * fireworksStack;
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
            var firework = Utils.Paths.GameObject.FireworkProjectile.Load<GameObject>();
            var projectileImpactExplosion = firework.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.blastRadius = blastRadius; // vanilla 5f
            projectileImpactExplosion.blastDamageCoefficient = blastDamageCoefficient;

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