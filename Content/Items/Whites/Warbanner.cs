using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Items.Whites
{
    public class Warbanner : ItemBase
    {
        public override string Name => ":: Items : Whites :: Warbanner";
        public override string InternalPickupToken => "wardOnLevel";

        public override string PickupText => $"Drop a Warbanner on level up or starting the Teleporter event. Grants allies{(enableMovementSpeed ? " movement speed" : "")}{(enableAttackSpeed ? (enableMovementSpeed ? " and" : "") + " attack speed" : "")}.";

        public override string DescText => "On <style=cIsUtility>level up</style> or starting the <style=cIsUtility>Teleporter event</style>, drop a banner that strengthens all allies" +
            StackDesc(baseRadius, radiusPerStack, init => $" within <style=cIsUtility>{m(init)}</style>{{Stack}}", m) + "." +
            StackDesc(attackSpeedAndMovementSpeed, attackSpeedAndMovementSpeedStack, init => $" Raise {(enableMovementSpeed ? " <style=cIsUtility>movement speed</style>" : "")}{(enableAttackSpeed ? (enableMovementSpeed ? " and" : "") + " <style=cIsDamage>attack speed</style>" : "")} by <style=cIsDamage>{d(init)}</style>{{Stack}}.", d);

        [ConfigField("Base Radius", 20f)]
        public static float baseRadius;
        [ConfigField("Radius Per Stack", 0f)]
        public static float radiusPerStack;
        [ConfigField("Radius is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float radiusIsHyperbolic;

        [ConfigField("Attack Speed and Movement Speed", "Decimal.", 0.3f)]
        public static float attackSpeedAndMovementSpeed;
        [ConfigField("Attack Speed and Movement Speed Per Stack", "Decimal.", 0.1f)]
        public static float attackSpeedAndMovementSpeedStack;
        [ConfigField("Attack Speed and Movement Speed is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float attackSpeedAndMovementSpeedIsHyperbolic;

        [ConfigField("Increase Movement Speed", true)]
        public static bool enableMovementSpeed;
        [ConfigField("Increase Attack Speed", true)]
        public static bool enableAttackSpeed;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.TeleporterInteraction.ChargingState.OnEnter += ChargingState_OnEnter;
            IL.RoR2.Items.WardOnLevelManager.OnCharacterLevelUp += WardOnLevelManager_OnCharacterLevelUp;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = Util.GetItemCountForTeam(sender.teamComponent.teamIndex, RoR2Content.Items.WardOnLevel.itemIndex, false);
                if (sender.HasBuff(RoR2Content.Buffs.Warbanner.buffIndex))
                {
                    float ret = StackAmount(attackSpeedAndMovementSpeed, attackSpeedAndMovementSpeedStack, stack, attackSpeedAndMovementSpeedIsHyperbolic);
                    if (enableAttackSpeed) args.baseAttackSpeedAdd += ret;
                    if (enableMovementSpeed) args.moveSpeedMultAdd += ret;
                }
            }
        }

        public static void WardOnLevelManager_OnCharacterLevelUp(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchCallOrCallvirt<BuffWard>("set_" + nameof(BuffWard.Networkradius))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc_1);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(baseRadius, radiusPerStack, stack, radiusIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Warbanner Radius 1 hook");
        }

        public static void ChargingState_OnEnter(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchCallOrCallvirt<BuffWard>("set_" + nameof(BuffWard.Networkradius))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, 8);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(baseRadius, radiusPerStack, stack, radiusIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Warbanner Radius 1 hook");
        }
    }
}