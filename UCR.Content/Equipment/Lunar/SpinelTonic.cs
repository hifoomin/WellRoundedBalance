using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using MonoMod.Cil;

namespace UltimateCustomRun.Equipment
{
    public class SpinelTonic : EquipmentBase
    {
        public override string Name => "::: Equipment ::: Spinel Tonic";
        public override string InternalPickupToken => "tonic";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Drink the Tonic, gaining a boost for " + Duration + " seconds. Increases <style=cIsDamage>damage</style> by <style=cIsDamage>+" + d(DamageBuff - 1) + "</style>. Increases <style=cIsDamage>attack speed</style> by <style=cIsDamage>+" + d(AttackSpeedMult - 1) + "</style>. Increases <style=cIsDamage>armor</style> by <style=cIsDamage>+" + ArmorBuff + "</style>. Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>+" + d(MaxHpBuff) + "</style>. Increases <style=cIsHealing>passive health regeneration</style> by <style=cIsHealing>+" + d(RegenBuff) + "</style>. Increases <style=cIsUtility>movespeed</style> by <style=cIsUtility>+" + d(MoveSpeedBuff) + "</style>.\n\nWhen the Tonic wears off, you have a <style=cIsHealth>" + AfflictionChance + "</style> chance to gain a <style=cIsHealth>Tonic Affliction, reducing all of your stats</style> by <style=cIsHealth>-" + d(AllStatsDebuff) + "</style> <style=cStack>(-" + d(AllStatsDebuff) + " per stack)</style>.";

        public static float Duration;
        public static float DamageBuff;
        public static float AttackSpeedMult;
        public static float ArmorBuff;
        public static float MaxHpBuff;
        public static float RegenBuff;
        public static float MoveSpeedBuff;
        public static float AfflictionChance;
        public static float CurseGain;
        public static float AllStatsDebuff;

        public override void Init()
        {
            Duration = ConfigOption(20f, "Duration", "Vanilla is 20");
            DamageBuff = ConfigOption(2f, "Damage", "Decimal. Vanilla is 2");
            AttackSpeedMult = ConfigOption(1.7f, "Attack Speed Multiplier", "Decimal. Vanilla is 1.7");
            ArmorBuff = ConfigOption(20f, "Armor", "Vanilla is 20");
            MaxHpBuff = ConfigOption(0.5f, "Percent Health", "Decimal. Vanilla is 0.5");
            RegenBuff = ConfigOption(3f, "Regen", "Decimal. Vanilla is 3");
            MoveSpeedBuff = ConfigOption(0.3f, "Movement Speed", "Decimal. Vanilla is 0.3");
            AfflictionChance = ConfigOption(20f, "Tonic Affliction Chance", "Vanilla is 20");
            CurseGain = ConfigOption(0.1f, "Affliction Curse", "Decimal. Vanilla is 0.1");
            AllStatsDebuff = ConfigOption(0.05f, "Affliction All Stats", "Decimal. Vanilla is 0.05");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
            IL.RoR2.EquipmentSlot.FireTonic += ChangeAff;
            On.RoR2.EquipmentSlot.Start += ChangeDur;
        }

        private void ChangeDur(On.RoR2.EquipmentSlot.orig_Start orig, EquipmentSlot self)
        {
            EquipmentSlot.tonicBuffDuration = Duration;
            orig(self);
        }

        private void ChangeAff(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(80f)))
            {
                c.Next.Operand = 100f - AfflictionChance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Spinel Tonic Affliction Chance hook");
            }
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_maxHealth"),
                x => x.MatchLdcR4(1.5f)))
            {
                c.Index += 1;
                c.Next.Operand = 1f + MaxHpBuff;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Spinel Tonic Health hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_maxShield"),
                x => x.MatchLdcR4(1.5f)))
            {
                c.Index += 1;
                c.Next.Operand = 1f + MaxHpBuff;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Spinel Tonic Shield hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_attackSpeed"),
                x => x.MatchLdcR4(1.7f)))
            {
                c.Index += 1;
                c.Next.Operand = AttackSpeedMult;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Spinel Tonic Attack Speed hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_moveSpeed"),
                x => x.MatchLdcR4(1.3f)))
            {
                c.Index += 1;
                c.Next.Operand = 1f + MoveSpeedBuff;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Spinel Tonic Movement Speed hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_armor"),
                x => x.MatchLdcR4(20f),
                x => x.MatchAdd()))
            {
                c.Index += 1;
                c.Next.Operand = ArmorBuff;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Spinel Tonic Armor hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_damage"),
                x => x.MatchLdcR4(2f)))
            {
                c.Index += 1;
                c.Next.Operand = DamageBuff;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Spinel Tonic Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_regen"),
                x => x.MatchLdcR4(4f)))
            {
                c.Index += 1;
                c.Next.Operand = 1f + RegenBuff;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Spinel Tonic Regen hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.95f)))

            {
                c.Next.Operand = 1f - AllStatsDebuff;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Tonic Affliction All Stats hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_cursePenalty"),
                x => x.MatchLdcR4(0.1f)))

            {
                c.Index += 1;
                c.Next.Operand = CurseGain;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Tonic Affliction Curse hook");
            }
        }
    }
}