namespace WellRoundedBalance.Mechanics.Bosses
{
    public class TeleportWhenOOB : MechanicBase
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
            if (characterBody.isChampion || characterBody.isBoss && characterBody.inventory)
            {
                characterBody.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
            }
        }
    }
}