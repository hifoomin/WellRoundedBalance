using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Skills;


namespace UltimateCustomRun.Enemies.Bosses
{
    public static class ImpOverlord
    {
        public static CharacterMaster master;
        public static CharacterBody body;
        public static CharacterBody bodybase;
        public static void Buff()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/ImpBossMaster").GetComponent<CharacterMaster>();
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/ImpBossBody").GetComponent<CharacterBody>();
            bodybase = Resources.Load<CharacterBody>("prefabs/characterbodies/ImpBossBody");

            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "GroundPound"
                                select x).First();
            ai.maxDistance = 10f;


            AISkillDriver ai2 = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "FireVoidspikesWhenInRange"
                                 select x).First();
            ai2.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            body.baseMoveSpeed = 16f;
            bodybase.GetComponent<CharacterDirection>().turnSpeed = 120f;
        }
    }
}
