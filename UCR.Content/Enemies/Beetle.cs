using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
// fuck off
namespace UltimateCustomRun.Enemies
{
    public static class Beetle
    {
        public static CharacterMaster master;
        public static CharacterBody body;
        public static void Buff()
        {
            On.EntityStates.BeetleMonster.HeadbuttState.FixedUpdate += (orig, self) =>
            {
                EntityStates.BeetleMonster.HeadbuttState.baseDuration = 2.25f;
                if (self.modelAnimator && self.modelAnimator.GetFloat("Headbutt.hitBoxActive") > 0.5f)
                {
                    Vector3 direction = self.GetAimRay().direction;
                    Vector3 a = direction.normalized * 2f * self.moveSpeedStat;
                    Vector3 b = Vector3.up * 5f;
                    Vector3 b2 = new Vector3(direction.x, 0f, direction.z).normalized * 4.5f;
                    self.characterMotor.Motor.ForceUnground();
                    self.characterMotor.velocity = a + b + b2;
                }
                if (self.fixedAge > 0.5f)
                {
                    self.attack.Fire(null);
                }
                orig(self);
            };

            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/BeetleMaster").GetComponent<CharacterMaster>();
            master.GetComponent<BaseAI>().fullVision = true;

            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "HeadbuttOffNodegraph"
                                select x).First();
            ai.maxDistance = 25f;
            ai.selectionRequiresOnGround = true;
            ai.activationRequiresAimTargetLoS = true;

            body = Resources.Load<CharacterBody>("prefabs/characterbodies/BeetleBody");
            body.GetComponent<SetStateOnHurt>().canBeHitStunned = false;
            body.baseMoveSpeed = 12f;

            On.EntityStates.GenericCharacterSpawnState.OnEnter += (orig, self) =>
            {
                if (self is EntityStates.BeetleMonster.SpawnState)
                {
                    self.duration = 2.5f;
                }
                orig(self);
            };
        }
    }
}
