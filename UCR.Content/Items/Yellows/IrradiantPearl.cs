using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Yellows
{
    public class IrradiantPearl : ItemBase
    {
        public static float PercentHealth;
        public static float FlatRegen;
        public static float PercentRegen;
        public static float PercentArmor;
        public static float PercentSpeed;
        public static float PercentDamage;
        public static float PercentAttackSpeed;
        public static float PercentCrit;
        public static float PercentBaseArmor;

        public override string Name => ":: Items :::: Yellows :: Irradiant Pearl";
        public override string InternalPickupToken => "shinyPearl";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Increases <style=cIsUtility>ALL stats</style> by <style=cIsUtility>" + Mathf.Round((PercentHealth * 100f + PercentRegen * 100f + PercentArmor * 100f + PercentSpeed * 100f + PercentDamage * 100f + PercentAttackSpeed * 100f + PercentCrit) / 7f) + "%</style> <style=cStack>(+" + Mathf.Round((PercentHealth * 100f + PercentRegen * 100f + PercentArmor * 100f + PercentSpeed * 100f + PercentDamage * 100f + PercentAttackSpeed * 100f + PercentCrit) / 7f) + " per stack)</style>.";

        public override void Init()
        {
            PercentHealth = ConfigOption(0.1f, "Percent Health", "Decimal. Per Stack. Vanilla is 0.1. Also affects Pearl.");
            FlatRegen = ConfigOption(0f, "Flat Regen", "Per Stack. Vanilla is 0.1");
            PercentRegen = ConfigOption(0.1f, "Percent Regen", "Decimal. Per Stack. Vanilla is 0");
            PercentArmor = ConfigOption(0.1f, "Percent Armor", "Decimal. Per Stack. Vanilla is 0");
            PercentSpeed = ConfigOption(0.1f, "Percent Speed", "Decimal. Per Stack. Vanilla is 0.1");
            PercentDamage = ConfigOption(0.1f, "Percent Damage", "Decimal. Per Stack. Vanilla is 0.1");
            PercentAttackSpeed = ConfigOption(0.1f, "Percent Attack Speed", "Decimal. Per Stack. Vanilla is 0.1");
            PercentCrit = ConfigOption(10f, "Percent Crit", "Per Stack. Vanilla is 10");
            PercentArmor = ConfigOption(0f, "Percent Base Armor", "Decimal. Per Stack. Vanilla is 0.1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        private void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.ShinyPearl);
                if (stack > 0)
                {
                    args.regenMultAdd += PercentRegen * stack;
                    args.armorAdd += sender.armor * PercentArmor * stack;
                }
            }
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(30),
                    x => x.MatchAdd(),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.1f),
                    x => x.MatchMul(),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(63)))
            {
                c.Index += 3;
                c.Next.Operand = PercentHealth;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Irradiant Pearl Percent Health hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(30),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.1f),
                    x => x.MatchMul(),
                    x => x.MatchLdloc(66)))
            {
                c.Index += 2;
                c.Next.Operand = FlatRegen;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Irradiant Pearl Flat Regen hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(30),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.1f),
                    x => x.MatchMul(),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(75)))
            {
                c.Index += 2;
                c.Next.Operand = PercentSpeed;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Irradiant Pearl Percent Speed hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(30),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.1f),
                    x => x.MatchMul(),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(79)))
            {
                c.Index += 2;
                c.Next.Operand = PercentDamage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Irradiant Pearl Percent Damage hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(30),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.1f),
                    x => x.MatchMul(),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(83)))
            {
                c.Index += 2;
                c.Next.Operand = PercentAttackSpeed;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Irradiant Pearl Percent Attack Speed hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(30),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(10f),
                    x => x.MatchMul(),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(84)))
            {
                c.Index += 2;
                c.Next.Operand = PercentCrit;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Irradiant Pearl Percent Crit hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdcR4(0.1f),
                    x => x.MatchLdloc(30)))
            {
                c.Index += 1;
                c.Next.Operand = PercentBaseArmor;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Irradiant Pearl Percent Base Armor hook");
            }
        }
    }
}