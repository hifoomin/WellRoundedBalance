using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class IgnitionTank : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Ignition Tank";
        public override string InternalPickupToken => "strengthenBurn";

        public override string PickupText => (igniteChanceOnHit > 0 ? "Gain a " + igniteChanceOnHit + "% chance to ignite enemies on hit. " : "") +
                                             "Your ignite effects deal " + igniteDamageIncrease + "x damage.";

        public override string DescText => (igniteChanceOnHit > 0 ? "Gain <style=cIsDamage>" + igniteChanceOnHit + "%</style> chance to <style=cIsDamage>ignite</style> enemies on hit. " : "") +
                                           "Ignite effects deal <style=cIsDamage>+" + d(igniteDamageIncrease) + "</style> <style=cStack>(+" + d(igniteDamageIncrease) + " per stack)</style> more damage over time.";

        [ConfigField("Ignite Chance On Hit", 15f)]
        public static float igniteChanceOnHit;

        [ConfigField("Ignite Damage Increase", "Decimal.", 2)]
        public static int igniteDamageIncrease;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += AddBurn;
            IL.RoR2.StrengthenBurnUtils.CheckDotForUpgrade += StrengthenBurnUtils_CheckDotForUpgrade;
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

        public void AddBurn(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                var body = attacker.GetComponent<CharacterBody>();
                if (body)
                {
                    var inventory = body.inventory;
                    if (inventory)
                    {
                        var stack = inventory.GetItemCount(DLC1Content.Items.StrengthenBurn);
                        if (stack > 0)
                        {
                            if (Util.CheckRoll(igniteChanceOnHit * damageInfo.procCoefficient, damageInfo.attacker.GetComponent<CharacterBody>().master.luck))
                            {
                                InflictDotInfo blaze = new()
                                {
                                    attackerObject = damageInfo.attacker,
                                    victimObject = victim,
                                    dotIndex = DotController.DotIndex.Burn,
                                    damageMultiplier = 1f,
                                    totalDamage = damageInfo.damage * 0.5f
                                };
                                StrengthenBurnUtils.CheckDotForUpgrade(inventory, ref blaze);
                                DotController.InflictDot(ref blaze);
                            }
                        }
                    }
                }
            }
            orig(self, damageInfo, victim);
        }
    }
}