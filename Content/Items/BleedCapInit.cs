using BepInEx.Configuration;
using WellRoundedBalance.Misc;

namespace WellRoundedBalance.Items
{
    public static class BleedCapInit
    {
        public static ConfigEntry<int> initialBleedCap { get; set; }

        public static void Init()
        {
            initialBleedCap = Main.WRBItemConfig.Bind(":: Items : Bleed :: Bleed Cap", "Initial Bleed Cap without any items", 4);
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.RoR2.DotController.InflictDot_GameObject_GameObject_DotIndex_float_float_Nullable1 += DotController_InflictDot_GameObject_GameObject_DotIndex_float_float_Nullable1;
            RecalculateEvent.RecalculateBleedCap += (object sender, RecalculateEventArgs args) =>
            {
                if (args.BleedCap)
                {
                    var body = args.BleedCap.body;
                    if (body)
                    {
                        args.BleedCap.bleedCapAdd += initialBleedCap.Value;
                    }
                }
            };
        }

        private static void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            if (body.isPlayerControlled && body.GetComponent<BleedCap>() == null && body.inventory)
                body.gameObject.AddComponent<BleedCap>();
        }

        private static void DotController_InflictDot_GameObject_GameObject_DotIndex_float_float_Nullable1(On.RoR2.DotController.orig_InflictDot_GameObject_GameObject_DotIndex_float_float_Nullable1 orig, GameObject victimObject, GameObject attackerObject, DotController.DotIndex dotIndex, float duration, float damageMultiplier, uint? maxStacksFromAttacker)
        {
            var attackerBody = attackerObject.GetComponent<CharacterBody>();
            if (attackerBody)
            {
                var bleedCap = attackerBody.GetComponent<BleedCap>();
                var inventory = attackerBody.inventory;
                if (bleedCap && inventory)
                {
                    if (dotIndex == DotController.DotIndex.Bleed)
                    {
                        maxStacksFromAttacker = (uint)bleedCap.bleedCap;
                    }
                }
            }
            orig(victimObject, attackerObject, dotIndex, duration, damageMultiplier, maxStacksFromAttacker);
        }
    }
}