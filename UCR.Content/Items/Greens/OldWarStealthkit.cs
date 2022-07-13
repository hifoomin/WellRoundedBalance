using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Greens
{
    public class OldWarStealthkit : ItemBase
    {
        public static float BuffArmor;
        public static bool StackBuffArmor;
        public static float Armor;
        public static bool StackArmor;

        public override string Name => ":: Items :: Greens :: Old War Stealthkit";
        public override string InternalPickupToken => "phasing";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => (Armor != 0f ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + Armor + "</style> <style=cStack>(+" + Armor + " per stack)</style>. " : "") +
                                           "Falling below <style=cIsHealth>25% health</style> causes you to gain <style=cIsUtility>40% movement speed</style>" +
                                           (BuffArmor != 0f ? ", <style=cIsHealing>" + BuffArmor + " armor</style>" +
                                           (StackBuffArmor ? " <style=cStack>(+" + BuffArmor + " per stack)</style>" : "") : "") +
                                           " and <style=cIsUtility>invisibility</style> for <style=cIsUtility>5s</style>. Recharges every <style=cIsUtility>30 seconds</style> <style=cStack>(-50% per stack)</style>.";

        public override void Init()
        {
            BuffArmor = ConfigOption(0f, "Buff Armor", "With Buff. Vanilla is 0");
            ROSOption("Greens", 0f, 200f, 5f, "2");
            StackBuffArmor = ConfigOption(false, "Stack Buff Armor?", "Vanilla is false");
            ROSOption("Greens", 0f, 10f, 1f, "2");
            Armor = ConfigOption(0f, "Armor", "Vanilla is 0");
            ROSOption("Greens", 0f, 50f, 5f, "2");
            StackArmor = ConfigOption(false, "Stack Armor?", "Vanilla is false");
            ROSOption("Greens", 0f, 10f, 1f, "2");
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
                    args.armorAdd += StackBuffArmor ? BuffArmor * stack : BuffArmor;
                }
                if (stack > 0)
                {
                    args.armorAdd += StackArmor ? Armor * stack : Armor;
                }
            }
        }
    }
}