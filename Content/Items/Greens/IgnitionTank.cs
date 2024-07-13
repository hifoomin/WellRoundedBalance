using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class IgnitionTank : ItemBase<IgnitionTank>
    {
        public override string Name => ":: Items :: Greens :: Ignition Tank";
        public override ItemDef InternalPickup => DLC1Content.Items.StrengthenBurn;

        public override string PickupText => (igniteChanceOnHit > 0 ? "Gain a " + igniteChanceOnHit + "% chance to ignite enemies on hit. " : "") +
                                             "Your ignite effects deal " + igniteDamageIncrease + "x damage.";

        public override string DescText => (igniteChanceOnHit > 0 ? "Gain <style=cIsDamage>" + igniteChanceOnHit + "%</style> chance to <style=cIsDamage>ignite</style> enemies on hit. " : "") +
                                           "Ignite effects deal <style=cIsDamage>+" + d(igniteDamageIncrease) + "</style> <style=cStack>(+" + d(igniteDamageIncrease) + " per stack)</style> more damage over time.";

        [ConfigField("Ignite Chance On Hit", 15f)]
        public static float igniteChanceOnHit;

        [ConfigField("Ignite Damage Increase", "Decimal.", 1.5f)]
        public static float igniteDamageIncrease;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            // IL.RoR2.StrengthenBurnUtils.CheckDotForUpgrade += StrengthenBurnUtils_CheckDotForUpgrade;
            On.RoR2.StrengthenBurnUtils.CheckDotForUpgrade += StrengthenBurnUtils_CheckDotForUpgrade1;
        }

        private void StrengthenBurnUtils_CheckDotForUpgrade1(On.RoR2.StrengthenBurnUtils.orig_CheckDotForUpgrade orig, Inventory inventory, ref InflictDotInfo dotInfo)
        {
            // sorry but WHY IS THERE AN INT AAAA
            if (dotInfo.dotIndex == DotController.DotIndex.Burn || dotInfo.dotIndex == DotController.DotIndex.Helfire)
            {
                int itemCount = inventory.GetItemCount(DLC1Content.Items.StrengthenBurn);
                if (itemCount > 0)
                {
                    dotInfo.preUpgradeDotIndex = new DotController.DotIndex?(dotInfo.dotIndex);
                    dotInfo.dotIndex = DotController.DotIndex.StrongerBurn;
                    float increase = 1f + igniteDamageIncrease * itemCount;
                    dotInfo.damageMultiplier *= increase;
                    dotInfo.totalDamage *= increase;
                }
            }
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport report)
        {
            var attacker = report.attacker;
            if (!attacker)
            {
                return;
            }
            var body = report.attackerBody;

            if (!body)
            {
                return;
            }
            var inventory = body.inventory;
            if (!inventory)
            {
                return;
            }
            var stack = inventory.GetItemCount(DLC1Content.Items.StrengthenBurn);
            if (stack > 0)
            {
                if (Util.CheckRoll(igniteChanceOnHit * report.damageInfo.procCoefficient, body.master.luck))
                {
                    InflictDotInfo blaze = new()
                    {
                        attackerObject = report.damageInfo.attacker,
                        victimObject = report.victim.gameObject,
                        dotIndex = DotController.DotIndex.Burn,
                        damageMultiplier = 1f,
                        totalDamage = report.damageInfo.damage * 0.5f
                    };
                    StrengthenBurnUtils.CheckDotForUpgrade(inventory, ref blaze);
                    DotController.InflictDot(ref blaze);
                }
            }
        }

        private void StrengthenBurnUtils_CheckDotForUpgrade(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcI4(3)))
            {
                c.Index += 1;
                c.Next.Operand = igniteDamageIncrease;
            }
            else
            {
                Logger.LogError("Failed to apply Ignition Tank Burn Damage hook");
            }
        }
    }
}