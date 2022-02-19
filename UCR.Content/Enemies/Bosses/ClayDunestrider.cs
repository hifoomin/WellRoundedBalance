using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Skills;
using RoR2.Projectile;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class ClayDunestrider : EnemyBase
    {
        public static bool tw;
        public static CharacterBody body;
        public override string Name => ":::: Enemies ::: Clay Dunestrider";

        public override void Init()
        {
            tw = ConfigOption(false, "Make Clay Dunestrider animations better?", "Vanilla is false. Recommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            if (tw)
            {
                Nerf();
            }
        }
        public static void Nerf()
        {
            On.EntityStates.ClayBoss.PrepTarBall.OnEnter += (orig, self) =>
            {
                orig(self);
                self.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", 1.5f, 0.1f);
            };
            On.EntityStates.ClayBoss.FireTarball.OnEnter += (orig, self) =>
            {
                orig(self);
                self.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", 1.5f, 0.1f);
            };
            On.EntityStates.ClayBoss.ClayBossWeapon.ChargeBombardment.OnEnter += (orig, self) =>
            {
                orig(self);
                self.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", 1.5f, 0.1f);
            };
            On.EntityStates.ClayBoss.ClayBossWeapon.FireBombardment.OnEnter += (orig, self) =>
            {
                orig(self);
                self.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", 1.5f, 0.1f);
            };
            // this whole entire thing is to make atgs and stuff stop missing the dunestrider...
        }
    }
}
