namespace UltimateCustomRun.Enemies.Bosses
{
    public class MithrixPhase2 : EnemyBase
    {
        public static bool Skip;
        public override string Name => ":::: Enemies :::: Mithrix Phase 2";

        public override void Init()
        {
            Skip = ConfigOption(false, "Skip phase 2?", "Vanilla is false.\nRecommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            RemoveLmao();
        }

        public static void RemoveLmao()
        {
            On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += (orig, self) =>
            {
                orig(self);
                self.PreEncounterBegin();
                self.outer.SetNextState(new EntityStates.Missions.BrotherEncounter.Phase3());
            };
        }
    }
}