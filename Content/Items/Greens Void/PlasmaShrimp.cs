using MonoMod.Cil;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class PlasmaShrimp : ItemBase<PlasmaShrimp>
    {
        public override string Name => ":: Items :::::: Voids :: Plasma Shrimp";
        public override ItemDef InternalPickup => DLC1Content.Items.MissileVoid;

        public override string PickupText => "While you have shield, fire missiles on every hit. <style=cIsVoid>Corrupts all AtG Missile Mk. 1s</style>.";
        public override string DescText => "Gain a <style=cIsHealing>shield</style> equal to <style=cIsHealing>" + d(percentShield) + "</style> of your maximum health. While you have a <style=cIsHealing>shield</style>, hitting an enemy fires a missile that deals <style=cIsDamage>" + d(totalDamage) + "</style> <style=cStack>(+" + d(totalDamage) + " per stack)</style> TOTAL damage. <style=cIsVoid>Corrupts all AtG Missile Mk. 1s</style>.";

        [ConfigField("TOTAL Damage", "Decimal.", 0.2f)]
        public static float totalDamage;

        [ConfigField("Percent Shield", "Decimal.", 0.12f)]
        public static float percentShield;

        [ConfigField("Proc Chance", 0f)]
        public static float procChance;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.4f)))
            {
                c.Next.Operand = totalDamage;
            }
            else
            {
                Logger.LogError("Failed to apply Plasma Shrimp Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.2f),
                    x => x.MatchStfld<RoR2.Orbs.GenericDamageOrb>("procCoefficient")))
            {
                c.Next.Operand = procChance * globalProc;
            }
            else
            {
                Logger.LogError("Failed to apply Plasma Shrimp Proc Coefficient hook");
            }
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt<CharacterBody>("get_maxHealth"),
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Index += 1;
                c.Next.Operand = percentShield;
            }
            else
            {
                Logger.LogError("Failed to apply Plasma Shrimp Shield hook");
            }
        }
    }
}