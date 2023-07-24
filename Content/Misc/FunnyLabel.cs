using TMPro;

namespace WellRoundedBalance.Misc
{
    public static class FunnyLabel
    {
        public static void Init()
        {
            On.RoR2.UI.SteamBuildIdLabel.Start += (orig, self) =>
            {
                orig(self);
                var textMeshProUGUI = self.GetComponent<TextMeshProUGUI>();
                textMeshProUGUI.text += " | <style=cWorldEvent>WellRoundedBalance ver. " + Main.PluginVersion.ToString() + "</style>";
            };

            Changes();
        }

        private static void Changes()
        {
            var sequence = Utils.Paths.GameEndingDef.EscapeSequenceFailed.Load<GameEndingDef>();
            sequence.icon = Main.wellroundedbalance.LoadAsset<Sprite>("Assets/WellRoundedBalance/texLoss.png");

            var limbo = Utils.Paths.GameEndingDef.LimboEnding.Load<GameEndingDef>();
            limbo.icon = Main.wellroundedbalance.LoadAsset<Sprite>("Assets/WellRoundedBalance/texLoss.png");

            var main = Utils.Paths.GameEndingDef.MainEnding.Load<GameEndingDef>();
            main.icon = Main.wellroundedbalance.LoadAsset<Sprite>("Assets/WellRoundedBalance/texWin.png");

            var oblit = Utils.Paths.GameEndingDef.ObliterationEnding.Load<GameEndingDef>();
            oblit.icon = Main.wellroundedbalance.LoadAsset<Sprite>("Assets/WellRoundedBalance/texUnknown.png");

            var loss = Utils.Paths.GameEndingDef.StandardLoss.Load<GameEndingDef>();
            loss.icon = Main.wellroundedbalance.LoadAsset<Sprite>("Assets/WellRoundedBalance/texLoss.png");

            var trial = Utils.Paths.GameEndingDef.PrismaticTrialEnding.Load<GameEndingDef>();
            trial.icon = Main.wellroundedbalance.LoadAsset<Sprite>("Assets/WellRoundedBalance/texWin.png");

            var @void = Utils.Paths.GameEndingDef.VoidEnding.Load<GameEndingDef>();
            @void.icon = Main.wellroundedbalance.LoadAsset<Sprite>("Assets/WellRoundedBalance/texUnknown.png");
        }
    }
}