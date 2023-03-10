using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Equipment.Orange
{
    public class OcularHud : EquipmentBase
    {
        public override string Name => "::: Equipment :: Ocular HUD";
        public override string InternalPickupToken => "critOnUse";

        public override string PickupText => "Gain " + critChanceGain + "% Critical Strike Chance" +
                                             (critDamageGain > 0f ? " and " + d(critDamageGain) + "% Critical Strike Damage" : "") +
                                             " for " + buffDuration + " seconds.";

        public override string DescText => "Gain <style=cIsDamage>+" + critChanceGain + "% Critical Strike Chance</style>" +
                                           (critDamageGain > 0f ? " and <style=cIsDamage>+" + critDamageGain + "% Critical Strike Damage</style>" : "") +
                                           " for " + buffDuration + " seconds.";

        [ConfigField("Cooldown", "", 50f)]
        public static float cooldown;

        [ConfigField("Crit Chance Gain", "", 100f)]
        public static float critChanceGain;

        [ConfigField("Crit Damage Gain", "Decimal.", 0f)]
        public static float critDamageGain;

        [ConfigField("Buff Duration", "", 8f)]
        public static float buffDuration;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireCritOnUse += ChangeDuration;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeChance;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;

            var HUD = Utils.Paths.EquipmentDef.CritOnUse.Load<EquipmentDef>();
            HUD.cooldown = cooldown;
        }

        private void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(RoR2Content.Buffs.FullCrit))
            {
                args.critDamageMultAdd += critDamageGain;
            }
        }

        private void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchBrfalse(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(100f)))
            {
                c.Index += 3;
                c.EmitDelegate<Func<float, float>>((useless) =>
                {
                    return critChanceGain;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Ocular HUD Crit Chance hook");
            }
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(8f)))
            {
                c.Next.Operand = buffDuration;
            }
            else
            {
                Logger.LogError("Failed to apply Ocular HUD Duration hook");
            }
        }
    }
}