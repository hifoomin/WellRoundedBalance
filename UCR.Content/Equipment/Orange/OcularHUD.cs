using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using System;

namespace UltimateCustomRun.Equipment
{
    public class OcularHud : EquipmentBase
    {
        public override string Name => "::: Equipment :: Ocular HUD";
        public override string InternalPickupToken => "critOnUse";

        public override bool NewPickup => true;

        public override bool NewDesc => true;

        public override string PickupText => "Gain " + CritChance + "% Critical Strike Chance" +
                                             (CritDamage > 0f ? " and " + CritDamage + "% Critical Strike Damage" : "") +
                                             " for " + Duration + " seconds.";

        public override string DescText => "Gain <style=cIsDamage>+" + CritChance + "% Critical Strike Chance</style>" +
                                           (CritDamage > 0f ? " and <style=cIsDamage>+" + CritDamage + "% Critical Strike Damage</style>" : "") +
                                           " for " + Duration + " seconds.";

        public static float Duration;
        public static float CritChance;
        public static float CritDamage;

        public override void Init()
        {
            Duration = ConfigOption(8f, "Duration", "Vanilla is 8");
            CritChance = ConfigOption(100f, "Crit Chance", "Vanilla is 100");
            CritDamage = ConfigOption(0f, "Crit Damage", "Vanilla is 0");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireCritOnUse += ChangeDuration;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeChance;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        private void AddBehavior(RoR2.CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(RoR2.RoR2Content.Buffs.FullCrit))
            {
                args.critDamageMultAdd += CritDamage;
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
                    return CritChance;
                });
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Ocular HUD Crit Chance hook");
            }
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(8f)))
            {
                c.Next.Operand = Duration;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Ocular HUD Duration hook");
            }
        }
    }
}