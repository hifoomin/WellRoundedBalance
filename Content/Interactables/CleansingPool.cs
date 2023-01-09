using UnityEngine;

namespace WellRoundedBalance.Interactables
{
    internal class CleansingPool : InteractableBase
    {
        public override string Name => ":: Interactables :::::::::: Cleansing Pool";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var pool1 = Utils.Paths.InteractableSpawnCard.iscShrineCleanse.Load<InteractableSpawnCard>();
            pool1.maxSpawnsPerStage = 1;

            var pool2 = Utils.Paths.InteractableSpawnCard.iscShrineCleanseSandy.Load<InteractableSpawnCard>();
            pool2.maxSpawnsPerStage = 1;

            var pool3 = Utils.Paths.InteractableSpawnCard.iscShrineCleanseSnowy.Load<InteractableSpawnCard>();
            pool3.maxSpawnsPerStage = 1;
        }
    }
}