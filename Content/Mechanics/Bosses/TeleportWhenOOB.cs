namespace WellRoundedBalance.Mechanics.Bosses
{
    public class TeleportWhenOOB : MechanicBase<TeleportWhenOOB>
    {
        public override string Name => ":: Mechanics ::::: Boss Teleport OOB";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        public static int count = 0;

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            var inventory = body.inventory;
            if (!inventory) return;

            count = Util.GetItemCountForTeam(TeamIndex.Player, DLC1Content.Items.HalfSpeedDoubleHealth.itemIndex, true);
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody characterBody)
        {
            var inventory = characterBody.inventory;
            if (!inventory) return;

            if (count > 0)
            {
                if (characterBody.isChampion || characterBody.isBoss)
                {
                    characterBody.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
                }
            }
        }
    }
}