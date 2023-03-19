using MonoMod.Cil;

namespace WellRoundedBalance.Eclipse
{
    internal class Eclipse3 : GamemodeBase<Eclipse3>
    {
        // look at Elites folder
        public override string Name => ":: Gamemode : Eclipse";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterHitGroundServer += GlobalEventManager_OnCharacterHitGroundServer;
        }

        private void GlobalEventManager_OnCharacterHitGroundServer(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(2f)))
            {
                c.Next.Operand = 1f;
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 3 hook");
            }
        }

        internal static bool CheckEclipse() {
            return instance.isEnabled && Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3;
        }
    }
}