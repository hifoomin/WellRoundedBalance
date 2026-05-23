using System;

namespace WellRoundedBalance.Items.Whites
{
    public class ElusiveAntlers : ItemBase<ElusiveAntlers>
    {
        public override string Name => ":: Items : Whites :: Elusive Antlers";
        public override ItemDef InternalPickup => DLC2Content.Items.SpeedBoostPickup;

        public override string PickupText => "Spawns orbs of energy that give bonus movement speed.";

        public override string DescText => $"Spawns orbs of energy nearby every <style=cIsUtility>10s</style> <style=cStack>(-10% per stack)</style>, giving <style=cIsUtility>+12% movement speed</style> up to <style=cIsUtility>{buffCount}</style> <style=cStack>(+{buffCount} per stack)</style> <style=cIsUtility>times</style> for <style=cIsUtility>12s</style>. ";

        [ConfigField("Max Count", 2)]
        public static int buffCount;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.GetElusiveAntlersCurrentMaxStack += AntlersMaxBuffs;
        }

        private int AntlersMaxBuffs(On.RoR2.CharacterBody.orig_GetElusiveAntlersCurrentMaxStack orig, CharacterBody self)
        {
            if (self.inventory) {
                return buffCount * self.inventory.GetItemCountEffective(DLC2Content.Items.SpeedBoostPickup);
            }

            return 0;
        }
    }
}