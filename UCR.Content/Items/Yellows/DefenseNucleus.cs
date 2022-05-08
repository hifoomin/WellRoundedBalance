using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun.Items.Yellows
{
    public class DefenseNucleus : ItemBase
    {
        public static int Limit;
        public static int StackLimit;

        public override string Name => ":: Items :::: Yellows :: Defense Nucleus";
        public override string InternalPickupToken => "minorConstructOnKill";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Killing elite monsters spawns an <style=cIsDamage>Alpha Construct</style>. Limited to <style=cIsUtility>" + Limit + "</style> <style=cStack>(+" + StackLimit + " per stack)</style>.";

        public override void Init()
        {
            Limit = ConfigOption(4, "Ally Limit", "Vanilla is 4");
            StackLimit = ConfigOption(4, "Stack Ally Limit", "Per Stack. Vanilla is 4");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += ChangeLimit;
        }

        public static int ChangeLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.MinorConstructOnKill)
            {
                return Limit + StackLimit * (self.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill) - 1);
            }
            else
            {
                return orig(self, slot);
            }
        }
    }
}