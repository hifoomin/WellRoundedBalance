using System;

namespace WellRoundedBalance.Items.Whites
{
    public class BolsteringLantern : ItemBase<BolsteringLantern>
    {
        public override string Name => ":: Items : Whites :: Bolstering Lantern";
        public override ItemDef InternalPickup => DLC2Content.Items.AttackSpeedPerNearbyAllyOrEnemy;

        public override string PickupText => "Increases your attack speed for every nearby enemy and ally.";

        public override string DescText => $"Increase your <style=cIsDamage>attack speed</style> by <style=cIsDamage>{d(increase)}</style> <style=cStack>(+{d(increase)} per stack)</style> for up to <style=cIsUtility>3</style>{(enemyStack > 0 ? $" <style=cStack>(+{enemyStack} per stack)</style>" : "")} enemies and allies within <style=cIsUtility>20</style> meters.";

        [ConfigField("Increase", 0.07f)]
        public static float increase;
        [ConfigField("Enemy Stack", 0)]
        public static int enemyStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += LanternChanges;
            On.RoR2.AttackSpeedPerNearbyCollider.UpdateValues += UpdateValues;
        }

        private void UpdateValues(On.RoR2.AttackSpeedPerNearbyCollider.orig_UpdateValues orig, AttackSpeedPerNearbyCollider self, int itemCount, out float diameter)
        {
            self.maxCharacterCount = 3 + ((itemCount - 1) * enemyStack);
	        // self.radiusSizeGrowth = Util.ConvertAmplificationPercentageIntoReductionPercentage(itemCount * 5);
            self.radiusSizeGrowth = 1f;
	        diameter = 40f;
        }

        private void LanternChanges(ILContext il)
        {
            ILCursor c = new(il);
            c.FindLocal(LocalType.ItemCount, "AttackSpeedPerNearbyAllyOrEnemy", out int lantern, "DLC2Content");
            c.StepLocal(lantern);
            c.TryGotoNext(x => x.MatchLdcR4(0.035f));
            c.Next.Operand = increase;
            c.TryGotoNext(x => x.MatchLdcR4(0.065f));
            c.Next.Operand = 0f;
        }
    }
}