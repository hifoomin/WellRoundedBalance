using MonoMod.Cil;

namespace WellRoundedBalance.Gamemodes.Eclipse
{
    internal class Eclipse1 : GamemodeBase<Eclipse1>
    {
        public override string Name => ":: Gamemode : Eclipse 1";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
            On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
        }

        private void CombatDirector_Awake(On.RoR2.CombatDirector.orig_Awake orig, CombatDirector self)
        {
            if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse1)
            {
                self.creditMultiplier += 0.03f * Run.instance.stageClearCount;
            }
            orig(self);
        }

        private void CharacterMaster_OnBodyStart(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.5f)))
            {
                c.Next.Operand = 1f;
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 1 hook");
            }
        }
    }
}