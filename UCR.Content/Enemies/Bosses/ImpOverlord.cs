using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Skills;


namespace UltimateCustomRun.Enemies.Bosses
{
    public class ImpOverlord : EnemyBase
    {
        public static bool aitw
        public static bool speedtw;
        public static CharacterMaster master;
        public static CharacterBody body;
        public static CharacterBody bodybase;
        public override string Name => ":::: Enemies ::: Imp Overlord";

        public override void Init()
        {
            aitw = ConfigOption(false, "Make Imp Overlord AI smarter?", "Vanilla is false. Recommended Value: True");
            speedtw = ConfigOption(false, "Make Imp Overlord faster?", "Vanilla is false. Recommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }
        public static void Buff()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/ImpBossMaster").GetComponent<CharacterMaster>();
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/ImpBossBody").GetComponent<CharacterBody>();
            bodybase = Resources.Load<CharacterBody>("prefabs/characterbodies/ImpBossBody");
            if (aitw)
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
            if (speedtw)
            {
                body.baseMoveSpeed = 18f;
                bodybase.GetComponent<CharacterDirection>().turnSpeed = 120f;
            }
        }
    }
}
