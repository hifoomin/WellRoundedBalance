using UnityEngine;

namespace WellRoundedBalance.Interactable
{
    internal class LegendaryChest : InteractableBase
    {
        public override string Name => "Interactables ::::::::::: Legendary Chest";

        public override void Hooks()
        {
            var legendaryChest = Utils.Paths.GameObject.GoldChest.Load<GameObject>();
            var legendaryChesturchaseInteraction = legendaryChest.GetComponent<PurchaseInteraction>();
            legendaryChesturchaseInteraction.cost = 250;
        }
    }
}