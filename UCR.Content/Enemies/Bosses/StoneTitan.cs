using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Skills;

namespace UltimateCustomRun.Enemies.Bosses
{
    public static class StoneTitan
    {
        public static CharacterMaster master;
        public static CharacterBody body;
        public static void Buff()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/TitanMaster").GetComponent<CharacterMaster>();
            master.GetComponent<BaseAI>().fullVision = true;
            master.GetComponent<BaseAI>().aimVectorMaxSpeed = 240f;
            master.GetComponent<BaseAI>().aimVectorDampTime = 0.05f;
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/TitanBody");
            body.GetComponent<CharacterBody>().baseMoveSpeed = 10f;
            body.GetComponent<CharacterDirection>().turnSpeed = 170f;

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

            var fistdef = Resources.Load<SkillDef>("skilldefs/titanbody/TitanBodyFist");
            fistdef.baseRechargeInterval = 4f;

            On.EntityStates.TitanMonster.FireFist.OnEnter += (orig, self) =>
            {
                EntityStates.TitanMonster.FireFist.entryDuration = 1.6f;
                EntityStates.TitanMonster.FireFist.fireDuration = 1f;
                EntityStates.TitanMonster.FireFist.exitDuration = 1.6f;
                orig(self);
            };

            var rocksdef = Resources.Load<SkillDef>("skilldefs/titanbody/TitanBodyFist");
            rocksdef.baseRechargeInterval = 30f;

        }
    }
}
