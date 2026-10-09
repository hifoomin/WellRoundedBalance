using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.VoidWhites
{
    public class WeepingFungus : ItemBase<WeepingFungus>
    {
        public override string Name => ":: Items :::::: Voids :: Weeping Fungus";
        public override ItemDef InternalPickup => DLC1Content.Items.MushroomVoid;

        public override string PickupText => "Heal while sprinting. <style=cIsVoid>Corrupts all Bustling Fungi</style>.";
        public override string DescText => "<style=cIsHealing>Heals</style> for <style=cIsHealing>" + d(basePercentHealing) + "</style> <style=cStack>(+" + d(percentHealingPerStack) + " per stack)</style> of your <style=cIsHealing>health</style> every second <style=cIsUtility>while sprinting</style>. <style=cIsVoid>Corrupts all Bustling Fungi</style>.";

        [ConfigField("Base Percent Healing", "Decimal.", 0.012f)]
        public static float basePercentHealing;

        [ConfigField("Percent Healing Per Stack", "Decimal.", 0.012f)]
        public static float percentHealingPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            MushroomVoidBehavior.healPercentagePerStack = percentHealingPerStack * 0.5f;
        }
    }
}