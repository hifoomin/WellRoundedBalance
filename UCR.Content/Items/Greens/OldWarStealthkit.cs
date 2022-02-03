using RoR2;
using R2API;

namespace UltimateCustomRun
{
    public class OldWarStealthkit : Based
    {
        public static float buffarmor;
        public static bool stackbuffarmor;

        public override string Name => ":: Items :: Greens :: Old War Stealthkit";
        public override string InternalPickupToken => "phasing";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public static bool oArmor = buffarmor != 0f;
        public static bool oArmorStack = stackbuffarmor;

        public override string DescText => "Falling below <style=cIsHealth>25% health</style> causes you to gain <style=cIsUtility>40% movement speed</style>" +
                                           (oArmor ? ", <style=cIsHealing>" + buffarmor + " armor</style>" +
                                           (oArmorStack ? " <style=cStack>(+" + buffarmor + " per stack)</style>" : "") : "") +
                                           " and <style=cIsUtility>invisibility</style> for <style=cIsUtility>5s</style>. Recharges every <style=cIsUtility>30 seconds</style> <style=cStack>(-50% per stack)</style>.";


        public override void Init()
        {
            buffarmor = ConfigOption(0f, "Armor", "With Buff. Vanilla is 0");
            stackbuffarmor = ConfigOption(false, "Stack Armor with Buff?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Phasing);
                var buffp1 = sender.HasBuff(RoR2.RoR2Content.Buffs.Cloak);
                var buffp2 = sender.HasBuff(RoR2.RoR2Content.Buffs.CloakSpeed);
                // periphery ZASED
                if (stack > 0 && buffp1 && buffp2)
                {
                    args.armorAdd += stackbuffarmor ? buffarmor * stack : buffarmor;
                }
            }
        }
    }
}
