using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Items;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class Warbanner : ItemBase<Warbanner>
    {
        public override string Name => ":: Items : Whites :: Warbanner";
        public override ItemDef InternalPickup => RoR2Content.Items.WardOnLevel;

        public override string PickupText => $"Drop a Warbanner upon levelling up or encountering a boss. Grants allies{(enableMovementSpeed ? " movement speed" : "")}{(enableAttackSpeed ? (enableMovementSpeed ? " and" : "") + " attack speed" : "")}.";

        public override string DescText => "Upon <style=cIsUtility>levelling up</style> or <style=cIsUtility>encountering a boss</style>, drop a banner that strengthens all allies" +
            StackDesc(baseRadius, radiusPerStack, init => $" within <style=cIsUtility>{m(init)}</style>{{Stack}}", m) + "." +
            StackDesc(attackSpeedAndMovementSpeed, attackSpeedAndMovementSpeedStack, init => $" Raise {(enableMovementSpeed ? " <style=cIsUtility>movement speed</style>" : "")}{(enableAttackSpeed ? (enableMovementSpeed ? " and" : "") + " <style=cIsDamage>attack speed</style>" : "")} by <style=cIsDamage>{d(init)}</style>{{Stack}}.", d);

        [ConfigField("Base Radius", 22f)]
        public static float baseRadius;

        [ConfigField("Radius Per Stack", 0f)]
        public static float radiusPerStack;

        [ConfigField("Radius is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float radiusIsHyperbolic;

        [ConfigField("Base Attack Speed and Movement Speed", "Decimal.", 0.2f)]
        public static float attackSpeedAndMovementSpeed;

        [ConfigField("Attack Speed and Movement Speed Per Stack", "Decimal.", 0.2f)]
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
            IL.RoR2.TeleporterInteraction.ChargingState.OnEnter += Change;
            IL.RoR2.Items.WardOnLevelManager.OnCharacterLevelUp += Change;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.ScriptedCombatEncounter.BeginEncounter += ScriptedCombatEncounter_BeginEncounter;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        public static int globalStack = 0;

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            globalStack = Util.GetItemCountGlobal(RoR2Content.Items.WardOnLevel.itemIndex, false);
        }

        private void ScriptedCombatEncounter_BeginEncounter(On.RoR2.ScriptedCombatEncounter.orig_BeginEncounter orig, ScriptedCombatEncounter self)
        {
            orig(self);
            if (NetworkServer.active)
                SpawnWarbanner();
        }

        private void SpawnWarbanner()
        {
            foreach (CharacterBody body in CharacterBody.readOnlyInstancesList)
            {
                if (body.isPlayerControlled && body.inventory)
                {
                    var stack = body.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
                    if (stack > 0)
                    {
                        var warbanner = UnityEngine.Object.Instantiate(WardOnLevelManager.wardPrefab, body.transform.position, Quaternion.identity);
                        warbanner.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                        warbanner.GetComponent<BuffWard>().Networkradius = baseRadius + radiusPerStack * (stack - 1);
                        NetworkServer.Spawn(warbanner);
                    }
                }
            }
        }

        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                if (sender.HasBuff(RoR2Content.Buffs.Warbanner.buffIndex))
                {
                    float ret = StackAmount(attackSpeedAndMovementSpeed, attackSpeedAndMovementSpeedStack, globalStack, attackSpeedAndMovementSpeedIsHyperbolic);

                    if (enableAttackSpeed) args.baseAttackSpeedAdd += ret - 0.3f;
                    if (enableMovementSpeed) args.moveSpeedMultAdd += ret - 0.3f;
                }
            }
        }

        public static void Change(ILContext il)
        {
            ILCursor c = new(il);
            int stack = GetItemLoc(c, nameof(RoR2Content.Items.WardOnLevel));
            if (stack != -1 && c.TryGotoNext(x => x.MatchCallOrCallvirt<BuffWard>("set_" + nameof(BuffWard.Networkradius))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(baseRadius, radiusPerStack, stack, radiusIsHyperbolic));
            }
            else Logger.LogError("Failed to apply Warbanner Radius hook");
        }
    }
}