using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class GreaterWisp : EnemyBase
    {
        public static bool Tweaks;
        public static float Duration;
        public override string Name => ":::: Enemies :: Greater Wisp";

        public override void Init()
        {
            Duration = ConfigOption(2f, "Cannon Charge Time", "Vanilla is 2.\nRecommended Value: 1.25");
            Tweaks = ConfigOption(false, "Enable Damage Tweaks and AI Tweaks?", "Vanilla is false.\nRecommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/GreaterWispMaster").GetComponent<CharacterMaster>();
            if (Tweaks)
            {
                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.minDistance == 15f
                                    select x).First();
                ai.movementType = AISkillDriver.MovementType.StrafeMovetarget;
                ai.maxDistance = 100f;
            }

            On.EntityStates.GreaterWispMonster.ChargeCannons.OnEnter += (orig, self) =>
            {
                self.baseDuration = 1.25f;
                orig(self);
            };
        }
    }
}