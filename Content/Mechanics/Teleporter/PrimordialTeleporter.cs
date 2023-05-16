using Rewired.Utils.Classes.Data;
using System;
using UnityEngine.SceneManagement;

namespace WellRoundedBalance.Mechanics.VoidFields
{
    internal class PrimordialTeleporter : MechanicBase<PrimordialTeleporter>
    {
        public override string Name => ":: Mechanics ::::::::::::::::: Primordial Teleporter";

        [ConfigField("Enable being always interactable?", "", true)]
        public static bool alwaysInteractable;

        [ConfigField("Enable spawning every stage when looping?", "", true)]
        public static bool everyStageWhenLoop;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            if (alwaysInteractable)
            {
                IL.EntityStates.LunarTeleporter.LunarTeleporterBaseState.FixedUpdate += LunarTeleporterBaseState_FixedUpdate;
            }
            if (everyStageWhenLoop)
            {
                SceneDirector.onPrePopulateSceneServer += SceneDirector_onPrePopulateSceneServer;
            }
        }

        public static SpawnCard tp = Utils.Paths.InteractableSpawnCard.iscLunarTeleporter.Load<InteractableSpawnCard>();

        private void SceneDirector_onPrePopulateSceneServer(SceneDirector sd)
        {
            bool isValid = false;
            if (Stage.instance && Stage.instance.sceneDef)
                isValid = !Stage.instance.sceneDef.isFinalStage && Stage.instance.sceneDef.sceneType == SceneType.Stage;
            if (Run.instance && Run.instance.loopClearCount > 0 && isValid)
            {
                sd.teleporterSpawnCard = tp;
            }
        }

        private void LunarTeleporterBaseState_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(0)))
            {
                c.Index++;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return 2;
                });
            }
        }
    }
}