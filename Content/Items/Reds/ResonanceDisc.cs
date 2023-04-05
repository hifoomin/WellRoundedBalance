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
        }
    }
}