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
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody characterBody)
        {
            var inventory = characterBody.inventory;
            if (!inventory) return;

            var stack = inventory.GetItemCount(DLC1Content.Items.HalfSpeedDoubleHealth);
            if (stack > 0)
            {
                if (characterBody.isChampion || characterBody.isBoss)
                {
                    characterBody.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
                }

            }
        }
    }
}