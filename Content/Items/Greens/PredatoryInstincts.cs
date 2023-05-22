using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class PredatoryInstincts : ItemBase<PredatoryInstincts>
    {
        public override string Name => ":: Items :: Greens :: Predatory Instincts";
        public override ItemDef InternalPickup => RoR2Content.Items.AttackSpeedOnCrit;

        public override string PickupText => "'Critical Strikes' increase attack speed up to " + baseBuffCap + " times.";

        public override string DescText => (critChance > 0 || critChancePerStack > 0 ? "Gain <style=cIsDamage>" + critChance + "% " + (critChancePerStack > 0 ? " <style=cStack>(+" + critChancePerStack + "% per stack)</style>" : "") + "critical chance</style>. " : "") + "<style=cIsDamage>Critical strikes</style> increase <style=cIsDamage>attack speed</style> by <style=cIsDamage>" + d(attackSpeedGainPerBuff) + "</style> up to <style=cIsDamage>" + baseBuffCap + "</style> <style=cStack>(+" + buffCapPerStack + " per stack)</style> times.";

        [ConfigField("Attack Speed Gain Per Buff", 0.12f)]
        public static float attackSpeedGainPerBuff;

        [ConfigField("Base Buff Cap", 3)]
        public static int baseBuffCap;

        [ConfigField("Buff Cap Per Stack", 3)]
        public static int buffCapPerStack;

        [ConfigField("Crit Chance", 5f)]
        public static float critChance;

        [ConfigField("Crit Chance Per Stack", 0f)]
        public static float critChancePerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            IL.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += CharacterBody_AddTimedBuff_BuffDef_float;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.AttackSpeedOnCrit);
                if (stack > 0)
                {
                    args.critAdd += StackAmount(critChance, critChancePerStack, stack) - 5f;
                }
            }
        }

        private void CharacterBody_AddTimedBuff_BuffDef_float(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchBge(out _),
                x => x.MatchLdarg(0)))
            {
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<int, CharacterBody, int>>((orig, body) =>
                {
                    var inventory = body.inventory;
                    if (inventory)
                    {
                        var stack = inventory.GetItemCount(RoR2Content.Items.AttackSpeedOnCrit);
                        if (stack > 0)
                        {
                            return baseBuffCap - buffCapPerStack + stack * buffCapPerStack;
                        }
                    }
                    return orig;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Predatory Instincts Buff Count hook");
            }
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                 x => x.MatchLdcR4(0.12f)))
            {
                c.Next.Operand = attackSpeedGainPerBuff;
            }
            else
            {
                Logger.LogError("Failed to apply Predatory Instincts Attack Speed hook");
            }
        }
    }
}