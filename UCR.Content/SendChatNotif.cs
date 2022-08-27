using RoR2;

namespace UltimateCustomRun
{
    public static class SendChatNotif
    {
        public static void Send()
        {
            Run.onRunStartGlobal += Message;
        }

        private static void Message(Run obj)
        {
            var msg = "</size></color><color=#BFA9D3>Thanks for trying out </color><color=#8932D5>UltimateCustomRun.</color>\n" +
                      "<color=#BFA9D3>For any mod devs that see this, feel free to contribute and make the mod as good as possible.\n" +
                      "There is a to-do list regarding items in the Main Class.\n" +
                      "<i>Github PR's / Issues are best</i>, but DMs and pings are also welcome. Have fun and peace out! \u2764</color>";
            // escape color first from those pesky <#F38> and <size=500%> steam users :smirk_cat:

            var timer = new System.Timers.Timer(8000);
            timer.Elapsed += delegate
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = msg });
            };
            timer.AutoReset = false;
            timer.Start();
        }
    }
}