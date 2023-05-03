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
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster master)
        {
            if (!Run.instance)
            {
                return;
            }
            if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse1 && master.teamIndex == TeamIndex.Monster)
            {
                var baseAI = master.GetComponent<BaseAI>();
                if (baseAI != null)
                {
                    baseAI.fullVision = true;
                    baseAI.aimVectorMaxSpeed = 250f;
                    if (master.name != "GolemMaster(Clone)" || master.name != "MegaConstructMaster(Clone)" || master.name != "ClayBruiserMaster(Clone)")
                    {
                        baseAI.aimVectorDampTime = 0.031f;
                    }
                    else
                    {
                        baseAI.aimVectorDampTime = 0.09f;
                    }
                }
            }
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