using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class UnstableTeslaCoil : ItemBase<UnstableTeslaCoil>
    {
        public override string Name => ":: Items ::: Reds :: Unstable Tesla Coil";
        public override ItemDef InternalPickup => RoR2Content.Items.ShockNearby;

        public override string PickupText => "Shock all nearby enemies every 0.5 seconds.";

        public override string DescText => "Fire out <style=cIsDamage>lightning</style> that hits <style=cIsDamage>3</style> <style=cStack>(+2 per stack)</style> enemies for <style=cIsDamage>" + d(baseDamagePerTick * 2) + "</style> damage per second. The Tesla Coil switches off every <style=cIsDamage>10 seconds</style>.";

        [ConfigField("Base Damage Per Tick", "Formula for DPS: Base Damage Per Tick * 2", 1.5f)]
        public static float baseDamagePerTick;

        [ConfigField("Range", 25f)]
        public static float range;

        [ConfigField("Proc Coefficient", 0.3f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.ShockNearbyBodyBehavior.FixedUpdate += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(2f)))
            {
                c.Next.Operand = 1.5f;
            }
            else
            {
                Logger.LogError("Failed to apply Unstable Tesla Coil Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.3f)))
            {
                c.Next.Operand = procCoefficient;
            }
            else
            {
                Logger.LogError("Failed to apply Unstable Tesla Coil Proc Coefficient hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(35f)))
            {
                c.Next.Operand = 25f;
            }
            else
            {
                Logger.LogError("Failed to apply Unstable Tesla Coil Range hook");
            }
        }
    }
}