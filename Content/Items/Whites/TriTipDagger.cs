using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace WellRoundedBalance.Items.Whites
{
    public class TriTipDagger : ItemBase
    {
        public static float Chance;

        public override string Name => ":: Items : Whites :: Tri Tip Dagger";
        public override string InternalPickupToken => "bleedOnHit";

        public override string PickupText => "Gain +9% chance to bleed enemies on hit.";

        public override string DescText => "<style=cIsDamage>9%</style> <style=cStack>(+9% per stack)</style> Chance to <style=cIsDamage>bleed</style> an enemy for <style=cIsDamage>240%</style> base damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeChance;
        }

        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdcR4(10f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4()))
            {
                c.Index += 1;
                c.Next.Operand = 9f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Tri-tip Dagger Chance hook");
            }
        }
    }
}