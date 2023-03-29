using MonoMod.Cil;
using UnityEngine.Rendering.PostProcessing;

namespace WellRoundedBalance.Equipment.Lunar
{
    public class SpinelTonic : EquipmentBase
    {
        public override string Name => ":: Equipment ::: Spinel Tonic";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.Tonic;

        public override string PickupText => "Gain a massive boost to ALL stats. <color=#FF7F7F>Chance to gain an affliction that reduces ALL stats.</color>";

        public override string DescText => "Drink the Tonic, gaining a boost for " + buffDuration + " seconds. Increases <style=cIsDamage>damage</style> by <style=cIsDamage>+" + d(damageIncrease) + "</style>. Increase <style=cIsDamage>attack speed</style> by <style=cIsDamage>+" + d(attackSpeedMultiplier - 1) + "</style>. Increases <style=cIsDamage>armor</style> by <style=cIsDamage>+" + armorGain + "</style>. Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>+" + d(percentMaximumHealthIncrease) + "</style>. Increases <style=cIsHealing>passive health regeneration</style> by <style=cIsHealing>+" + d(regenMultiplier) + "</style>. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>+" + d(movementSpeedIncrease) + "</style>.\n\nWhen the Tonic wears off, you have a <style=cIsHealth>" + afflictionChance + "</style> chance to gain a <style=cIsHealth>Tonic Affliction, reducing all of your stats</style> by <style=cIsHealth>-" + d(afflictionAllStatsDecrease) + "</style> <style=cStack>(-" + d(afflictionAllStatsDecrease) + " per stack)</style>.";

        [ConfigField("Cooldown", "", 65f)]
        public static float cooldown;

        [ConfigField("Buff Duration", "", 20f)]
        public static float buffDuration;

        [ConfigField("Damage Increase", "Decimal.", 0.6f)]
        public static float damageIncrease;

        [ConfigField("Attack Speed Multiplier", "", 1.5f)]
        public static float attackSpeedMultiplier;

        [ConfigField("Armor Gain", "", 20f)]
        public static float armorGain;

        [ConfigField("Percent Maximum Health Increase", "", 0.5f)]
        public static float percentMaximumHealthIncrease;

        [ConfigField("Regen Multiplier", "", 3f)]
        public static float regenMultiplier;

        [ConfigField("Movement Speed Increase", "Decimal.", 0.3f)]
        public static float movementSpeedIncrease;

        [ConfigField("Affliction Chance", "", 20f)]
        public static float afflictionChance;

        [ConfigField("Affliction Curse Gain", "Decimal.", 0.1f)]
        public static float afflictionCurseGain;

        [ConfigField("Affliction All Stats Decrease", "Decimal.", 0.05f)]
        public static float afflictionAllStatsDecrease;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
            IL.RoR2.EquipmentSlot.FireTonic += ChangeAff;
            On.RoR2.EquipmentSlot.Start += ChangeDur;
            Changess();
        }

        private void ChangeDur(On.RoR2.EquipmentSlot.orig_Start orig, EquipmentSlot self)
        {
            EquipmentSlot.tonicBuffDuration = buffDuration;
            orig(self);
        }

        private void ChangeAff(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(80f)))
            {
                c.Next.Operand = 100f - afflictionChance;
            }
            else
            {
                Logger.LogError("Failed to apply Spinel Tonic Affliction Chance hook");
            }
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_maxHealth"),
                x => x.MatchLdcR4(1.5f)))
            {
                c.Index += 1;
                c.Next.Operand = 1f + percentMaximumHealthIncrease;
            }
            else
            {
                Logger.LogError("Failed to apply Spinel Tonic Health hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_maxShield"),
                x => x.MatchLdcR4(1.5f)))
            {
                c.Index += 1;
                c.Next.Operand = 1f + percentMaximumHealthIncrease;
            }
            else
            {
                Logger.LogError("Failed to apply Spinel Tonic Shield hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_attackSpeed"),
                x => x.MatchLdcR4(1.7f)))
            {
                c.Index += 1;
                c.Next.Operand = attackSpeedMultiplier;
            }
            else
            {
                Logger.LogError("Failed to apply Spinel Tonic Attack Speed hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_moveSpeed"),
                x => x.MatchLdcR4(1.3f)))
            {
                c.Index += 1;
                c.Next.Operand = 1f + movementSpeedIncrease;
            }
            else
            {
                Logger.LogError("Failed to apply Spinel Tonic Movement Speed hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_armor"),
                x => x.MatchLdcR4(20f),
                x => x.MatchAdd()))
            {
                c.Index += 1;
                c.Next.Operand = armorGain;
            }
            else
            {
                Logger.LogError("Failed to apply Spinel Tonic Armor hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_damage"),
                x => x.MatchLdcR4(2f)))
            {
                c.Index += 1;
                c.Next.Operand = 1f + damageIncrease;
            }
            else
            {
                Logger.LogError("Failed to apply Spinel Tonic Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_regen"),
                x => x.MatchLdcR4(4f)))
            {
                c.Index += 1;
                c.Next.Operand = 1f + regenMultiplier;
            }
            else
            {
                Logger.LogError("Failed to apply Spinel Tonic Regen hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.95f)))

            {
                c.Next.Operand = 1f - afflictionAllStatsDecrease;
            }
            else
            {
                Logger.LogError("Failed to apply Tonic Affliction All Stats hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_cursePenalty"),
                x => x.MatchLdcR4(0.1f)))

            {
                c.Index += 1;
                c.Next.Operand = afflictionCurseGain;
            }
            else
            {
                Logger.LogError("Failed to apply Tonic Affliction Curse hook");
            }
        }

        private void Changess()
        {
            var pp = Utils.Paths.GameObject.TonicBuffEffect.Load<GameObject>().transform.GetChild(1).GetChild(0);
            var postProcessVolume = pp.GetComponent<PostProcessVolume>();
            var profile = postProcessVolume.profile;
            var lensDistortion = profile.GetSetting<LensDistortion>();
            lensDistortion.intensity.value = -20f;

            var colorGrading = profile.GetSetting<ColorGrading>();
            colorGrading.contrast.value = 60f;
        }
    }
}