﻿using MonoMod.Cil;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class LysateCell : ItemBase<LysateCell>
    {
        public override string Name => ":: Items :::::: Voids :: Lysate Cell";
        public override ItemDef InternalPickup => DLC1Content.Items.EquipmentMagazineVoid;

        public override string PickupText => "Add an extra charge of your Special skill" +
                                             (baseSpecialSkillCooldownReduction > 0 ? " and reduce its cooldown." : ".") + "<style=cIsVoid>Corrupts all Fuel Cells</style>.";

        public override string DescText => "Add <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> charge of your <style=cIsUtility>Special skill</style>" +
                                            (baseSpecialSkillCooldownReduction > 0 ? " and <style=cIsUtility>reduce</style> its <style=cIsUtility>cooldown</style> by <style=cIsUtility>" + d(baseSpecialSkillCooldownReduction) + "</style>." : ".");

        [ConfigField("Base Special Skill Cooldown Reduction", "Decimal.", 0.2f)]
        public static float baseSpecialSkillCooldownReduction;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.67f)))
            {
                c.Next.Operand = 1f - baseSpecialSkillCooldownReduction;
            }
            else
            {
                Logger.LogError("Failed to apply Lysate Cell Cooldown Reduction hook");
            }
        }
    }
}