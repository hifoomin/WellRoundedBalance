using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace WellRoundedBalance.Items.Whites
{
    public class BisonSteak : ItemBase
    {
        public override string Name => ":: Items : Whites :: Bison Steak";
        public override string InternalPickupToken => "flatHealth";

        public override string PickupText => "Gain 45 max health.";

        public override string DescText => "Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>45</style> <style=cStack>(+45 per stack)</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeHealth;
        }

        public static void ChangeHealth(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(25f)))
            {
                c.Index += 1;
                c.Next.Operand = 45f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Bison Steak Health hook");
            }
        }
    }
}