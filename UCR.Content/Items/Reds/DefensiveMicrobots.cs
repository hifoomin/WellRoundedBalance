namespace UltimateCustomRun
{
    public static class DefensiveMicrobots
    {
        public static void Changes()
        {
            On.EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.OnEnter += (orig, self) =>
            {
                EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.baseRechargeFrequency = Main.DefeMicroRechargeFreq.Value;
                EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.minimumFireFrequency = Main.DefeMicroMinFireFreq.Value;
                EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.projectileEraserRadius = Main.DefeMicroRange.Value;
                orig(self);
            };
        }
    }
}
