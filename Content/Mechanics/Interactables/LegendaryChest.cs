namespace WellRoundedBalance.Mechanics.Interactables
{
    internal class LegendaryChest : MechanicBase<LegendaryChest>
    {
        public override string Name => ":: Mechanics :::::::::: Legendary Chest For All";

        [ConfigField("Affect only Stage 4?", "", true)]
        public static bool affectOnlyStage4;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.ChestBehavior.ItemDrop += ChestBehavior_ItemDrop;
        }

        private bool IsValidScene(Transform chestTransform)
        {
            if (affectOnlyStage4)
            {
                bool parentIsRootJungle = chestTransform.parent && chestTransform.parent.name == "HOLDER: Newt Statues and Preplaced Chests";
                bool parentIsDampCaveSimple = chestTransform.parent
                    && chestTransform.parent.parent
                    && chestTransform.parent.parent.name == "GROUP: Large Treasure Chests";
                return parentIsDampCaveSimple || parentIsRootJungle;
            }
            return true;
        }

        private void ChestBehavior_ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, ChestBehavior self)
        {
            // do this with IL later, prod lol
            if (self.tier3Chance != 1 || self.dropPickup == PickupIndex.none || self.dropPickup == PickupIndex.none || !IsValidScene(self.transform))
            {
                orig(self);
                return;
            }
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::ItemDrop()' called on client");
                return;
            }

            int participatingPlayerCount = Run.instance.participatingPlayerCount != 0 ? Run.instance.participatingPlayerCount : 1;
            float angle = 360f / participatingPlayerCount;
            var chestVelocity = Vector3.up * self.dropUpVelocityStrength + self.dropTransform.forward * self.dropForwardVelocityStrength;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            int i = 0;
            while (i < participatingPlayerCount)
            {
                PickupDropletController.CreatePickupDroplet(self.dropPickup, self.dropTransform.position + Vector3.up * 1.5f, chestVelocity);
                i++;
                chestVelocity = rotation * chestVelocity;
            }
            self.dropPickup = PickupIndex.none;
        }
    }
}