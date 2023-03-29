using UnityEngine.Rendering.PostProcessing;

namespace WellRoundedBalance.Items.Reds
{
    public class Brainstalks : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Brainstalks";
        public override ItemDef InternalPickup => RoR2Content.Items.KillEliteFrenzy;

        public override string PickupText => "Skills have NO cooldowns for a short period after killing an elite.";

        public override string DescText => "Upon killing an elite monster, <style=cIsDamage>enter a frenzy</style> for <style=cIsDamage>4s</style> <style=cStack>(+4s per stack)</style> where <style=cIsUtility>skills have no cooldowns</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var pp = Utils.Paths.GameObject.NoCooldownEffect.Load<GameObject>().transform.GetChild(1).GetChild(0);
            var postProcessVolume = pp.GetComponent<PostProcessVolume>();
            var profile = postProcessVolume.profile;
            var colorGrading = profile.GetSetting<ColorGrading>();
            colorGrading.saturation.value = 15f;
        }
    }
}