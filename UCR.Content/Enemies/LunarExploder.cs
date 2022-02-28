using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class LunarExploder : EnemyBase
    {
        public static bool Tweaks;
        public override string Name => ":::: Enemies :: Lunar Exploder";

        public override void Init()
        {
            Tweaks = ConfigOption(false, "Make Lunar Exploder faster?", "Vanilla is false.\nRecommended Value: true");
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
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/LunarExploderMaster").GetComponent<CharacterMaster>();
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/LunarExploderBody").GetComponent<CharacterBody>();
            body.baseMoveSpeed = 20f;
            body.baseAcceleration = 1000f;
            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "StrafeAndShoot"
                                select x).First();
            ai.maxDistance = 100f;
            AISkillDriver ai2 = (from x in master.GetComponents<AISkillDriver>()
                                 where x.customName == "SprintNodegraphAndShoot"
                                 select x).First();
            ai2.maxDistance = 100f;
        }
    }
}