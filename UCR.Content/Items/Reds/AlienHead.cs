using R2API;
using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class AlienHead : Based
    {
        public static float cdr;
        public static float flatcdr;
        public static bool flatcdrstack;

        public override string Name => ":: Items ::: Reds :: Alien Head";
        public override string InternalPickupToken => "alienHead";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public static bool ahFCDR = flatcdr != 0f;
        public static bool ahFCDRStack = flatcdrstack;

        public override string DescText => "<style=cIsUtility>Reduce skill cooldowns</style> by <style=cIsUtility>" + d(cdr) + "</style> <style=cStack>(+" + d(cdr) + " per stack)</style>" +
                                           (ahFCDR ? " and <style=cIsUtility>" + flatcdr + "</style> second" + 
                                           (ahFCDRStack ? "<style=cStack>(+" + flatcdr + " per stack)</style>" : ".") : ".");
        public override void Init()
        {
            cdr = ConfigOption(0.25f, "Percent Cooldown Reduction", "Decimal. Per Stack. Vanilla is 0.25");
            flatcdr = ConfigOption(0f, "Flat Cooldown Reduction", "Vanilla is 0");
            flatcdrstack = ConfigOption(false, "Stack Flat Cooldown Reduction?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeCDR;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }
        public static void ChangeCDR(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.75f),
                x => x.MatchMul()
            );
            c.Next.Operand = 1f - cdr;
        }
        // PLEASE HELP TO FIX
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.AlienHead);
                if (stack > 0)
                {
                    args.cooldownReductionAdd += flatcdrstack ? flatcdr * stack : flatcdr;
                }
            }
        }
        // PLEASE HELP TO FIX
    }
}
