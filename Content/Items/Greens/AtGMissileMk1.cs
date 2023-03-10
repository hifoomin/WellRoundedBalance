using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class AtGMissileMk1 : ItemBase
    {
        public override string Name => ":: Items :: Greens :: AtG Missile Mk1";
        public override string InternalPickupToken => "missile";

        public override string PickupText => "Chance to fire a missile.";
        public override string DescText => "<style=cIsDamage>10%</style> chance to fire a missile that deals <style=cIsDamage>" + d(baseTotalDamage) + "</style> <style=cStack>(+" + d(totalDamagePerStack) + " per stack)</style> TOTAL damage.";

        [ConfigField("Base TOTAL Damage", "Decimal.", 3f)]
        public static float baseTotalDamage;

        [ConfigField("TOTAL Damage Per Stack", "Decimal.", 2f)]
        public static float totalDamagePerStack;

        [ConfigField("Improve targeting?", "Affects all missile items and equipment.", true)]
        public static bool improveTargeting;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            Changes();
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchBrfalse(out _),
                x => x.MatchLdcR4(3f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = totalDamagePerStack;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((self) =>
                {
                    return self + (baseTotalDamage - totalDamagePerStack);
                });
            }
            else
            {
                Logger.LogError("Failed to apply AtG Damage hook");
            }
        }

        public static void Changes()
        {
            var missileProjectile = Utils.Paths.GameObject.MissileProjectile.Load<GameObject>();
            var missileProjectileController = missileProjectile.GetComponent<ProjectileController>();
            missileProjectileController.procCoefficient = 0f;

            if (improveTargeting)
            {
                var missileController = missileProjectile.GetComponent<MissileController>();
                missileController.maxSeekDistance = 10000f;
                missileController.turbulence = 0f;
                missileController.deathTimer = 30f;
                missileController.giveupTimer = 30f;
                missileController.delayTimer = 0f;
            }
        }
    }
}