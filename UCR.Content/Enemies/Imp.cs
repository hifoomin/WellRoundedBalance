using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class Imp : EnemyBase
    {
        public static bool Tweaks;
        public override string Name => ":::: Enemies :: Imp";

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
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/ImpMaster").GetComponent<CharacterMaster>();
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/ImpBody").GetComponent<CharacterBody>();
            body.baseMoveSpeed = 14f;

            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "Slash"
                                select x).First();
            ai.maxDistance = 7f;
        }
    }
}