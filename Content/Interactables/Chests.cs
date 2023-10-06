using RoR2.Artifacts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WellRoundedBalance.Interactables
{
    internal class Chests : InteractableBase<Chests>
    {
        public override string Name => ":: Interactables :: Chests";

        [ConfigField("Equipment Trishop Max Spawns Per Stage", "", 2)]
        public static int equipmentTrishopMaxSpawnsPerStage;

        [ConfigField("Legendary Chest Cost", "", 250)]
        public static int legendaryChestCost;

        [ConfigField("Small Category Chest Cost", "", 25)]
        public static int smallCategoryChestCost;

        [ConfigField("Large Category Chest Cost", "", 50)]
        public static int largeCategoryChestCost;

        [ConfigField("Timed Security Chest Orange Command Essence", "Should the Timed Security Chest drop an Orange Command Essence?", true)]
        public static bool command;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Run.GetDifficultyScaledCost_int_float += Run_GetDifficultyScaledCost_int_float;
            On.RoR2.ChestBehavior.Open += ChestBehavior_Open;
            On.RoR2.ChestBehavior.ItemDrop += ChestBehavior_ItemDrop;
            // fucking piece of shit why
            Changes();
        }

        private void ChestBehavior_ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, ChestBehavior self)
        {
            orig(self);
            ToggleCommand(false);
        }

        private void ChestBehavior_Open(On.RoR2.ChestBehavior.orig_Open orig, ChestBehavior self)
        {
            orig(self);
            if (IsTimedSecurityChest(self.gameObject))
            {
                ToggleCommand(true);
            }
        }

        private void Changes()
        {
            var legendaryChest = Utils.Paths.GameObject.GoldChest.Load<GameObject>();
            var legendaryChesturchaseInteraction = legendaryChest.GetComponent<PurchaseInteraction>();
            legendaryChesturchaseInteraction.cost = legendaryChestCost;

            var stealthedChest = Utils.Paths.InteractableSpawnCard.iscChest1Stealthed.Load<InteractableSpawnCard>();
            stealthedChest.maxSpawnsPerStage = 2;
            stealthedChest.directorCreditCost = 1;

            var smallDamage = Utils.Paths.GameObject.CategoryChestDamage.Load<GameObject>().GetComponent<PurchaseInteraction>();
            smallDamage.cost = smallCategoryChestCost;

            var smallHealing = Utils.Paths.GameObject.CategoryChestHealing.Load<GameObject>().GetComponent<PurchaseInteraction>();
            smallHealing.cost = smallCategoryChestCost;

            var smallUtility = Utils.Paths.GameObject.CategoryChestUtility.Load<GameObject>().GetComponent<PurchaseInteraction>();
            smallUtility.cost = smallCategoryChestCost;

            var largeDamage = Utils.Paths.GameObject.CategoryChest2DamageVariant.Load<GameObject>().GetComponent<PurchaseInteraction>();
            largeDamage.cost = largeCategoryChestCost;

            var largeHealing = Utils.Paths.GameObject.CategoryChest2HealingVariant.Load<GameObject>().GetComponent<PurchaseInteraction>();
            largeHealing.cost = largeCategoryChestCost;

            var largeUtility = Utils.Paths.GameObject.CategoryChest2UtilityVariant.Load<GameObject>().GetComponent<PurchaseInteraction>();
            largeUtility.cost = largeCategoryChestCost;

            var equipTrishop = Utils.Paths.InteractableSpawnCard.iscTripleShopEquipment.Load<InteractableSpawnCard>();
            equipTrishop.maxSpawnsPerStage = equipmentTrishopMaxSpawnsPerStage;
        }

        private int Run_GetDifficultyScaledCost_int_float(On.RoR2.Run.orig_GetDifficultyScaledCost_int_float orig, Run self, int baseCost, float difficultyCoefficient)
        {
            if (baseCost == 400)
            {
                baseCost = legendaryChestCost;
            }
            return orig(self, baseCost, difficultyCoefficient);
        }

        private bool IsTimedSecurityChest(GameObject go)
        {
            return go.name == "TimedChest(Clone)";
        }

        private void ToggleCommand(bool isEnabled)
        {
            var artifactOfCommand = ArtifactCatalog.FindArtifactDef("Command");
            if (RunArtifactManager.instance.IsArtifactEnabled(artifactOfCommand) == isEnabled)
            {
                return;
            }
            if (isEnabled)
            {
                RunArtifactManager.instance.SetArtifactEnabledServer(artifactOfCommand, true);
                return;
            }
            Task.Run(delegate () // the guh
            {
                Thread.Sleep(1500);
                RunArtifactManager.instance.SetArtifactEnabledServer(artifactOfCommand, false);
            });
        }
    }
}