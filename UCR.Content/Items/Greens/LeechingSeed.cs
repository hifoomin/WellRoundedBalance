using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace UltimateCustomRun.Items.Greens
{
    public class LeechingSeed : ItemBase
    {
        public static float AdditionalHealing;
        public static float StackAdditionalHealing;

        public override string Name => ":: Items :: Greens :: Leeching Seed";
        public override string InternalPickupToken => "seed";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>1 <style=cStack>(+1 per stack)</style> health</style>" +
                               (AdditionalHealing > 0 ? " and an additional <style=cIsHealing>" + AdditionalHealing + " <style=cStack>(+" + StackAdditionalHealing + " per stack)</style> health</style> regardless of source." : ".");

        public override void Init()
        {
            AdditionalHealing = ConfigOption(0f, "Additional Healing, regardless of Proc Coefficient", "Vanilla is 0");
            ROSOption("Greens", 0f, 3f, 0.05f, "2");
            StackAdditionalHealing = ConfigOption(0f, "Stack Additional Healing, regardless of Proc Coefficient", "Per Stack. Vanilla is 0");
            ROSOption("Greens", 0f, 3f, 0.05f, "2");
            base.Init();
        }

        public override void Hooks()
        {
            GlobalEventManager.onServerDamageDealt += AddUnconditionalHealing;
        }

        public static void AddUnconditionalHealing(DamageReport report)
        {
            var AB = report.attackerBody;
            if (report != null && AB != null)
            {
                var HealMask = report.damageInfo.procChainMask;
                if (AB.inventory)
                {
                    var Stack = AB.inventory.GetItemCount(RoR2Content.Items.Seed);
                    if (Stack > 0)
                    {
                        AB.healthComponent.Heal(AdditionalHealing + StackAdditionalHealing * (Stack - 1f), HealMask, true);
                    }
                }
            }
        }
    }
}