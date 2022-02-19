using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public class LesserWisp : EnemyBase
    {
        public static bool tw;
        public static CharacterBody body;
        public static CharacterMaster master;
        public override string Name => ":::: Enemies :: Lesser Wisp";

        public override void Init()
        {
            tw = ConfigOption(false, "Enable Speed Tweaks and AI Tweaks?", "Vanilla is false. Recommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            if (tw)
            {
                Buff();
            }
        }
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
