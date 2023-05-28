using RoR2.UI;

namespace WellRoundedBalance.Mechanics.Health
{
    public class LowHealthThreshold : MechanicBase<LowHealthThreshold>
    {
        public override string Name => ":: Mechanics :: Low Health Threshold";

        [ConfigField("Low Health Fraction", "Decimal. Affects low health visuals and all other mods using it!", 0.25f)]
        public static float lowHealthFraction;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.HealthComponent.Awake += ChangeThreshold;
            On.RoR2.UI.HealthBar.UpdateBarInfos += HealthBar_UpdateBarInfos;
            Fragile.Hook();
        }

        private void HealthBar_UpdateBarInfos(On.RoR2.UI.HealthBar.orig_UpdateBarInfos orig, RoR2.UI.HealthBar self)
        {
            orig(self);
            var hc = self.source;
            if (hc)
            {
                var bar = self.barInfoCollection.lowHealthUnderBarInfo;
                bool underHalf = (hc.health / hc.shield) / hc.fullCombinedHealth <= 0.5f;
                bar.enabled = self.hasLowHealthItem && underHalf;

                bar.normalizedXMax = 0.5f * (1f - hc.GetHealthBarValues().curseFraction);
            }
        }

        private void ChangeThreshold(On.RoR2.HealthComponent.orig_Awake orig, HealthComponent self)
        {
            HealthComponent.lowHealthFraction = lowHealthFraction;
            orig(self);
        }
    }

    public class Fragile
    {
        private static List<ItemDef> fragileDefs;
        private static Dictionary<ItemDef, FragileInfo> fragileMap;

        public struct FragileInfo
        {
            public float fraction;
        }

        public static void Hook()
        {
            On.RoR2.UI.HealthBar.UpdateBarInfos += UpdateBarInfos;
            fragileDefs = new();
            fragileMap = new();
        }

        private static void UpdateBarInfos(On.RoR2.UI.HealthBar.orig_UpdateBarInfos orig, HealthBar self) {
            orig(self);
            float highest = LowHealthThreshold.lowHealthFraction;
            if (!self.source || !self.source.body || !self.source.body.inventory) {
                return;
            }
            if (self.source.body.inventory) {
                CharacterBody body = self.source.body;
                Inventory inventory = body.inventory;

                foreach (ItemIndex item in inventory.itemAcquisitionOrder) {
                    ItemDef def = ItemCatalog.GetItemDef(item);
                    if (fragileMap.TryGetValue(def, out FragileInfo info)) {
                        self.hasLowHealthItem = true;
                        float newFraction = info.fraction * 0.01f;
                        if (newFraction > highest) {
                            highest = newFraction;
                        }
                    }
                }
            }

            var val = self.source.GetHealthBarValues();
            self.barInfoCollection.lowHealthOverBarInfo.enabled = self.hasLowHealthItem && self.source.IsAboveFraction(highest * 100);
            self.barInfoCollection.lowHealthOverBarInfo.normalizedXMin = highest * (1f - val.curseFraction);
            self.barInfoCollection.lowHealthOverBarInfo.normalizedXMax = highest * (1f - val.curseFraction) + 0.005f;
            self.barInfoCollection.lowHealthOverBarInfo.color = self.style.lowHealthOverStyle.baseColor;
            self.barInfoCollection.lowHealthOverBarInfo.sprite = self.style.lowHealthOverStyle.sprite;
            self.barInfoCollection.lowHealthOverBarInfo.imageType = self.style.lowHealthOverStyle.imageType;
            self.barInfoCollection.lowHealthOverBarInfo.sizeDelta = self.style.lowHealthOverStyle.sizeDelta;
            
            self.barInfoCollection.lowHealthUnderBarInfo.enabled = self.hasLowHealthItem && !self.source.IsAboveFraction(highest * 100);
            self.barInfoCollection.lowHealthUnderBarInfo.normalizedXMin = 0f;
            self.barInfoCollection.lowHealthUnderBarInfo.normalizedXMax = highest * (1f - val.curseFraction);
            self.barInfoCollection.lowHealthUnderBarInfo.color = self.style.lowHealthUnderStyle.baseColor;
            self.barInfoCollection.lowHealthUnderBarInfo.sprite = self.style.lowHealthUnderStyle.sprite;
            self.barInfoCollection.lowHealthUnderBarInfo.imageType = self.style.lowHealthUnderStyle.imageType;
            self.barInfoCollection.lowHealthUnderBarInfo.sizeDelta = self.style.lowHealthUnderStyle.sizeDelta;
        }

        /// <summary>
        /// makes an item break below 25% health and give the player the broken version
        /// </summary>
        /// <param name="fragileItem"> the fragile itemdef </param>
        /// <param name="brokenVersion"> the broken version to grant </param>
        public static void AddFragileItem(ItemDef fragileItem, FragileInfo info)
        {
            if (fragileItem)
            {
                fragileDefs.Add(fragileItem);
                fragileMap.Add(fragileItem, info);
            }
        }
    }
}