using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Reds
{
    public class LaserScope : ItemBase
    {
        public static float Damage;
        public static float Crit;
        public static bool StackCrit;
        public override string Name => ":: Items ::: Reds :: Laser Scope";
        public override string InternalPickupToken => "critDamage";
        public override bool NewPickup => true;
        public override string PickupText => "Your 'Critical Strikes' deal an additional " + d(Damage) + " damage.";

        public override string DescText => (Crit > 0 ? (StackCrit ? "" : "") : "") +
                                           "<style=cIsDamage>Critical Strikes</style> deal an additional <style=cIsDamage>" + d(Damage) + " damage</style><style=cStack>(+" + d(Damage) + " per stack)</style>.";

        public override void Init()
        {
            Damage = ConfigOption(1f, "Crit Damage Increase", "Decimal. Per Stack. Vanilla is 1");
            Crit = ConfigOption(0f, "Crit Chance", "Vanilla is 0");
            StackCrit = ConfigOption(false, "Stack Crit Chance?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeDamage;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(2f),
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4()))
            {
                c.Index += 1;
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Laser Scope Damage hook");
            }
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(DLC1Content.Items.CritDamage);
                if (stack > 0)
                {
                    args.critAdd += StackCrit ? Crit * stack : Crit;
                }
            }
        }
    }
}