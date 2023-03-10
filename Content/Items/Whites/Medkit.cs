using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class Medkit : ItemBase
    {
        public override string Name => ":: Items : Whites :: Medkit";
        public override string InternalPickupToken => "medkit";

        public override string PickupText => "Receive a delayed heal after taking damage.";

        public override string DescText => "2 seconds after getting hurt, <style=cIsHealing>heal</style> for " +
            StackDesc(flatHealing, flatHealingStack, init => $"<style=cIsHealing>{init}</style>{{Stack}}", noop) +
            StackDesc(percentHealing, percentHealingStack, init => (flatHealing > 0 || flatHealingStack > 0 ? "plus an additional " : "") + $"<style=cIsHealing>{d(init)}</style>{{Stack}} of <style=cIsHealing>maximum health</style>", d) + ".";

        [ConfigField("Flat Healing", 20f)]
        public static float flatHealing;

        [ConfigField("Flat Healing per Stack", 0f)]
        public static float flatHealingStack;

        [ConfigField("Flat Healing is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float flatHealingIsHyperbolic;

        [ConfigField("Percent Healing", "Decimal.", 0.035f)]
        public static float percentHealing;

        [ConfigField("Percent Healing per Stack", "Decimal.", 0.035f)]
        public static float percentHealingStack;

        [ConfigField("Percent Healing is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float percentHealingIsHyperbolic;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RemoveBuff_BuffIndex += CharacterBody_RemoveBuff_BuffIndex;
        }

        private void CharacterBody_RemoveBuff_BuffIndex(ILContext il)
        {
            ILCursor c = new(il);
            int bufftype = -1;
            c.TryGotoNext(x => x.MatchLdarg(out bufftype), x => x.MatchLdcI4(-1), x => x.MatchBneUn(out _));
            int count = GetItemLoc(c, nameof(RoR2Content.Items.Medkit));
            if (bufftype == -1 || count == -1) return;
            int cmp = -1;
            if (c.TryGotoNext(x => x.MatchCallOrCallvirt<CharacterBody>("get_" + nameof(CharacterBody.maxHealth))) && c.TryGotoNext(x => x.MatchStloc(out _)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc, count);
                c.EmitDelegate<Func<CharacterBody, int, float>>((self, stack) => self.maxHealth * StackAmount(percentHealing, percentHealingStack, stack, percentHealingIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Medkit Percent Healing hook");
            if (c.TryGotoPrev(x => x.MatchStloc(out cmp)) && cmp != count)
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, count);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(flatHealing, flatHealingStack, stack, flatHealingIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Medkit Flat Healing hook");
        }
    }
}