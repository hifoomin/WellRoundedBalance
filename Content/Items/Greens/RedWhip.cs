using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class RedWhip : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Red Whip";
        public override string InternalPickupToken => "sprintOutOfCombat";

        public override string PickupText => "Move fast out of combat.";
        public override string DescText => "Leaving combat boosts your <style=cIsUtility>movement speed</style> by <style=cIsUtility>35%</style> <style=cStack>(+35% per stack)</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeSpeed;
        }

        private void ChangeSpeed(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff"),
                    x => x.MatchBrfalse(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.3f)))
            {
                c.Index += 5;
                c.Next.Operand = 0.35f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Red Whip Speed hook");
            }
        }
    }
}