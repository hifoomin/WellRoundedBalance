using R2API.Utils;

namespace WellRoundedBalance.Artifacts.Vanilla
{
    internal class Spite : ArtifactEditBase
    {
        public override string Name => ":: Artifacts :::::::::::::: Spite";

        [ConfigField("Explosion Radius", "", 7.5f)]
        public static float explosionRadius;

        [ConfigField("Damage", "Decimal.", 1.2f)]
        public static float damage;

        [ConfigField("Maximum Fuse Time", "", 4.5f)]
        public static float maximumFuseTime;

        [ConfigField("Base Spawn Radius", "", 7f)]
        public static float baseSpawnRadius;

        [ConfigField("Spawn Radius Coefficient", "", 7f)]
        public static float spawnRadiusCoefficient;

        [ConfigField("Extra Bombs Per Radius", "", 16f)]
        public static float extraBombsPerRadius;

        [ConfigField("Maximum Bomb Count", "", 1000)]
        public static int maximumBombCount;

        [ConfigField("Maximum Fall Distance", "", 500f)]
        public static float maximumFallDistance;

        [ConfigField("Maximum Step Up Distance", "", 100f)]
        public static float maximumStepUpDistance;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.RoR2.SpiteBombController.FixedUpdate += SpiteBombController_FixedUpdate;
            On.RoR2.SpiteBombController.Start += SpiteBombController_Start;
        }

        private void SpiteBombController_Start(On.RoR2.SpiteBombController.orig_Start orig, SpiteBombController self)
        {
            self.initialVelocityY = Random.Range(-40f, 40f);

            var origin = self.transform.position;

            var time = Trajectory.CalculateFlightDuration(origin.y, self.bouncePosition.y, self.initialVelocityY);

            var bounceDistance = self.bouncePosition - origin;
            bounceDistance.y = 0f;
            var magnitude = bounceDistance.magnitude;

            var d = Trajectory.CalculateGroundSpeed(time, magnitude);

            var value = bounceDistance / magnitude * d;
            value.y = self.initialVelocityY;
            self.SetFieldValue("velocity", value);

            // copypasted code so it's awful, fix later pls
        }

        private void SpiteBombController_FixedUpdate(On.RoR2.SpiteBombController.orig_FixedUpdate orig, SpiteBombController self)
        {
            var velocity = self.velocity;
            var fixedDeltaTime = Time.fixedDeltaTime;

            velocity.y += fixedDeltaTime * Physics.gravity.y;

            var origin = self.transform.position;
            var maxDistance = velocity.magnitude * fixedDeltaTime + self.radius;

            if (Physics.Raycast(origin, velocity, out RaycastHit raycastHit, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
            {
                origin = raycastHit.point;
                velocity -= 2f * Vector3.Dot(velocity, raycastHit.normal) * raycastHit.normal;
                velocity *= 0.8f;
                try { self.InvokeMethod("OnBounce"); } catch { }
            }
            else
            {
                origin += velocity * fixedDeltaTime;
            }

            self.SetFieldValue("velocity", velocity);
            self.rb.MovePosition(origin);
            self.delayBlast.position = origin;

            // copypasted code so it's awful, fix later pls
        }

        private void Changes()
        {
            RoR2.Artifacts.BombArtifactManager.bombBlastRadius = explosionRadius;
            RoR2.Artifacts.BombArtifactManager.bombDamageCoefficient = damage;
            RoR2.Artifacts.BombArtifactManager.bombFuseTimeout = maximumFuseTime;
            RoR2.Artifacts.BombArtifactManager.bombSpawnBaseRadius = baseSpawnRadius;
            RoR2.Artifacts.BombArtifactManager.bombSpawnRadiusCoefficient = spawnRadiusCoefficient;
            RoR2.Artifacts.BombArtifactManager.extraBombPerRadius = extraBombsPerRadius;
            RoR2.Artifacts.BombArtifactManager.maxBombCount = maximumBombCount;
            RoR2.Artifacts.BombArtifactManager.maxBombFallDistance = maximumFallDistance;
            RoR2.Artifacts.BombArtifactManager.maxBombStepUpDistance = maximumStepUpDistance;
        }

        private void ThemissileknowswhereitisatalltimesItknowsthisbecauseitknowswhereitisntBysubtractingwhereitisfromwhereitisntorwhereitisntfromwhereitiswhicheverisgreateritobtainsadifferenceordeviationTheguidancesubsystemusesdeviationstogeneratecorrectivecommandstodrivethemissilefromapositionwhereitistoapositionwhereitisntandarrivingatapositionwhereitwasntitnowisConsquentlythepositionwhereitisisnowthepositionthatitwasntanditfollowsthatthepositionthatitwasisnowthepositionthatitisntIntheeventthatthepositionthatitisinisnotthepositionthatitwasntthesystemhasacquiredavariation()
        {
        }

        private void ThevariationbeingthedifferencebetweenwherethemissileisandwhereitwasntIfvariationisconsideredtobeasignificantfactorittoomaybecorrectedbytheGEAhoweverthemissilemustalsoknowwhereitwasThemissileguidancecomputerscenarioworksasfollowsBecausethevariationhasmodifiedsomeoftheinformationthemissilehasobtaineditisnotsurejustwhereitishoweveritissurewhereitisntwithinreasonanditknowswhereitwasItnowsubtractswhereitshouldbefromwhereitwasntorviceversaAndbydifferentiatingthisfromthealgebraicsumofwhereitshouldntbeandwhereitwasitisabletoobtainthedeviationanditsvariationwhichiscallederror()
        {
        }
    }
}