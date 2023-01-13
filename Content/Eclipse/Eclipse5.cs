using MonoMod.Cil;

namespace WellRoundedBalance.Eclipse
{
    internal class Eclipse5 : GamemodeBase
    {
        public override string Name => ":: Gamemode : Eclipse";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterMaster.OnServerStageBegin += MeteorStormBehavior.CharacterMaster_OnServerStageBegin;
            On.RoR2.Run.FixedUpdate += MeteorStormBehavior.Run_FixedUpdate;
            IL.RoR2.HealthComponent.Heal += HealthComponent_Heal;
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
                Main.WRBLogger.LogError("Failed to apply Eclipse 5 hook");
            }
        }
    }

    public class MeteorStormBehavior : MonoBehaviour
    {
        public static int previousTime;

        public static void Run_FixedUpdate(On.RoR2.Run.orig_FixedUpdate orig, Run self)
        {
            orig(self);
            var currentTime = (int)self.time;
            if (currentTime - previousTime > 300 && self.selectedDifficulty >= DifficultyIndex.Eclipse5)
            {
                var playerList = CharacterBody.readOnlyInstancesList.Where(x => x.isPlayerControlled).ToArray();
                for (int i = 0; i < playerList.Length; i++)
                {
                    var meteorStormController = Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/MeteorStorm"), playerList[i].corePosition, Quaternion.identity).GetComponent<MeteorStormController>();
                    meteorStormController.owner = null;
                    meteorStormController.ownerDamage = 8f + Mathf.Sqrt(Run.instance.ambientLevel * 200f);
                    meteorStormController.isCrit = false;
                    NetworkServer.Spawn(meteorStormController.gameObject);
                    previousTime = 0;
                }
            }
        }

        public static void CharacterMaster_OnServerStageBegin(On.RoR2.CharacterMaster.orig_OnServerStageBegin orig, CharacterMaster self, Stage stage)
        {
            previousTime = (int)stage.entryTime.t;
            orig(self, stage);
        }
    }
}