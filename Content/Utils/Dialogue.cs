using RoR2.UI;

namespace WellRoundedBalance.Utils
{
    public class Dialogue
    {
        public static MPInput input;

        public static SimpleDialogBox ShowPopup(string title, string desc, bool noCancel = false)
        {
            Time.timeScale = 0f;
            input.eventSystem.cursorOpenerCount++;
            input.eventSystem.cursorOpenerForGamepadCount++;
            SimpleDialogBox box = SimpleDialogBox.Create();
            box.headerToken = new SimpleDialogBox.TokenParamsPair(title);
            box.descriptionToken = new SimpleDialogBox.TokenParamsPair(desc);
            if (!noCancel) box.AddActionButton(() => DefaultCancel(input.eventSystem), "OK");
            return box;
        }

        public static void DefaultCancel(MPEventSystem events)
        {
            events.cursorOpenerCount--;
            events.cursorOpenerForGamepadCount--;
            if (SimpleDialogBox.instancesList.Count <= 1) Time.timeScale = 1f;
        }
    }
}
