using UnityEngine;

namespace WellRoundedBalance.Interactables
{
    internal class Chests : InteractableBase
    {
        public override string Name => ":: Interactables :: Chests";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var legendaryChest = Utils.Paths.GameObject.GoldChest.Load<GameObject>();
            var legendaryChesturchaseInteraction = legendaryChest.GetComponent<PurchaseInteraction>();
            legendaryChesturchaseInteraction.cost = 250;

            var stealthedChest = Utils.Paths.InteractableSpawnCard.iscChest1Stealthed.Load<InteractableSpawnCard>();
            stealthedChest.maxSpawnsPerStage = 2;
            stealthedChest.directorCreditCost = 1;

            var stealthedChestGO = Utils.Paths.GameObject.Chest1StealthedVariant.Load<GameObject>();
            var chestBehavior = stealthedChestGO.GetComponent<ChestBehavior>();
            chestBehavior.tier1Chance = 0.6f;
            chestBehavior.tier2Chance = 0.4f;

            var smallDamage = Utils.Paths.GameObject.CategoryChestDamage.Load<GameObject>().GetComponent<PurchaseInteraction>();
            smallDamage.cost = 25;

            var smallHealing = Utils.Paths.GameObject.CategoryChestHealing.Load<GameObject>().GetComponent<PurchaseInteraction>();
            smallHealing.cost = 25;

            var smallUtility = Utils.Paths.GameObject.CategoryChestUtility.Load<GameObject>().GetComponent<PurchaseInteraction>();
            smallUtility.cost = 25;

            var largeDamage = Utils.Paths.GameObject.CategoryChest2DamageVariant.Load<GameObject>().GetComponent<PurchaseInteraction>();
            largeDamage.cost = 25;

            var largeHealing = Utils.Paths.GameObject.CategoryChest2HealingVariant.Load<GameObject>().GetComponent<PurchaseInteraction>();
            largeHealing.cost = 25;

            var largeUtility = Utils.Paths.GameObject.CategoryChest2UtilityVariant.Load<GameObject>().GetComponent<PurchaseInteraction>();
            largeUtility.cost = 25;
        }
    }
}