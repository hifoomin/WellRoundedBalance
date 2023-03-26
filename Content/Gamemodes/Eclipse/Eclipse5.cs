using MonoMod.Cil;

namespace WellRoundedBalance.Gamemodes.Eclipse
{
    internal class Eclipse5 : GamemodeBase<Eclipse5>
    {
        public static float timer;
        public static float previousTime;
        public override string Name => ":: Gamemode : Eclipse";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Stage.onServerStageBegin += Stage_onServerStageBegin;
            On.RoR2.Run.FixedUpdate += Run_FixedUpdate;
            IL.RoR2.HealthComponent.Heal += HealthComponent_Heal;
        }

        private void Run_FixedUpdate(On.RoR2.Run.orig_FixedUpdate orig, Run self)
        {
            orig(self);
            timer += Time.fixedDeltaTime;
            var currentTime = (int)self.time;
            if (currentTime - previousTime > 300 && timer > 300 && self.selectedDifficulty >= DifficultyIndex.Eclipse5)
            {
                for (int i = 0; i < 4 + Run.instance.participatingPlayerCount; i++)
                {
                    var meteorStormController = Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/MeteorStorm"), new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<MeteorStormController>();
                    meteorStormController.owner = null;
                    meteorStormController.ownerDamage = 8f + Mathf.Sqrt(Run.instance.ambientLevel * 100f);
                    meteorStormController.isCrit = false;
                    NetworkServer.Spawn(meteorStormController.gameObject);
                }

                previousTime = 0;
                timer = 0;
            }
        }

        private void Stage_onServerStageBegin(Stage stage)
        {
            previousTime = (int)stage.entryTime.t;
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