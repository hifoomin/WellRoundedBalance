using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Lunars
{
    public class Transcendence : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Transcendence";
        public override string InternalPickupToken => "shieldOnly";

        public override string PickupText => "Convert all your health into shield and increase maximum health... <color=#FF7F7F>BUT increase shield cooldown time.</color>\\n";
        public override string DescText => "<style=cIsHealing>Convert</style> all but <style=cIsHealing>1 health</style> into <style=cIsHealing>regenerating shields</style>. <style=cIsHealing>Gain 50% <style=cStack>(+25% per stack)</style> maximum health</style>. Increase <style=cIsUtility>shield cooldown time</style> by <style=cIsUtility>1s</style> <style=cStack>(+1s per stack)</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
        }

        private void CharacterBody_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchLdfld<CharacterBody>("outOfDangerStopwatch"),
                 x => x.MatchLdcR4(7f)))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((outOfDangerDelay, self) =>
                {
                    if (self.inventory)
                    {
                        var stack = self.inventory.GetItemCount(RoR2Content.Items.ShieldOnly);
                        if (stack > 0)
                        {
                            outOfDangerDelay += 1f * stack;
                        }
                    }
                    return outOfDangerDelay;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Transcendence Shield Timer hook");
            }
        }
    }
}