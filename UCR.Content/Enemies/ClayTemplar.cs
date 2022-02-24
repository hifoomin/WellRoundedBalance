using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class ClayTemplar : EnemyBase
    {
        public static bool AITweaks;
        public static bool SecondaryTweaks;
        public static bool SpeedTweaks;
        public override string Name => ":::: Enemies :: Clay Templar";

        public override void Init()
        {
            AITweaks = ConfigOption(false, "Make Clay Templar AI smarter?", "Vanilla is false.\nRecommended Value: true");
            SecondaryTweaks = ConfigOption(false, "Make Shotgun better?", "Vanilla is false.\nRecommended Value: true");
            SpeedTweaks = ConfigOption(false, "Make Clay Templar faster?", "Vanilla is false.\nRecommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/ClayBruiserMaster");
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/ClayBruiserBody");

            if (AITweaks)
            {
                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.customName == "WalkAndShoot"
                                    select x).First();
                ai.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            }

            if (SecondaryTweaks)
            {
                On.EntityStates.ClayBruiser.Weapon.FireSonicBoom.OnEnter += (orig, self) =>
                {
                    self.baseDuration = 1f;
                    self.fieldOfView = 60f;
                    orig(self);
                };
            }

            if (SpeedTweaks)
            {
                body.baseMoveSpeed = 13f;
            }
        }
    }
}