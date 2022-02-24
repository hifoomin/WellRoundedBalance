using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class Grovetender : EnemyBase
    {
        public static bool AITweaks;
        public static bool SpeedTweaks;
        public static bool WispTweaks;
        public static bool EnableUnusedHook;
        public override string Name => ":::: Enemies ::: Grovetender";

        public override void Init()
        {
            AITweaks = ConfigOption(false, "Make Grovetender AI smarter?", "Vanilla is false.\nRecommended Value: True");
            SpeedTweaks = ConfigOption(false, "Make Grovetender faster?", "Vanilla is false.\nRecommended Value: True");
            WispTweaks = ConfigOption(false, "Make Wisps faster and have more HP?", "Vanilla is false.\nRecommended Value: True");
            EnableUnusedHook = ConfigOption(false, "Enable Unused Hook?", "Vanilla is false.\nRecommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/GravekeeperMaster").GetComponent<CharacterMaster>();
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/GravekeeperBody").GetComponent<CharacterBody>();
            if (AITweaks)
            {
                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.customName == "RunAndShoot"
                                    select x).First();
                ai.movementType = AISkillDriver.MovementType.StrafeMovetarget;

                AISkillDriver ai2 = (from x in master.GetComponents<AISkillDriver>()
                                     where x.customName == "Hooks"
                                     select x).First();
                ai2.movementType = AISkillDriver.MovementType.StrafeMovetarget;
                ai2.maxUserHealthFraction = Mathf.Infinity;
                ai2.minDistance = 12f;

                AISkillDriver ai3 = (from x in master.GetComponents<AISkillDriver>()
                                     where x.customName == "WaitAroundUntilSkillIsBack"
                                     select x).First();
                ai3.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            }

            if (SpeedTweaks)
            {
                body.baseMoveSpeed = 20f;

                On.EntityStates.GravekeeperBoss.PrepHook.OnEnter += (orig, self) =>
                {
                    EntityStates.GravekeeperBoss.PrepHook.baseDuration = 1f;
                    orig(self);
                };
            }

            if (WispTweaks)
            {
                var wisp = Resources.Load<GameObject>("prefabs/projectiles/GravekeeperTrackingFireball").GetComponent<CharacterBody>();
                wisp.baseMoveSpeed = 50f;
                wisp.baseMaxHealth = 50f;
                On.EntityStates.GravekeeperMonster.Weapon.GravekeeperBarrage.OnEnter += (orig, self) =>
                {
                    EntityStates.GravekeeperMonster.Weapon.GravekeeperBarrage.damageCoefficient = 2f;
                    EntityStates.GravekeeperMonster.Weapon.GravekeeperBarrage.missileSpawnDelay = 0.15f;
                    EntityStates.GravekeeperMonster.Weapon.GravekeeperBarrage.missileSpawnFrequency = 5f;
                    orig(self);
                };

                var wisp2 = Resources.Load<SkillDef>("skilldefs/gravekeeperbody/GravekeeperBodyBarrage");
                wisp2.baseRechargeInterval = 3f;
            }

            if (EnableUnusedHook)
            {
                On.EntityStates.GravekeeperBoss.FireHook.OnEnter += (orig, self) =>
                {
                    EntityStates.GravekeeperBoss.FireHook.projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/GravekeeperHookProjectile");
                    EntityStates.GravekeeperBoss.FireHook.projectileDamageCoefficient = 1.2f;
                    EntityStates.GravekeeperBoss.FireHook.baseDuration = 1.5f;
                    EntityStates.GravekeeperBoss.FireHook.projectileCount = 40;
                    EntityStates.GravekeeperBoss.FireHook.spread = 360f;
                    orig(self);
                };
            }
        }
    }
}