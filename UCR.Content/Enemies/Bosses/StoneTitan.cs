using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class StoneTitan : EnemyBase
    {
        public static bool AITweaks;
        public static bool FistTweaks;
        public static bool RockTweaks;
        public static bool SpeedTweaks;

        public override string Name => ":::: Enemies ::: Stone Titan";

        public override void Init()
        {
            AITweaks = ConfigOption(false, "Make Stone Titan AI smarter?", "Vanilla is false.\nRecommended Value: True");
            FistTweaks = ConfigOption(false, "Make Fist faster?", "Vanilla is false.\nRecommended Value: True");
            RockTweaks = ConfigOption(false, "Make Rock Turret faster?", "Vanilla is false.\nRecommended Value: True");
            SpeedTweaks = ConfigOption(false, "Make Stone Titan faster?", "Vanilla is false.\nRecommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/TitanMaster").GetComponent<CharacterMaster>();
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/TitanBody");
            if (AITweaks)
            {
                master.GetComponent<BaseAI>().fullVision = true;
                master.GetComponent<BaseAI>().aimVectorMaxSpeed = 240f;
                master.GetComponent<BaseAI>().aimVectorDampTime = 0.05f;

                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.skillSlot == SkillSlot.Special
                                    select x).First();
                ai.maxUserHealthFraction = Mathf.Infinity;
                ai.minDistance = 16f;
                ai.movementType = AISkillDriver.MovementType.StrafeMovetarget;

                AISkillDriver ai2 = (from x in master.GetComponents<AISkillDriver>()
                                     where x.skillSlot == SkillSlot.Utility
                                     select x).First();
                ai2.maxUserHealthFraction = 0.8f;
                ai2.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            }

            if (SpeedTweaks)
            {
                body.GetComponent<CharacterBody>().baseMoveSpeed = 10f;
                body.GetComponent<CharacterDirection>().turnSpeed = 170f;
            }

            if (FistTweaks)
            {
                var fistdef = Resources.Load<SkillDef>("skilldefs/titanbody/TitanBodyFist");
                fistdef.baseRechargeInterval = 4f;

                On.EntityStates.TitanMonster.FireFist.OnEnter += (orig, self) =>
                {
                    EntityStates.TitanMonster.FireFist.entryDuration = 1.6f;
                    EntityStates.TitanMonster.FireFist.fireDuration = 1f;
                    EntityStates.TitanMonster.FireFist.exitDuration = 1.6f;
                    orig(self);
                };
            }

            if (RockTweaks)
            {
                var rocksdef = Resources.Load<SkillDef>("skilldefs/titanbody/TitanBodyFist");
                rocksdef.baseRechargeInterval = 30f;
            }
        }
    }
}