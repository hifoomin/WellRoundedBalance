using MonoMod.Cil;

namespace WellRoundedBalance.Gamemodes.Eclipse
{
    internal class Eclipse5 : GamemodeBase<Eclipse5>
    {
        public static float timer;
        public static float previousTime;
        public override string Name => ":: Gamemode : Eclipse 5";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.Heal += HealthComponent_Heal;
            On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
        }

        private void CombatDirector_Awake(On.RoR2.CombatDirector.orig_Awake orig, CombatDirector self)
        {
            if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse5)
            {
                self.creditMultiplier += 0.03f * Run.instance.stageClearCount;
            }
            orig(self);
        }

        private void HealthComponent_Heal(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(2f),
                x => x.MatchDiv()))
            {
                c.Next.Operand = 1f;
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 5 hook");
            }
        }
    }
}