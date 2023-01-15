namespace WellRoundedBalance.Mechanic.Director
{
    internal class CombatDirector : MechanicBase
    {
        public override string Name => ":: Mechanics ::::: Combat Director";

        public override void Hooks()
        {
            On.RoR2.CombatDirector.OnEnable += CombatDirector_OnEnable;
            On.RoR2.CombatDirector.OnDisable += CombatDirector_OnDisable;
        }

        private void CombatDirector_OnDisable(On.RoR2.CombatDirector.orig_OnDisable orig, RoR2.CombatDirector self)
        {
            self.minRerollSpawnInterval *= 1.35f;
            self.maxRerollSpawnInterval *= 1.35f;
            orig(self);
        }

        private void CombatDirector_OnEnable(On.RoR2.CombatDirector.orig_OnEnable orig, RoR2.CombatDirector self)
        {
            self.minRerollSpawnInterval /= 1.35f;
            self.maxRerollSpawnInterval /= 1.35f;
            orig(self);
        }
    }
}