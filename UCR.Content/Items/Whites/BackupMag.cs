using R2API;
using RoR2;

namespace UltimateCustomRun
{
    public class BackupMag : ItemBase
    {
        public static float cdr;

        public override string Name => ":: Items : Whites :: Backup Mag";
        public override string InternalPickupToken => "secondarySkillMagazine";
        public override bool NewPickup => true;

        bool bCDR = cdr != 0f;

        public override string PickupText => "Add an extra charge of your Secondary skill" + 
                                             (bCDR ? " and reduce its Cooldown." : ".");
        public override string DescText => "Add <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> charge of your <style=cIsUtility>Secondary skill</style>." +
                                           (bCDR ? " Reduce your Secondary skill Cooldown by <style=cIsUtility>" + d(cdr) + "</style> <style=cStack>(+" + d(cdr) + " per stack)</style>." : "");


        public override void Init()
        {
            cdr = ConfigOption(0f, "Secondary Skill Cooldown Reduction", "Decimal. Per Stack. Vanilla is 0");
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
                    args.secondaryCooldownMultAdd -= cdr * stack;
                }
            }
        }
    }
}
