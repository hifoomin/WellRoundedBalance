using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class LesserWisp : EnemyBase
    {
        public static bool Tweaks;
        public override string Name => ":::: Enemies :: Lesser Wisp";

        public override void Init()
        {
            Tweaks = ConfigOption(false, "Enable Speed Tweaks and AI Tweaks?", "Vanilla is false.\nRecommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            if (Tweaks)
            {
                Buff();
            }
        }

        public static void Buff()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/WispMaster").GetComponent<CharacterMaster>();
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/WispBody").GetComponent<CharacterBody>();
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