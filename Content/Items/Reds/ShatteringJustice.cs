using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Reds
{
    public class ShatteringJustice : ItemBase<ShatteringJustice>
    {
        public static BuffDef pulverized;
        public override string Name => ":: Items ::: Reds :: Shattering Justice";
        public override ItemDef InternalPickup => RoR2Content.Items.ArmorReductionOnHit;

        public override string PickupText => "Reduce the armor of enemies after repeatedly striking them.";

        public override string DescText => "After hitting an enemy <style=cIsDamage>" + hitCount + "</style> times, reduce their <style=cIsDamage>armor</style> by <style=cIsDamage>" + baseArmorReduction + "</style>" +
                                           (armorReductionPerStack > 0 ? " <style=cStack>(+" + armorReductionPerStack + " per stack)</style>" : "") +
                                           " for <style=cIsDamage>" + baseDebuffDuration + "</style> seconds.";

        [ConfigField("Hit Count", 5)]
        public static int hitCount;

        [ConfigField("Base Armor Reduction", 60f)]
        public static float baseArmorReduction;

        [ConfigField("Armor Reduction Per Stack", 30f)]
        public static float armorReductionPerStack;

        [ConfigField("Base Debuff Duration", 8f)]
        public static float baseDebuffDuration;

        public override void Init()
        {
            pulverized = ScriptableObject.CreateInstance<BuffDef>();
            pulverized.canStack = false;
            pulverized.isDebuff = true;
            pulverized.isCooldown = false;
            pulverized.isHidden = false;
            pulverized.startSfx = Utils.Paths.NetworkSoundEventDef.nsePulverizeBuildupBuffApplied.Load<NetworkSoundEventDef>();
            pulverized.iconSprite = Utils.Paths.BuffDef.bdPulverized.Load<BuffDef>().iconSprite;
            pulverized.buffColor = new Color32(245, 159, 73, 255);
            pulverized.name = "Shattering Justice Armor Reduction";

            ContentAddition.AddBuffDef(pulverized);
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            globalStack = Util.GetItemCountGlobal(RoR2Content.Items.ArmorReductionOnHit.itemIndex, true);
        }

        public static int globalStack;

        private void CharacterModel_UpdateOverlays(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Pulverized")))
            {
                c.Remove();
                c.Emit<ShatteringJustice>(OpCodes.Ldsfld, nameof(pulverized));
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(pulverized))
            {
                args.armorAdd -= baseArmorReduction + armorReductionPerStack * (globalStack - 1);
            }
        }

        private void HealthComponent_TakeDamageProcess(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "PulverizeBuildup"),
                x => x.MatchCallOrCallvirt<CharacterBody>("GetBuffCount"),
                x => x.MatchLdcI4(out _)))
            {
                c.Index += 3;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return hitCount;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Shattering Justice Hit Count hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Pulverized"),
                x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff")))
            {
                c.Remove();
                c.Emit<ShatteringJustice>(OpCodes.Ldsfld, nameof(pulverized));
            }
            else
            {
                Logger.LogError("Failed to apply Shattering Justice Debuff 1 hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Pulverized"),
                x => x.MatchLdcR4(8f)))
            {
                c.Remove();
                c.Emit<ShatteringJustice>(OpCodes.Ldsfld, nameof(pulverized));
                c.Index++;
                c.Next.Operand = baseDebuffDuration;
                for (int i = 0; i < 3; i++)
                {
                    c.Remove();
                }
            }
            else
            {
                Logger.LogError("Failed to apply Shattering Justice Debuff 2 & Length hook");
            }
        }
    }
}