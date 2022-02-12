using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public static class Imp
    {
        public static CharacterMaster master;
        public static CharacterBody body;
        public static void Buff()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/ImpMaster").GetComponent<CharacterMaster>();
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/ImpBody").GetComponent<CharacterBody>();
            body.baseMoveSpeed = 14f;

            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "Slash"
                                select x).First();
            ai.maxDistance = 7f;
        }
    }
}
