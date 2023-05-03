namespace WellRoundedBalance.Enemies.Standard
{
    internal class Lemurian : EnemyBase<Lemurian>
    {
        public override string Name => ":: Enemies :: Lemurian";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.LemurianMonster.Bite.OnEnter += Bite_OnEnter;
            On.EntityStates.LemurianMonster.Bite.FixedUpdate += Bite_FixedUpdate;
            On.EntityStates.LemurianMonster.FireFireball.OnEnter += FireFireball_OnEnter;
        }

        private void FireFireball_OnEnter(On.EntityStates.LemurianMonster.FireFireball.orig_OnEnter orig, EntityStates.LemurianMonster.FireFireball self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.LemurianMonster.FireFireball.damageCoefficient = 1.3f;
            orig(self);
        }

        private void Bite_FixedUpdate(On.EntityStates.LemurianMonster.Bite.orig_FixedUpdate orig, EntityStates.LemurianMonster.Bite self)
        {
            if (!Main.IsInfernoDef() && self.isAuthority)
            {
                if (self.modelAnimator && self.modelAnimator.GetFloat("Bite.hitBoxActive") > 0.1f)
                {
                    Vector3 direction = self.GetAimRay().direction;
                    Vector3 a = direction.normalized * 1.5f * self.moveSpeedStat;
                    Vector3 b = new Vector3(direction.x, 0f, direction.z).normalized * 1.5f;
                    self.characterMotor.Motor.ForceUnground();
                    self.characterMotor.velocity = a + b;
                }
                if (self.fixedAge > 0.5f)
                {
                    self.attack.Fire(null);
                }
            }
            orig(self);
        }

        private void Bite_OnEnter(On.EntityStates.LemurianMonster.Bite.orig_OnEnter orig, EntityStates.LemurianMonster.Bite self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.LemurianMonster.Bite.radius = 3f;
                EntityStates.LemurianMonster.Bite.baseDuration = 0.8f;
                EntityStates.LemurianMonster.Bite.forceMagnitude = 400f;
                EntityStates.LemurianMonster.Bite.damageCoefficient = 2.1f;
            }

            orig(self);
        }

        private void Changes()
        {
            var fireball = Utils.Paths.GameObject.Fireball.Load<GameObject>();
            var projectileSimple = fireball.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = 7f;
            projectileSimple.desiredForwardSpeed = 65f;
        }
    }
}