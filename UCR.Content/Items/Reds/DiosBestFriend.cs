using RoR2;

namespace UltimateCustomRun
{
    public static class DiosBestFriend
    {
        public static void AddBehavior()
        {
            /*
            On.RoR2.CharacterMaster.RespawnExtraLife += (orig, self) =>
            {
                orig(self);
                var body = self.GetComponent<RoR2.CharacterBody>().inventory;
                if (body)
                {
                    var stack = body.GetItemCount(RoR2.RoR2Content.Items.ExtraLifeConsumed);
                    for (int i = 0; i < stack; i++)
                    {
                        body.GiveItem(RoR2.RoR2Content.Items.Bear.itemIndex, Main.DiosTTCount.Value);
                        // if only i could easily unstringify a fucking string eeeeeeeeeeeeeeeee
                    }
                }
            };
            */
        }
    }
}
