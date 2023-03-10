using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.VoidWhites
{
    public class WeepingFungus : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Weeping Fungus";
        public override string InternalPickupToken => "mushroomVoid";

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
            IL.RoR2.MushroomVoidBehavior.FixedUpdate += ChangeHealing;
        }

        private void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.01f)))
            {
                c.Index += 1;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, MushroomVoidBehavior, float>>((useless, self) =>
                {
                    return (basePercentHealing + percentHealingPerStack * (self.stack - 1)) * 0.5f;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Weeping Fungus Healing hook");
            }
        }
    }
}