using MonoMod.Cil;
using RoR2;

namespace WellRoundedBalance.Items.Yellows
{
    public class EmpathyCores : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Empathy Cores";
        public override string InternalPickupToken => "roboBallBuddy";

        public override string PickupText => "Recruit a pair of Solus Probes that gain power with more allies.";

        public override string DescText => "Every 30 seconds, <style=cIsUtility>summon two Solus Probes</style> that gain <style=cIsDamage>+70%</style> <style=cStack>(+70% per stack)</style> damage per <style=cIsUtility>ally on your team</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeDamage;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchStloc(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchMul(),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(1f)))
            {
                c.Index += 6;
                c.Next.Operand = 0.7f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Empathy Cores Damage hook");
            }
        }
    }
}