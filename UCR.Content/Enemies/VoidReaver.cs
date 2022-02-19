using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public class VoidReaver : EnemyBase
    {
        public static bool tw;
        public static CharacterMaster master;
        public static CharacterBody body;
        public override string Name => ":::: Enemies :: Void Reaver";

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
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/NullifierMaster").GetComponent<CharacterMaster>();
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/NullifierBody");
            body.GetComponent<CharacterBody>().baseMoveSpeed = 16f;

            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "PanicFireWhenClose"
                                select x).First();
            ai.movementType = AISkillDriver.MovementType.ChaseMoveTarget;

            AISkillDriver ai2 = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "StopAndAim"
                                select x).First();
            ai2.movementType = AISkillDriver.MovementType.ChaseMoveTarget;

            AISkillDriver ai3 = (from x in master.GetComponents<AISkillDriver>()
                                 where x.customName == "FireAndTrack"
                                 select x).First();
            ai3.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
        }
    }
}
