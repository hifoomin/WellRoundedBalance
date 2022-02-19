using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class Gasoline : ItemBase
    {
        public static float expdamage;
        public static float burndamage;
        public static float range;
        public static float rangestack;

        public override string Name => ":: Items : Whites :: Gasoline";
        public override string InternalPickupToken => "igniteOnKill";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public static float actualRange = range + rangestack;
        public static float actualBurnStackdamage = burndamage / 2;

        public override string DescText => "Killing an enemy <style=cIsDamage>ignites</style> all enemies within <style=cIsDamage>" + actualRange + "m</style> <style=cStack>(+" + rangestack + "m per stack)</style> for <style=cIsDamage>" + d(expdamage) + "</style> base damage. Additionally, enemies <style=cIsDamage>burn</style> for <style=cIsDamage>" + d(burndamage) + "</style> <style=cStack>(+" + d(actualBurnStackdamage) + " per stack)</style> base damage.";
        public override void Init()
        {
            expdamage = ConfigOption(1.5f, "Damage Coefficient", "Decimal. Vanilla is 1.5");
            burndamage = ConfigOption(1.5f, "Damage Coefficient", "Decimal. Per Stack. Vanilla is 1.5");
            range = ConfigOption(12f, "Range", "Vanilla is 12");
            rangestack = ConfigOption(4f, "Range", "Vanilla is 4");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += ChangeBurnDamage;
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += ChangeExplosionDamage;
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += ChangeRadius;
        }
        public static void ChangeExplosionDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchAdd(),
                x => x.MatchStloc(1),
                x => x.MatchLdcR4(1.5f)
            );
            c.Index += 2;
            c.Next.Operand = expdamage;
        }

        public static void ChangeBurnDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.5f),
                x => x.MatchLdcR4(1.5f)
            );
            c.Next.Operand = burndamage;
            c.Index += 1;
            c.Next.Operand = burndamage;
            // how the FUCK does this item work lmao
            // with this, it'd be 300% (+150% per stack) of burn instead of the 150% (+75% per stack) as listed on the wiki...
            // is burns damage hardcoded and gasoline just extends the DoT duration for more ticks?
        }
        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8f),
                x => x.MatchLdcR4(4f)
            );
            c.Next.Operand = range - rangestack;
            c.Index += 1;
            c.Next.Operand = rangestack;
        }
    }
}
