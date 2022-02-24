using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class ImpOverlord : EnemyBase
    {
        public static bool AITweaks;
        public static bool SpeedTweaks;
        public override string Name => ":::: Enemies ::: Imp Overlord";

        public override void Init()
        {
            AITweaks = ConfigOption(false, "Make Imp Overlord AI smarter?", "Vanilla is false.\nRecommended Value: True");
            SpeedTweaks = ConfigOption(false, "Make Imp Overlord faster?", "Vanilla is false.\nRecommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/ImpBossMaster").GetComponent<CharacterMaster>();
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/ImpBossBody").GetComponent<CharacterBody>();
            var bodybase = Resources.Load<CharacterBody>("prefabs/characterbodies/ImpBossBody");
            if (AITweaks)
            {
                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.customName == "GroundPound"
                                    select x).First();
                ai.maxDistance = 11f;

                AISkillDriver ai2 = (from x in master.GetComponents<AISkillDriver>()
                                     where x.customName == "FireVoidspikesWhenInRange"
                                     select x).First();
                ai2.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            }
            if (SpeedTweaks)
            {
                body.baseMoveSpeed = 18f;
                bodybase.GetComponent<CharacterDirection>().turnSpeed = 120f;
            }
        }
    }
}