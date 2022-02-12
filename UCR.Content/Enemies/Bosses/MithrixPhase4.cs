using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.Skills;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
// fuck off
namespace UltimateCustomRun.Enemies.Bosses
{
    public static class MithrixPhase4
    {
        public static CharacterMaster master;
        public static CharacterMaster masterbase;
        public static CharacterBody body;
        public static void Buff()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/BrotherHurtMaster").GetComponent<CharacterMaster>();
            masterbase = Resources.Load<CharacterMaster>("prefabs/charactermasters/BrotherHurtMaster");
            masterbase.GetComponent<BaseAI>().aimVectorDampTime = 0.07f;
            masterbase.GetComponent<BaseAI>().aimVectorMaxSpeed = 400f;
            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "SlamGround"
                                select x).First();
            ai.maxUserHealthFraction = Mathf.Infinity;
            ai.movementType = AISkillDriver.MovementType.StrafeMovetarget;

            AISkillDriver ai2 = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "Shoot"
                                 select x).First();
            ai2.movementType = AISkillDriver.MovementType.StrafeMovetarget;

            body = Resources.Load<CharacterBody>("prefabs/characterbodies/BrotherHurtBody");
            body.baseMoveSpeed = 16f;
            body.baseMaxHealth = 1700f;
            body.levelMaxHealth = 510f;
            body.sprintingSpeedMultiplier = 1.45f;

            On.EntityStates.BrotherMonster.FistSlam.OnEnter += (orig, self) =>
            {
                EntityStates.BrotherMonster.FistSlam.damageCoefficient = 3f;
                EntityStates.BrotherMonster.FistSlam.healthCostFraction = 0f;
                EntityStates.BrotherMonster.FistSlam.waveProjectileCount = 20;
                EntityStates.BrotherMonster.FistSlam.baseDuration = 3f;
                orig(self);
            };
            On.EntityStates.BrotherMonster.FistSlam.OnExit += (orig, self) =>
            {
                Ray aimRay = self.GetAimRay();
                if (self.isAuthority)
                {
                    int wavecount = 360 / 45;
                    Vector3 point = Vector3.ProjectOnPlane(self.inputBank.aimDirection, Vector3.up);
                    Transform transform = self.FindModelChild(EntityStates.BrotherMonster.FistSlam.muzzleString);
                    Vector3 position = transform.position;
                    for (int i = 0; i < wavecount; i++)
                    {
                        Vector3 forward = Quaternion.AngleAxis(wavecount * i, Vector3.up) * point;
                        ProjectileManager.instance.FireProjectile(EntityStates.BrotherMonster.ExitSkyLeap.waveProjectilePrefab, position, RoR2.Util.QuaternionSafeLookRotation(forward), self.gameObject, self.characterBody.damage * 2f, -200f, RoR2.Util.CheckRoll(self.characterBody.crit, self.characterBody.master), DamageColorIndex.Default, null, -1f);
                    }
                }
                orig(self);
            };

            var shardsdef = Resources.Load<SkillDef>("skilldefs/brotherbody/FireLunarShardsHurt");
            shardsdef.baseMaxStock = 32;
            shardsdef.rechargeStock = 32;

            On.EntityStates.BrotherMonster.SpellChannelEnterState.OnEnter += (orig, self) =>
            {
                EntityStates.BrotherMonster.SpellChannelEnterState.duration = 3f;
                orig(self);
            };
            On.EntityStates.BrotherMonster.SpellChannelState.OnEnter += (orig, self) =>
            {
                EntityStates.BrotherMonster.SpellChannelState.stealInterval = 0f;
                EntityStates.BrotherMonster.SpellChannelState.delayBeforeBeginningSteal = 0f;
                EntityStates.BrotherMonster.SpellChannelState.maxDuration = 1f;
                orig(self);
            };
            On.EntityStates.BrotherMonster.SpellChannelExitState.OnEnter += (orig, self) =>
            {
                EntityStates.BrotherMonster.SpellChannelExitState.lendInterval = 0f;
                EntityStates.BrotherMonster.SpellChannelExitState.duration = 2.5f;
                orig(self);
            };
            On.EntityStates.BrotherMonster.StaggerEnter.OnEnter += (orig, self) =>
            {
                self.duration = 0f;
                orig(self);
            };
            On.EntityStates.BrotherMonster.StaggerExit.OnEnter += (orig, self) =>
            {
                self.duration = 0f;
                orig(self);
            };
            On.EntityStates.BrotherMonster.StaggerLoop.OnEnter += (orig, self) =>
            {
                self.duration = 0f;
                orig(self);
            };
            On.EntityStates.BrotherMonster.TrueDeathState.OnEnter += (orig, self) =>
            {
                EntityStates.BrotherMonster.TrueDeathState.dissolveDuration = 3f;
                orig(self);
            };
        }
    }
}
