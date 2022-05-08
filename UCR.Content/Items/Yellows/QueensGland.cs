using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun.Items.Yellows
{
    public class QueensGland : ItemBase
    {
        public static int Limit;
        public static int StackLimit;

        public override string Name => ":: Items :::: Yellows :: Queens Gland";
        public override string InternalPickupToken => "beetleGland";
        public override bool NewPickup => true;

        public override string PickupText => "Recruit " + Limit + " Beetle Guard" +
                                             (Limit > 1 ? "s" : "") +
                                             ".";

        public override string DescText => "<style=cIsUtility>Summon a Beetle Guard</style> with bonus <style=cIsDamage>300%</style> damage and <style=cIsHealing>100% health</style>. Can have up to <style=cIsUtility>" + Limit + "</style> <style=cStack>(+" + StackLimit + " per stack)</style> Guards at a time.";

        public override void Init()
        {
            Limit = ConfigOption(1, "Ally Limit", "Vanilla is 1");
            StackLimit = ConfigOption(1, "Stack Ally Limit", "Per Stack. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += ChangeLimit;
        }

        public static int ChangeLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.BeetleGuardAlly)
            {
                return Limit + StackLimit * (self.inventory.GetItemCount(RoR2Content.Items.BeetleGland) - 1);
            }
            else
            {
                return orig(self, slot);
            }
        }
    }
}