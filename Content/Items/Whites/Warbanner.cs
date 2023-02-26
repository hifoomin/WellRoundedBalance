using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Items.Whites
{
    public class Warbanner : ItemBase
    {
        public override string Name => ":: Items : Whites :: Warbanner";
        public override string InternalPickupToken => "wardOnLevel";

        public override string PickupText => "Drop a Warbanner on level up or starting the Teleporter event. Grants allies movement speed and attack speed.";

        public override string DescText => "On <style=cIsUtility>level up</style> or starting the <style=cIsUtility>Teleporter event</style>, drop a banner that strengthens all allies within <style=cIsUtility>" + baseRadius + "m</style>" +
                                           (radiusPerStack > 0 ? " <style=cStack>(+ " + radiusPerStack + "m per stack)</style>." : ".") +
                                           " Raise <style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>30%</style> " +
                                           (attackSpeedAndMovementSpeedPerStack > 0 ? "<style=cStack>(+" + d(attackSpeedAndMovementSpeedPerStack) + " per stack)</style>." : ".");

        [ConfigField("Base Radius", "", 20f)]
        public static float baseRadius;

        [ConfigField("Radius Per Stack", "", 0f)]
        public static float radiusPerStack;

        [ConfigField("Attack Speed and Movement Speed Per Stack", "Decimal.", 0.1f)]
        public static float attackSpeedAndMovementSpeedPerStack;

        public override void Init()
        {
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
                    args.baseAttackSpeedAdd += attackSpeedAndMovementSpeedPerStack * (stack - 1);
                    args.moveSpeedMultAdd += attackSpeedAndMovementSpeedPerStack * (stack - 1);
                }
            }
        }

        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(8),
                    x => x.MatchLdcR4(8)))
            {
                c.Next.Operand = baseRadius - radiusPerStack;
                c.Index += 1;
                c.Next.Operand = radiusPerStack;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Warbanner Radius 1 hook");
            }
        }

        public static void ChangeRadiusTP(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(8),
                    x => x.MatchLdcR4(8)))
            {
                c.Next.Operand = baseRadius - radiusPerStack;
                c.Index += 1;
                c.Next.Operand = radiusPerStack;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Warbanner Radius 2 hook");
            }
        }
    }
}