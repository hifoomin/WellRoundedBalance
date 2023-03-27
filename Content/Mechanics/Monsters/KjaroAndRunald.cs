using static RoR2.CharacterBody;

namespace WellRoundedBalance.Mechanics.Monsters
{
    internal class KjaroAndRunald : MechanicBase
    {
        public override string Name => ":: Mechanics ::::::::: Kjaro And Runald Scaling";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            if (body.GetComponent<ElementalRingsBehavior>() != null && body.name == "LemurianBruiserBody(Clone)")
            {
                if (body.inventory)
                {
                    body.inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
                }
            }
        }
    }
}