using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public static class LesserWisp
    {
        public static CharacterBody body;
        public static CharacterMaster master;
        public static void Buff()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/WispMaster").GetComponent<CharacterMaster>();
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/WispBody").GetComponent<CharacterBody>();
            body.baseMoveSpeed = 12f;
            body.baseAcceleration = 24f;

            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.minDistance == 0
                                select x).First();
            ai.maxDistance = 10f;

            AISkillDriver ai2 = (from x in master.GetComponents<AISkillDriver>()
                                where x.maxDistance == 30
                                select x).First();
            ai2.minDistance = 10f;
        }
    }
}
