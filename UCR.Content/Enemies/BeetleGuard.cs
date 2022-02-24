using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class BeetleGuard : EnemyBase
    {
        public static bool AITweaks;
        public static bool SunderTweaks;
        public static bool SpeedTweaks;
        public override string Name => ":::: Enemies :: Beetle Guard";

        public override void Init()
        {
            AITweaks = ConfigOption(false, "Make Beetle Guard AI smarter?", "Vanilla is false.\nRecommended Value: true");
            SunderTweaks = ConfigOption(false, "Make Beetle Guard spammier?", "Vanilla is false.\nRecommended Value: true");
            SpeedTweaks = ConfigOption(false, "Make Beetle Guard faster?", "Vanilla is false.\nRecommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/BeetleGuardMaster");
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/BeetleGuardBody");

            if (AITweaks)
            {
                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.customName == "FireSunder"
                                    select x).First();
                ai.maxDistance = 80f;
            }

            if (SunderTweaks)
            {
                On.EntityStates.BeetleGuardMonster.FireSunder.OnEnter += (orig, self) =>
                {
                    EntityStates.BeetleGuardMonster.FireSunder.baseDuration = 1.7f;
                    orig(self);
                };
            }

            if (SpeedTweaks)
            {
                body.baseMoveSpeed = 22f;
                On.EntityStates.BeetleGuardMonster.GroundSlam.OnEnter += (orig, self) =>
                {
                    EntityStates.BeetleGuardMonster.GroundSlam.baseDuration = 2.4f;
                    orig(self);
                };
            }
        }
    }
}