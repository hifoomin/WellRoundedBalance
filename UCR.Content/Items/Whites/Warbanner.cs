using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltimateCustomRun.Items.Whites
{
    public class Warbanner : ItemBase
    {
        public static float BaseDamage;
        public static bool StackBaseDamage;
        public static float Radius;
        public static float StackRadius;
        public static float PercentDamage;
        public static bool StackPercentDamage;
        public static float BuffDuration;
        public static float BuffAttackSpeed;
        public static float BuffMoveSpeed;

        public override string Name => ":: Items : Whites :: Warbanner";
        public override string InternalPickupToken => "wardOnLevel";
        public override bool NewPickup => true;

        public override string PickupText => "Drop a Warbanner on level up or starting the Teleporter event. Grants allies" +
                                             (BaseDamage != 0f || PercentDamage != 0f ? " damage," : "") +
                                             " movement Speed and attack Speed.";

        public override string DescText => "On <style=cIsUtility>level up</style> or starting the <style=cIsUtility>Teleporter event</style>, drop a banner that strengthens all allies within <style=cIsUtility>" + Radius + "m</style> <style=cStack>(+" + StackRadius + "m per stack)</style>. Raise" +
                                           (PercentDamage != 0f ? " <style=cIsDamage>damage</style> by <style=cIsDamage>" + d(PercentDamage) + "</style>, " +
                                           (StackPercentDamage ? "<style=cStack>(+" + PercentDamage + " per stack)</style>. " : "") : "") +
                                           (BaseDamage != 0f ? " <style=cIsDamage>base damage</style> by <style=cIsDamage>" + BaseDamage + "</style>, " +
                                           (StackBaseDamage ? "<style=cStack>(+" + BaseDamage + " per stack)</style>. " : "") : "") +
                                           "<style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>30%</style>.";

        public override void Init()
        {
            BaseDamage = ConfigOption(0f, "Base Damage", "Vanilla is 0");
            ROSOption("Whites", 0f, 10f, 0.5f, "1");
            StackBaseDamage = ConfigOption(false, "Stack Base Damage?", "Vanilla is false");
            ROSOption("Whites", 0f, 5f, 0.01f, "1");
            Radius = ConfigOption(16f, "Base Range", "Vanilla is 16");
            ROSOption("Whites", 0f, 30f, 1f, "1");
            StackRadius = ConfigOption(8f, "Stack Range", "Per Stack. Vanilla is 8");
            ROSOption("Whites", 0f, 30f, 1f, "1");
            PercentDamage = ConfigOption(0f, "Percent Damage", "Decimal. Vanilla is 0");
            ROSOption("Whites", 0f, 1f, 0.01f, "1");
            StackPercentDamage = ConfigOption(false, "Stack Percent Damage?", "Vanilla is false");
            ROSOption("Whites", 0f, 5f, 0.01f, "1");
            BuffDuration = ConfigOption(1.5f, "Buff Duration", "Vanilla is 1.5");
            ROSOption("Whites", 0f, 5f, 0.5f, "1");
            BuffAttackSpeed = ConfigOption(0.3f, "Buff Attack Speed", "Decimal. Vanilla is 0.3");
            ROSOption("Whites", 0f, 1f, 0.05f, "1");
            BuffMoveSpeed = ConfigOption(0.3f, "Buff Move Speed", "Decimal. Vanilla is 0.3");
            ROSOption("Whites", 0f, 1f, 0.05f, "1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.TeleporterInteraction.ChargingState.OnEnter += ChangeRadiusTP;
            IL.RoR2.Items.WardOnLevelManager.OnCharacterLevelUp += ChangeRadius;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
                if (sender.HasBuff(RoR2Content.Buffs.Warbanner.buffIndex))
                {
                    args.baseDamageAdd += (StackBaseDamage ? BaseDamage * stack : BaseDamage);
                    args.damageMultAdd += (StackPercentDamage ? PercentDamage * stack : PercentDamage);
                }
            }
        }

        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8),
                x => x.MatchLdcR4(8)
            );
            c.Next.Operand = Radius - StackRadius;
            c.Index += 1;
            c.Next.Operand = StackRadius;
        }

        public static void ChangeRadiusTP(ILContext il)
        {
            ILCursor c = new(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8),
                x => x.MatchLdcR4(8)
            );
            c.Next.Operand = Radius - StackRadius;
            c.Index += 1;
            c.Next.Operand = StackRadius;
        }

        public static void ChangeBuffDuration()
        {
            var w = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/WardOnLevel/WarbannerWard.prefab").WaitForCompletion();
            w.GetComponent<BuffWard>().buffDuration = BuffDuration;
        }

        public static void ChangeBuffStats(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "Warbanner"),
                x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdloc(75),
                x => x.MatchLdcR4(0.3f)
            );
            c.Index += 4;
            c.Next.Operand = BuffMoveSpeed;

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "Warbanner"),
                x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdloc(83),
                x => x.MatchLdcR4(0.3f)
            );
            c.Index += 4;
            c.Next.Operand = BuffAttackSpeed;
        }
    }
}