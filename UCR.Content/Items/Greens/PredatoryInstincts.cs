using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun
{
    public class PredatoryInstincts : Based
    {
        // TODO: FIX ALL THE CODE AAAAAAAAA WHY DOESNT IT WORK RANDOMLY
        public static float aspd;
        public static int basecap;
        public static int stackcap;
        public static float crit;
        public static bool critstack;
        public static float speed;
        public static bool speedstack;

        public override string Name => ":: Items :: Greens :: Predatory Instincts";
        public override string InternalPickupToken => "attackSpeedOnCrit";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "<style=cIsDamage>Critical strikes</style> increase <style=cIsDamage>attack speed</style> by <style=cIsDamage>12%</style>. Maximum cap of <style=cIsDamage>36% <style=cStack>(+24% per stack)</style> attack speed</style>.";


        public override void Init()
        {
            /*
            aspd = ConfigOption(0.12f, "Buff Attack Speed", "Decimal. Per Buff. Vanilla is 0.12");
            basecap = ConfigOption(1, "Base Buff Cap", "V. Vanilla is 1");
            stackcap = ConfigOption(2, "Stack Buff Cap", "V. Per Stack. Vanilla is 2");
            crit = ConfigOption(5f, "Crit Chance", "Vanilla is 5");
            critstack = ConfigOption(false, "Stack Crit Chance?", "Vanilla is false");
            speed = ConfigOption(0f, "Buff Speed", "Decimal. Per Buff. Vanilla is 0");
            speedstack = ConfigOption(false, "Stack Buff Speed?", "Vanilla is false");
            */
            base.Init();
        }

        public override void Hooks()
        {
            /*
            IL.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += PredatoryInstincts.ChangeCap;
            IL.RoR2.CharacterBody.RecalculateStats += PredatoryInstincts.ChangeAS;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
            */
        }

        public static void ChangeAS(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("GetBuffCount"),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.12f)
            );
            c.Index += 2;
            c.Next.Operand = aspd;
        }

        public static void ChangeCap(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(1),
                x => x.MatchLdcI4(1),
                x => x.MatchLdloc(1),
                x => x.MatchLdcI4(2)
            );
            c.Index += 1;
            c.Next.Operand = basecap;
            c.Index += 2;
            c.Next.Operand = stackcap;
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.AttackSpeedOnCrit);
                int buff = sender.GetBuffCount(RoR2Content.Buffs.AttackSpeedOnCrit);
                if (stack > 0)
                {
                    args.critAdd += critstack ? crit * stack : crit;

                    if (buff > 0)
                    {
                        args.moveSpeedMultAdd += speedstack ? speed * buff * stack : speed * buff;
                    }
                }
            }
        }
        // NONE OF THESE WORK PLEASE HELP TO FIX
    }
}
