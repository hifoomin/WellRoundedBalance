using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class RoseBuckler : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Rose Buckler";
        public override ItemDef InternalPickup => RoR2Content.Items.SprintArmor;

        public override string PickupText => "Reduce incoming damage while sprinting.";

        public override string DescText => StackDesc(armor, armorStack, init => $"<style=cIsHealing>Increase armor</style> by <style=cIsHealing>{init}</style>{{Stack}} while <style=cIsUtility>sprinting</style>.", noop);

        [ConfigField("Armor Increase", "", 25)]
        public static int armor;

        [ConfigField("Armor Increase per Stack", "", 15)]
        public static int armorStack;

        [ConfigField("Armor Increase is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float armorIsHyperbolic;

        public override void Init()
        {
            base.Init();
        }
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory && sender.isSprinting)
            {
                int stack = sender.inventory.GetItemCount(InternalPickup);
                args.armorAdd += StackAmount(armor, armorStack, stack, armorIsHyperbolic) - (30 * stack);
            }
        }
    }
}