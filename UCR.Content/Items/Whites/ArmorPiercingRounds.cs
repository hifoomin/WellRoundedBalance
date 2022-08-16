using MonoMod.Cil;

namespace UltimateCustomRun.Items.Whites
{
    public class ArmorPiercingRounds : ItemBase
    {
        public static float Damage;

        public override string Name => ":: Items : Whites :: Armor Piercing Rounds";
        public override string InternalPickupToken => "bossDamageBonus";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Deal an additional <style=cIsDamage>" + d(Damage) + "</style> damage <style=cStack>(+" + d(Damage) + " per stack)</style> to bosses.";

        public override void Init()
        {
            Damage = ConfigOption(0.2f, "Damage", "Decimal. Per Stack. Vanilla is 0.2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeDamage;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdcR4(0.2f)))
            {
                c.Index += 1;
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Armor Piercing Rounds Damage hook");
            }
        }
    }
}