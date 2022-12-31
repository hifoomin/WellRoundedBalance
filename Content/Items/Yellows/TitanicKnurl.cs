using MonoMod.Cil;
using UnityEngine.UIElements;

namespace WellRoundedBalance.Items.Yellows
{
    public class TitanicKnurl : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Titanic Knurl";
        public override string InternalPickupToken => "knurl";

        public override string PickupText => "Boosts health and regeneration.";

        public override string DescText => "<style=cIsHealing>Increase maximum health</style> by <style=cIsHealing>100</style> <style=cStack>(+100 per stack)</style> and <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+2.4 hp/s</style> <style=cStack>(+2.4 hp/s per stack</style>)";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(40f),
                    x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = 100f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Titanic Knurl Health hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(1.6f),
                    x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = 2.4f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Titanic Knurl Regen hook");
            }
        }
    }
}