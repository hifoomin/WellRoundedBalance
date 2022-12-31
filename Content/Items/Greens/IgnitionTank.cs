using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace WellRoundedBalance.Items.Greens
{
    public class IgnitionTank : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Ignition Tank";
        public override string InternalPickupToken => "strengthenBurn";

        public override string PickupText => "Gain a 15% chance to ignite enemies on hit. Your ignite effects deal quadruple damage.";

        public override string DescText => "Gain <style=cIsDamage>15%</style> <style=cStack>(+15% per stack)</style> chance to <style=cIsDamage>ignite</style> enemies on hit. Ignite effects deal <style=cIsDamage>+300%</style> <style=cStack>(+300% per stack)</style> more damage over time.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += AddBurn;
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
                            if (Util.CheckRoll(15f * damageInfo.procCoefficient, damageInfo.attacker.GetComponent<CharacterBody>().master.luck))
                            {
                                InflictDotInfo blaze = new()
                                {
                                    attackerObject = damageInfo.attacker,
                                    victimObject = victim,
                                    dotIndex = DotController.DotIndex.Burn,
                                    damageMultiplier = 1f
                                };
                                DotController.InflictDot(ref blaze);
                                StrengthenBurnUtils.CheckDotForUpgrade(inventory, ref blaze);
                            }
                        }
                    }
                }
            }
            orig(self, damageInfo, victim);
        }
    }
}