/*
using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Reds
{
    public class BottledChaos : ItemBase
    {
        public static int Uses;

        public override string Name => ":: Items ::: Reds :: Bottled Chaos";
        public override string InternalPickupToken => "randomEquipmentTrigger";
        public override bool NewPickup => true;

        public override string PickupText => "Activating your Equipment triggers" +
                                             (Uses > 1 ? Uses : " an") +
                                             " additional, random Equipment effect" +
                                             (Uses > 1 ? "s" : "") +
                                             ".";

        public override string DescText => "Trigger a <style=cIsDamage>random equipment</style> effect <style=cIsDamage>1</style> <style=cStack>(+1 per stack)</style> time(s).";

        public override void Init()
        {
            Uses = ConfigOption(1, "Random Equipment Uses", "Per Stack. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
        }
    }
}
*/