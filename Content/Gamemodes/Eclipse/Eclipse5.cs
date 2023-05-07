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
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster master)
        {
            if (!Run.instance)
            {
                return;
            }
            if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse5 && master.teamIndex == TeamIndex.Monster)
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
                }
            }
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