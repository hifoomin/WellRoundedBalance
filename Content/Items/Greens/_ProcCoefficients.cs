// placed here so it initializes first just in case of any problems

using System;

namespace WellRoundedBalance.Items.Greens
{
    public class _ProcCoefficients : ItemBase<_ProcCoefficients>
    {
        public override string Name => ":: Items Changes :: Proc Coefficients";
        public override ItemDef InternalPickup => null;

        public override string PickupText => string.Empty;
        public override string DescText => string.Empty;

        public override bool isEnabled => true;

        [ConfigField("Global Proc Chance", "This is a multiplier to all items that can proc something themselves, not necessarily items like Ignition Tank for example. There are special cases where this doesn't work - Sticky Bomb (as it has a Proc Coefficient of 0 by default), Shuriken (most people like it proccing even factoring Railgunner), Kjaro's Band, Runald's Band, Singularity Band, Visions of Heresy and Hooks of Heresy. This also does not include equipment.", 0.2f)]
        public static float globalProc;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
        }
    }
}