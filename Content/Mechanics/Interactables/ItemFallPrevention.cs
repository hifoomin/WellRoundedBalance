namespace WellRoundedBalance.Mechanics.Interactables
{
    internal class ItemFallPrevention : MechanicBase
    {
        public override string Name => ":: Mechanics :::::::::::: Item Fall Prevention";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.MapZone.Awake += MapZone_Awake;
            On.RoR2.MapZone.TryZoneStart += MapZone_TryZoneStart;
            // do this better later
        }

        private void MapZone_TryZoneStart(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            if (self.zoneType == MapZone.ZoneType.OutOfBounds)
            {
                if (other.GetComponent<PickupDropletController>() || other.GetComponent<GenericPickupController>())
                {
                    SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
                    spawnCard.hullSize = HullClassification.Human;
                    spawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
                    spawnCard.prefab = LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab");
                    GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                        position = other.transform.position
                    }, RoR2Application.rng));
                    if (gameObject)
                    {
                        TeleportHelper.TeleportGameObject(other.gameObject, gameObject.transform.position);
                        Object.Destroy(gameObject);
                    }
                    Object.Destroy(spawnCard);
                }
            }
            orig(self, other);
        }

        private void MapZone_Awake(On.RoR2.MapZone.orig_Awake orig, MapZone self)
        {
            orig(self);
            if (self.zoneType == MapZone.ZoneType.OutOfBounds)
            {
                self.gameObject.layer = 29;
            }
        }
    }
}