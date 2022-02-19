using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public class LunarExploder : EnemyBase
    {
        public static bool tw;
        public static CharacterBody body;
        public static CharacterMaster master;
        public override string Name => ":::: Enemies :: Lunar Exploder";

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
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/LunarExploderMaster").GetComponent<CharacterMaster>();
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/LunarExploderBody").GetComponent<CharacterBody>();
            body.baseMoveSpeed = 24f;
            body.baseAcceleration = 1000f;
            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "StrafeAndShoot"
                                select x).First();
            ai.maxDistance = 40f;
        }
    }
}
