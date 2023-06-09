using JetBrains.Annotations;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class BundleOfFireworks : ItemBase<BundleOfFireworks>
    {
        public override string Name => ":: Items : Whites :: Bundle of Fireworks";
        public override ItemDef InternalPickup => RoR2Content.Items.Firework;

        public override string PickupText => "Activating an interactable launches fireworks at nearby enemies.";

        public override string DescText =>
            StackDesc(fireworks, fireworksStack, init => $"Activating an interactable <style=cIsDamage>launches {s(init, "{Stack} firework")}</style> that deal <style=cIsDamage>{d(blastDamageCoefficient)}</style> base damage.");

        [ConfigField("Fireworks", 8f)]
        public static float fireworks;

        [ConfigField("Fireworks Per Stack", 8f)]
        public static float fireworksStack;

        [ConfigField("Fireworks is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float fireworksIsHyperbolic;

        [ConfigField("Improve targeting?", true)]
        public static bool improveTargeting;

        [ConfigField("Blast Radius", 6f)]
        public static float blastRadius;

        [ConfigField("Blast Damage Coefficient", 3f)]
        public static float blastDamageCoefficient;

        [ConfigField("Blast Proc Coefficient", 0.25f)]
        public static float blastProcCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnInteractionBegin += GlobalEventManager_OnInteractionBegin;
            Changes();
        }

        public static void GlobalEventManager_OnInteractionBegin(ILContext il)
        {
            ILCursor c = new(il);
            int idx = GetItemLoc(c, nameof(RoR2Content.Items.Firework));
            if (idx != -1 && c.TryGotoNext(x => x.MatchStfld<FireworkLauncher>(nameof(FireworkLauncher.remaining))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, idx);
                c.EmitDelegate<Func<int, int>>(stack => (int)StackAmount(fireworks, fireworksStack, stack, fireworksIsHyperbolic));
            }
            else Logger.LogError("Failed to apply Bundle Of Fireworks Count hook");
        }

        public static void Changes()
        {
            var firework = Utils.Paths.GameObject.FireworkProjectile.Load<GameObject>();
            var projectileImpactExplosion = firework.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.blastRadius = blastRadius; // vanilla 5f
            projectileImpactExplosion.blastDamageCoefficient = blastDamageCoefficient / 3f;
            projectileImpactExplosion.blastProcCoefficient = blastProcCoefficient * globalProc;

            var projectileController = firework.GetComponent<ProjectileController>();
            var ghostPrefab = projectileController.ghostPrefab;
            ghostPrefab.transform.localScale = new Vector3(blastRadius / 4f, blastRadius / 4f, blastRadius / 4f);

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