using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Projectile;

namespace UltimateCustomRun.Enemies
{
    public class Lemurian : EnemyBase
    {
        public static bool tw;
        public static float fbdur;
        public static float fbspd;
        public static bool cbhs;
        public static CharacterMaster master;
        public static CharacterBody body;
        public override string Name => ":::: Enemies :: Lemurian";

        public override void Init()
        {
            tw = ConfigOption(false, "Enable Bite Lunge, Speed Tweaks and AI Tweaks?", "Vanilla is false. Recommended Value: true");
            cbhs = ConfigOption(false, "Can be Hit Stunned?", "Vanilla is true. Recommended Value: false");
            fbdur = ConfigOption(0.6f, "Fireball Charge Duration", "Vanilla is 0.6. Recommended Value: 0.4");
            fbspd = ConfigOption(60f, "Fireball Projecitle Speed", "Vanilla is 60. Recommended Value: 75");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }
        public static void Buff()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/LemurianMaster").GetComponent<CharacterMaster>();
            master.GetComponent<BaseAI>().fullVision = true;
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/LemurianBody");

            if (tw)
            {
                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.customName == "ChaseAndBiteOffNodegraph"
                                    select x).First();
                ai.maxDistance = 8f;

                AISkillDriver ai2 = (from x in master.GetComponents<AISkillDriver>()
                                     where x.customName == "ChaseAndBiteOffNodegraphWhileSlowingDown"
                                     select x).First();
                ai2.maxDistance = 0f;

                AISkillDriver ai3 = (from x in master.GetComponents<AISkillDriver>()
                                     where x.customName == "StrafeAndShoot"
                                     select x).First();
                ai3.minDistance = 10f;
                ai3.maxDistance = 100f;

                AISkillDriver ai4 = (from x in master.GetComponents<AISkillDriver>()
                                     where x.customName == "StrafeIdley"
                                     select x).First();

                ai4.minDistance = 10f;
                ai4.maxDistance = 100f;

                body.baseMoveSpeed = 9f;

                On.EntityStates.LemurianMonster.Bite.OnEnter += (orig, self) =>
                {
                    EntityStates.LemurianMonster.Bite.radius = 3f;
                    EntityStates.LemurianMonster.Bite.baseDuration = 0.8f;
                    EntityStates.LemurianMonster.Bite.forceMagnitude = 400f;
                    orig(self);
                };

                On.EntityStates.LemurianMonster.Bite.FixedUpdate += (orig, self) =>
                {
                    if (self.modelAnimator && self.modelAnimator.GetFloat("Bite.hitBoxActive") > 0.1f)
                    {
                        Vector3 direction = self.GetAimRay().direction;
                        Vector3 a = direction.normalized * 2f * self.moveSpeedStat;
                        Vector3 b = new Vector3(direction.x, 0f, direction.z).normalized * 2f;
                        self.characterMotor.Motor.ForceUnground();
                        self.characterMotor.velocity = a + b;
                    }
                    if (self.fixedAge > 0.5f)
                    {
                        self.attack.Fire(null);
                    }
                    orig(self);
                };
            }

            body.GetComponent<SetStateOnHurt>().canBeHitStunned = cbhs;

            On.EntityStates.LemurianMonster.ChargeFireball.OnEnter += (orig, self) =>
            {
                EntityStates.LemurianMonster.ChargeFireball.baseDuration = fbdur;
                orig(self);
            };

            var projectile = Resources.Load<GameObject>("prefabs/projectiles/Fireball").GetComponent<ProjectileSimple>();
            projectile.desiredForwardSpeed = fbspd;
            
        }
    }
}
