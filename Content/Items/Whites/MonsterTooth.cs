using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    public class MonsterTooth : ItemBase
    {
        public override string Name => ":: Items : Whites :: MonsterTooth";
        public override string InternalPickupToken => "tooth";

        public override string PickupText => "Drop a healing orb on kill.";

        public override string DescText => "Killing an enemy spawns a <style=cIsHealing>healing orb</style> that heals for <style=cIsHealing>" + baseFlatHealing + "</style>" +
                                           (flatHealingPerStack > 0 ? " <style=cStack>(+" + flatHealingPerStack + " per stack)</style>" : "") +
                                           (basePercentHealing > 0 || percentHealingPerStack > 0 ? " plus an additional <style=cIsHealing>" + d(basePercentHealing) +
                                           (percentHealingPerStack > 0 ? " <style=cStack>(+" + d(percentHealingPerStack) + " per stack)</style></style> of <style=cIsHealing>maximum health</style>." : ".") : ".");

        [ConfigField("Base Flat Healing", 6f)]
        public static float baseFlatHealing;

        [ConfigField("Flat Healing Per Stack", 0f)]
        public static float flatHealingPerStack;

        [ConfigField("Base Percent Healing", "Decimal.", 0.015f)]
        public static float basePercentHealing;

        [ConfigField("Percent Healing Per Stack", "Decimal.", 0.015f)]
        public static float percentHealingPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Items), "Tooth")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessItem));
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Monster Tooth Deletion hook");
            }
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (!damageReport.victim)
            {
                return;
            }

            if (!damageReport.attacker)
            {
                return;
            }

            if (!damageReport.attackerBody)
            {
                return;
            }

            var inventory = damageReport.attackerBody.inventory;
            if (!inventory)
            {
                return;
            }

            var vector = damageReport.victim.transform.position;

            var stack = inventory.GetItemCount(RoR2Content.Items.Tooth);
            if (stack > 0)
            {
                float scale = Mathf.Pow(stack, 0.25f);
                var monsterToothPrefab = Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/HealPack"), vector, Random.rotation);
                var teamFilter = monsterToothPrefab.GetComponent<TeamFilter>();
                if (teamFilter)
                {
                    teamFilter.teamIndex = damageReport.attackerBody.teamComponent.teamIndex;
                }
                var healthPickup = monsterToothPrefab.GetComponentInChildren<HealthPickup>();
                if (healthPickup)
                {
                    healthPickup.flatHealing = baseFlatHealing + flatHealingPerStack * (stack - 1);
                    healthPickup.fractionalHealing = basePercentHealing + percentHealingPerStack * (stack - 1);
                }
                monsterToothPrefab.transform.localScale = new Vector3(scale, scale, scale);
                NetworkServer.Spawn(monsterToothPrefab);
            }
        }
    }
}