﻿namespace WellRoundedBalance.Interactables
{
    internal class VoidSeed : InteractableBase<VoidSeed>
    {
        public override string Name => ":: Interactables :::: Void Seed";

        [ConfigField("Max Spawns Per Stage", "", 1)]
        public static int maxSpawnsPerStage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var voidSeed = Utils.Paths.InteractableSpawnCard.iscVoidCamp.Load<InteractableSpawnCard>();
            voidSeed.maxSpawnsPerStage = maxSpawnsPerStage;

            var interactables = Utils.Paths.DirectorCardCategorySelection.dccsVoidCampInteractables.Load<DirectorCardCategorySelection>();
            interactables.categories[0].cards[1].selectionWeight = 2;
        }
    }
}