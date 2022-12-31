using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace WellRoundedBalance.Items.Whites
{
    public class OddlyShapedOpal : ItemBase
    {
        public override string Name => ":: Items : Whites :: Oddly Shaped Opal";
        public override string InternalPickupToken => "outOfCombatArmor";

        public override string PickupText => "Reduce damage the first time you are hit.";

        public override string DescText => "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>30</style> <style=cStack>(+30 per stack)</style> while out of danger.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeArmor;
        }

        public static void ChangeArmor(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdsfld("RoR2.DLC1Content/Buffs", "OutOfCombatArmorBuff"),
                    x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff"),
                    x => x.MatchBrtrue(out _),
                    x => x.MatchLdcR4(0.0f),
                    x => x.MatchBr(out _),
                    x => x.MatchLdcR4(100f)))
            {
                c.Index += 5;
                c.Next.Operand = 30f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Oddly-shaped Opal Armor hook");
            }
        }
    }
}