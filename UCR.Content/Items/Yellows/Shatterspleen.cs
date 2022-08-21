using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Yellows
{
    public class Shatterspleen : ItemBase
    {
        public static float Crit;
        public static bool StackCrit;
        public static float BaseExplosionDamage;
        public static float PercentExplosionDamage;
        public static float AoE;

        public override string Name => ":: Items :::: Yellows :: Shatterspleen";
        public override string InternalPickupToken => "bleedOnHitAndExplode";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "<style=cIsDamage>Critical Strikes bleed</style> enemies for <style=cIsDamage>240%</style> base damage." +
            (BaseExplosionDamage > 0f || PercentExplosionDamage > 0f ? "<style=cIsDamage>Bleeding</style> enemies <style=cIsDamage>explode</style> on death for " +
            (BaseExplosionDamage > 0f ? "<style=cIsDamage>" + d(BaseExplosionDamage) + "</style> <style=cStack>(+" + d(BaseExplosionDamage) + " per stack)</style> damage, plus an additional" : "") +
            (PercentExplosionDamage > 0f ? "<style=cIsDamage>" + d(PercentExplosionDamage) + "</style> <style=cStack>(+" + d(PercentExplosionDamage) + " per stack)</style> of their maximum health." : "") : "");

        public override void Init()
        {
            Crit = ConfigOption(5f, "Crit Chance", "Vanilla is 5");
            StackCrit = ConfigOption(false, "Stack Crit Chance?", "Vanilla is false");
            BaseExplosionDamage = ConfigOption(4f, "Base Explosion Damage", "Decimal. Per Stack. Vanilla is 4");
            PercentExplosionDamage = ConfigOption(0.15f, "Percent Explosion Damage", "Decimal. Per Stack. Vanilla is 0.15");
            AoE = ConfigOption(16f, "Area of Effect", "Vanilla is 16");
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(4f),
                    x => x.MatchLdcI4(1),
                    x => x.MatchLdloc(out _)))
            {
                c.Next.Operand = BaseExplosionDamage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Shatterspleen Base Explosion Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.15f),
                    x => x.MatchLdcI4(1),
                    x => x.MatchLdloc(out _)))
            {
                c.Next.Operand = PercentExplosionDamage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Shatterspleen Percent Explosion Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.0f),
                    x => x.MatchStfld("RoR2.DelayBlast", "baseForce"),
                    x => x.MatchDup(),
                    x => x.MatchLdcR4(16f),
                    x => x.MatchStfld("RoR2.DelayBlast", "radius")))
            {
                c.Index += 3;
                c.Next.Operand = AoE;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Shatterspleen Radius hook");
            }
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.BleedOnHitAndExplode);
                if (stack > 0)
                {
                    args.critAdd += StackCrit ? Crit * stack : Crit;
                }
            }
        }
    }
}