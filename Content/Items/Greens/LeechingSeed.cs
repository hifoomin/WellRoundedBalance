using MonoMod.Cil;
using RoR2;

namespace WellRoundedBalance.Items.Greens
{
    public class LeechingSeed : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Leeching Seed";
        public override string InternalPickupToken => "seed";

        public override string PickupText => "Dealing damage heals you.";

        public override string DescText => "Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>1 <style=cStack>(+1 per stack)</style> health</style>, plus an additional <style=cIsHealing>0.7</style> <style=cStack>(+0.35 per stack)</style> <style=cIsHealing>health</style> regardless of source.";

        public override void Init()
        {
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
                        AB.healthComponent.Heal(0.75f + 0.35f * (Stack - 1), HealMask, true);
                    }
                }
            }
        }
    }
}