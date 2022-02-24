using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Whites
{
    public class BackupMag : ItemBase
    {
        public static float CooldownReduction;

        public override string Name => ":: Items : Whites :: Backup Mag";
        public override string InternalPickupToken => "secondarySkillMagazine";
        public override bool NewPickup => true;

        public override string PickupText => "Add an extra charge of your Secondary skill" +
                                             (CooldownReduction != 0f ? " and reduce its Cooldown." : ".");

        public override string DescText => "Add <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> charge of your <style=cIsUtility>Secondary skill</style>." +
                                           (CooldownReduction != 0f ? " Reduce your Secondary skill Cooldown by <style=cIsUtility>" + d(CooldownReduction) + "</style> <style=cStack>(+" + d(CooldownReduction) + " per stack)</style>." : "");

        public override void Init()
        {
            CooldownReduction = ConfigOption(0f, "Secondary Skill Cooldown Reduction", "Decimal. Per Stack. Vanilla is 0");
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
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine);
                if (stack > 0)
                {
                    args.secondaryCooldownMultAdd -= CooldownReduction * stack;
                }
            }
        }
    }
}