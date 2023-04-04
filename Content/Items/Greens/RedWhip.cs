using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class RedWhip : ItemBase<RedWhip>
    {
        public override string Name => ":: Items :: Greens :: Red Whip";
        public override ItemDef InternalPickup => RoR2Content.Items.SprintOutOfCombat;

        public override string PickupText => "Move fast out of combat.";
        public override string DescText => "Leaving combat boosts your <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(movementSpeedGain) + "</style> <style=cStack>(+" + d(movementSpeedGain) + " per stack)</style>.";

        [ConfigField("Movement Speed Gain", "Decimal.", 0.35f)]
        public static float movementSpeedGain;

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
                c.Next.Operand = movementSpeedGain;
            }
            else
            {
                Logger.LogError("Failed to apply Red Whip Speed hook");
            }
        }
    }
}