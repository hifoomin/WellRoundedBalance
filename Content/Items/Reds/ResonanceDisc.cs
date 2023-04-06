namespace WellRoundedBalance.Items.Reds
{
    public class ResonanceDisc : ItemBase<ResonanceDisc>
    {
        public override string Name => ":: Items ::: Reds :: Resonance Disc";
        public override ItemDef InternalPickup => RoR2Content.Items.LaserTurbine;

        public override string PickupText => "Obtain a Resonance Disc charged by killing enemies. Fires automatically when fully charged.";

        public override string DescText => "Killing <style=cIsDamage>4</style> enemies in <style=cIsUtility>7 seconds</style> charges the Resonance Disc. The disc launches itself toward a target for <style=cIsDamage>300%</style> base damage <style=cStack>(+300% per stack)</style>, piercing all enemies it doesn't kill, and then explodes for <style=cIsDamage>1000%</style> base damage <style=cStack>(+1000% per stack)</style>. Returns to the user, striking all enemies along the way for <style=cIsDamage>300%</style> base damage <style=cStack>(+300% per stack)</style>.";

        [ConfigField("Beam Proc Coefficient", 1f)]
        public static float beamProcCoefficient;

        [ConfigField("Explosion Proc Coefficient", 1f)]
        public static float explosionProcCoefficient;

        [ConfigField("Explosion Radius", 16f)]
        public static float explosionRadius;

        [ConfigField("Beam Radius", 3f)]
        public static float beamRadius;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.LaserTurbine.FireMainBeamState.OnEnter += FireMainBeamState_OnEnter;
            Changes();
        }

        private void FireMainBeamState_OnEnter(On.EntityStates.LaserTurbine.FireMainBeamState.orig_OnEnter orig, EntityStates.LaserTurbine.FireMainBeamState self)
        {
            EntityStates.LaserTurbine.FireMainBeamState.mainBeamProcCoefficient = beamProcCoefficient * globalProc;
            EntityStates.LaserTurbine.FireMainBeamState.mainBeamRadius = beamRadius;
            orig(self);
        }

        private void Changes()
        {
            var bomb = Utils.Paths.GameObject.LaserTurbineBomb.Load<GameObject>();
            var projectileImpactExplosion = bomb.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            projectileImpactExplosion.blastRadius = explosionRadius;
            projectileImpactExplosion.blastProcCoefficient = explosionProcCoefficient * globalProc;

            var explosion = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.LaserTurbineBombExplosion.Load<GameObject>(), "Resonance Disc Explosion", false);
            var transform = explosion.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                var index = transform.GetChild(i);
                index.transform.localScale *= 2f;
            }

            var sparklers = transform.GetChild(3);
            for (int j = 0; j < sparklers.transform.childCount; j++)
            {
                var index = sparklers.transform.GetChild(j);
                index.transform.localScale *= 2f;
            }

            var evis = transform.GetChild(5);
            for (int k = 0; k < evis.transform.childCount; k++)
            {
                var index = evis.transform.GetChild(k);
                index.transform.localScale *= 2f;
            }

            ContentAddition.AddEffect(explosion);

            projectileImpactExplosion.impactEffect = explosion;
        }
    }
}