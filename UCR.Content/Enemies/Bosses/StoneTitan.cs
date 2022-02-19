using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Skills;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class StoneTitan : EnemyBase
    {
        public static bool aitw;
        public static bool fisttw;
        public static bool rockstw;
        public static bool speedtw;

        public override string Name => ":::: Enemies ::: Stone Titan";

        public override void Init()
        {
            aitw = ConfigOption(false, "Make Stone Titan AI smarter?", "Vanilla is false. Recommended Value: True");
            fisttw = ConfigOption(false, "Make Fist faster?", "Vanilla is false. Recommended Value: True");
            rockstw = ConfigOption(false, "Make Rock Turret faster?", "Vanilla is false. Recommended Value: True");
            speedtw = ConfigOption(false, "Make Stone Titan faster?", "Vanilla is false. Recommended Value: True");
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
            if (aitw)
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

            if (speedtw)
            {
                body.GetComponent<CharacterBody>().baseMoveSpeed = 10f;
                body.GetComponent<CharacterDirection>().turnSpeed = 170f;
            }
            
            if (fisttw)
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

            if (rockstw)
            {
                var rocksdef = Resources.Load<SkillDef>("skilldefs/titanbody/TitanBodyFist");
                rocksdef.baseRechargeInterval = 30f;
            }
        }
    }
}
