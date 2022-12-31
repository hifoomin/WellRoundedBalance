using MonoMod.Cil;
using R2API;
using RoR2;

namespace WellRoundedBalance.Items.Greens
{
    public class HuntersHarpoon : ItemBase
    {
        public override string DescText => "Killing an enemy increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>125%</style>, fading over <style=cIsUtility>2</style> <style=cStack>(+1.25 per stack)</style> seconds.";
        public override string InternalPickupToken => "moveSpeedOnKill";
        public override string Name => ":: Items :: Greens :: Hunters Harpoon";

        public override string PickupText => "Killing an enemy gives you a burst of movement speed.";

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeDuration;
        }

        public override void Init()
        {
            base.Init();
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.5f)))
            {
                c.Next.Operand = 2f;
                c.Index += 3;
                c.Next.Operand = 1.25f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Hunter's Harpoon Duration hook");
            }
        }
    }
}