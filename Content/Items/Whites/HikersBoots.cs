using System;

namespace WellRoundedBalance.Items.Whites
{
    public class HikersBoots : ItemBase<HikersBoots>
    {
        public override string Name => ":: Items : Whites :: Hikers Boots";
        public override ItemDef InternalPickup => DLC3Content.Items.CritAtLowerElevation;

        public override string PickupText => "Striking enemies from a higher elevation grants critical strike chance and damage.";

        public override string DescText => $"Striking enemies from a higher elevation grants <style=cIsDamage>+{d(critUp)}</style> <style=cStack>(+{d(critUp)} per stack)</style> <style=cIsDamage>critical strike chance</style> and <style=cIsDamage>+{d(damageUp)}</style> <style=cStack>(+{d(damageUp)} per stack)</style> critical strike damage</style>, up to <style=cIsUtility>10 times</style>. Lasts 10 seconds.";

        [ConfigField("Crit Damage Increase", 0.022f)]
        public static float damageUp;
        [ConfigField("Crit Chance Increase", 0.5f)]
        public static float critUp;

        public override void Init()
        {
            base.Init();
            Utils.Assets.BuffDef.bdCritChanceAndDamage.stackingDisplayMethod = BuffDef.StackingDisplayMethod.Percentage;
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);
            int boots = -1;
            c.TryGotoNext(MoveType.After,
                x => x.MatchLdsfld(typeof(DLC3Content.Buffs), nameof(DLC3Content.Buffs.CritChanceAndDamage)),
                x => x.MatchCallOrCallvirt(out _),
                x => x.MatchStloc(out boots)
            );

            c.TryGotoNext(x => x.MatchLdloc(boots));
            c.Prev.Operand = damageUp;
            c.Index++;
            c.TryGotoNext(x => x.MatchLdloc(boots));
            c.Index++;
            c.EmitDelegate<Func<int, float>>((stack) => {
                return stack * critUp;
            });
        }
    }
}