namespace UltimateCustomRun.Enemies.Bosses
{
    public class ClayDunestrider : EnemyBase
    {
        public static bool Tweaks;
        public override string Name => ":::: Enemies ::: Clay Dunestrider";

        public override void Init()
        {
            Tweaks = ConfigOption(false, "Make Clay Dunestrider animations better?", "Vanilla is false.\nRecommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            if (Tweaks)
            {
                Nerf();
            }
        }

        public static void Nerf()
        {
            On.EntityStates.ClayBoss.PrepTarBall.OnEnter += (orig, self) =>
            {
                self.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", 1.5f, 0.1f);
                orig(self);
                self.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", 1.5f, 0.1f);
            };
            On.EntityStates.ClayBoss.FireTarball.OnEnter += (orig, self) =>
            {
                self.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", 1.5f, 0.1f);
                orig(self);
                self.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", 1.5f, 0.1f);
            };
            On.EntityStates.ClayBoss.ClayBossWeapon.ChargeBombardment.OnEnter += (orig, self) =>
            {
                self.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", 1.5f, 0.1f);
                orig(self);
                self.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", 1.5f, 0.1f);
            };
            On.EntityStates.ClayBoss.ClayBossWeapon.FireBombardment.OnEnter += (orig, self) =>
            {
                self.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", 1.5f, 0.1f);
                orig(self);
                self.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", 1.5f, 0.1f);
            };
            // this whole entire thing is to make atgs and stuff stop missing the dunestrider...
        }
    }
}