using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Skills;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class Grovetender : EnemyBase
    {
        public static bool aitw;
        public static bool speedtw;
        public static bool wisptw;
        public static bool eun;
        public static CharacterMaster master;
        public static CharacterBody body;
        public override string Name => ":::: Enemies ::: Grovetender";

        public override void Init()
        {
            aitw = ConfigOption(false, "Make Grovetender AI smarter?", "Vanilla is false. Recommended Value: True");
            speedtw = ConfigOption(false, "Make Grovetender faster?", "Vanilla is false. Recommended Value: True");
            aitw = ConfigOption(false, "Make Wisps faster and have more HP?", "Vanilla is false. Recommended Value: True");
            eun = ConfigOption(false, "Enable Unused Hook?", "Vanilla is false. Recommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }
        public static void Buff()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/GravekeeperMaster").GetComponent<CharacterMaster>();
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/GravekeeperBody").GetComponent<CharacterBody>();
            if (aitw)
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

            if (speedtw)
            {
                body.baseMoveSpeed = 16f;

                On.EntityStates.GravekeeperBoss.PrepHook.OnEnter += (orig, self) =>
                {
                    EntityStates.GravekeeperBoss.PrepHook.baseDuration = 1f;
                    orig(self);
                };
            }

            if (wisptw)
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

            if (eun)
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
