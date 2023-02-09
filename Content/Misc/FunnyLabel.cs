using TMPro;

namespace WellRoundedBalance.Misc
{
    public static class FunnyLabel
    {
        public static void Hooks()
        {
            On.RoR2.UI.SteamBuildIdLabel.Start += (orig, self) =>
            {
                orig(self);
                var textMeshProUGUI = self.GetComponent<TextMeshProUGUI>();
                textMeshProUGUI.text += " | <style=cWorldEvent>WellRoundedBalance ver. " + Main.PluginVersion.ToString() + "</style>";
            };
        }
    }
}