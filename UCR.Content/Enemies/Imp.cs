using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public class Imp : EnemyBase
    {
        public static bool tw;
        public static CharacterMaster master;
        public static CharacterBody body;
        public override string Name => ":::: Enemies :: Imp";

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
