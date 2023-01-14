using Inferno.Stat_AI;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    internal class RollOfPennies : ItemBase
    {
        public override string Name => ":: Items : Whites :: Roll of Pennies";

        public override string InternalPickupToken => "goldOnHurt";

        public override string PickupText => "Gain gold at the beginning of each stage.";

        public override string DescText => "Gain <style=cIsUtility>25</style> gold on pickup. At the beginning of every stage, gain <style=cIsUtility>15 <style=cStack>(+10 per stack)</style> gold</style>. <style=cIsUtility>Scales over time.</style>";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
            On.RoR2.Stage.Start += Stage_Start;
        }

        private void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            orig(self, itemIndex, count);
            if (NetworkServer.active)
            {
                if (itemIndex == DLC1Content.Items.GoldOnHurt.itemIndex)
                {
                    var master = self.gameObject.GetComponent<CharacterMaster>();
                    if (master)
                    {
                        master.GiveMoney(25);
                    }
                }
            }
        }

        private void Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            if (CharacterMaster.instancesList != null)
            {
                foreach (CharacterMaster master in CharacterMaster.instancesList)
                {
                    if (master.inventory)
                    {
                        var stack = master.inventory.GetItemCount(DLC1Content.Items.GoldOnHurt);
                        if (stack > 0)
                        {
                            master.GiveMoney(15 + 10 * ((uint)stack - 1));
                        }
                    }
                }
            }
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(3),
                x => x.MatchStloc(out _),
                x => x.MatchNewobj("RoR2.Orbs.GoldOrb")))
            {
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Roll of Pennies Gold hook");
            }
        }
    }
}