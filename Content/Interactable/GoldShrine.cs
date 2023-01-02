using UnityEngine;

namespace WellRoundedBalance.Interactable
{
    internal class GoldShrine : InteractableBase
    {
        public override string Name => "Interactables :::::::::: Gold Shrine";

        public override void Hooks()
        {
            var goldShrine = Utils.Paths.GameObject.ShrineGoldshoresAccess.Load<GameObject>();
            var goldShrinePurchaseInteraction = goldShrine.GetComponent<PurchaseInteraction>();
            goldShrinePurchaseInteraction.cost = 125;
        }
    }
}