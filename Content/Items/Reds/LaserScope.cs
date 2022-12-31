using MonoMod.Cil;
using R2API;
using RoR2;

namespace WellRoundedBalance.Items.Reds
{
    public class LaserScope : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Laser Scope";
        public override string InternalPickupToken => "critDamage";

        public override string PickupText => "Your 'Critical Strikes' deal an additional 50% damage.";

        public override string DescText => "Gain <style=cIsDamage>20% critical chance</style>. <style=cIsDamage>Critical Strikes</style> deal an additional <style=cIsDamage>50% damage</style> <style=cStack>(+50% per stack)</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(DLC1Content.Items.CritDamage);
                if (stack > 0)
                {
                    args.critAdd += 20f;
                }
            }
        }
    }
}