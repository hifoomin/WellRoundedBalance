namespace WellRoundedBalance.Equipment.Orange
{
    public class TrophyHuntersTricorn : EquipmentBase<TrophyHuntersTricorn>
    {
        public override string PickupText => ExecuteThreshold < 100 ? "<style=cIsUtility>Execute</style> a boss at <style=cDeath>low health</style> and <style=cIsUtility>claim its trophy</style>" : "<style=cIsUtility>Execute</style> a boss and <style=cIsUtility>claim its trophy</style>";

        public override string DescText => $"<style=cIsUtility>Execute</style> a boss at or below <style=cDeath>30%</style> health and <style=cIsUtility>obtain its boss item</style>. Consumed on use.";

        public override string Name => ":: Equipment :: Trophy Hunters Tricorn";
        public override EquipmentDef InternalPickup => DLC1Content.Equipment.BossHunter;

        [ConfigField("Execute Threshold", "The health threshold (in %) before a boss can be executed", 30f)]
        public static float ExecuteThreshold;

        private static float GetActualThreshold => ExecuteThreshold * 0.01f;

        public override void Hooks()
        {
            On.RoR2.EquipmentSlot.UpdateTargets += HandleExecuteThreshold;
        }

        public void HandleExecuteThreshold(On.RoR2.EquipmentSlot.orig_UpdateTargets orig, EquipmentSlot self, EquipmentIndex index, bool anticipate)
        {
            orig(self, index, anticipate);
            if (index == DLC1Content.Equipment.BossHunter.equipmentIndex)
            {
                if (!self.currentTarget.hurtBox)
                {
                    return;
                }

                HealthComponent hc = self.currentTarget.hurtBox.healthComponent;
                if (hc.health >= (hc.fullCombinedHealth * GetActualThreshold))
                {
                    HurtBox box = null; // prevents ambiguous call error
                    self.currentTarget = new(box);
                    self.targetIndicator.active = false;
                    self.targetIndicator.targetTransform = null;
                }
            }
        }
    }
}