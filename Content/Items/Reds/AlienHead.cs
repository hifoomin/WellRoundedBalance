using MonoMod.Cil;
using R2API;
using RoR2;

namespace WellRoundedBalance.Items.Reds
{
    public class AlienHead : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Alien Head";
        public override string InternalPickupToken => "alienHead";

        public override string PickupText => "Reduces cooldowns for your skills.";

        public override string DescText => "<style=cIsUtility>Reduce skill cooldowns</style> by <style=cIsUtility>0.5s</style> and <style=cIsUtility>30%</style> <style=cStack>(+30% per stack)</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeCDR;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void ChangeCDR(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.75f),
                    x => x.MatchMul()))
            {
                c.Next.Operand = 0.7f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Alien Head Cooldown Reduction hook");
            }
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.AlienHead);
                if (stack > 0)
                {
                    args.cooldownReductionAdd += 0.5f;
                }
            }
        }
    }
}