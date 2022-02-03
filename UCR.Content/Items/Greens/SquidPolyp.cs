using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class SquidPolyp : Based
    {
        public static int aspd;
        public static int dur;

        public override string Name => ":: Items :: Greens :: Squid Polyp";
        public override string InternalPickupToken => "squidTurret";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Activating an interactable summons a <style=cIsDamage>Squid Turret</style> that attacks nearby enemies at <style=cIsDamage>100% <style=cStack>(+100% per stack)</style> attack speed</style>. Lasts <style=cIsUtility>30</style> seconds.";


        public override void Init()
        {
            
            aspd = ConfigOption(10, "Attack Speed Item", "Per Stack. Vanilla is Attack Speed Item * 10 = 100%");
            dur = ConfigOption(30, "Lifetime", "Vanilla is 30");
            base.Init();
        }

        public override void Hooks()
        {
            // IL.RoR2.GlobalEventManager.OnInteractionBegin += SquidPolyp.ChangeAS;
            // IL.RoR2.GlobalEventManager.OnInteractionBegin += SquidPolyp.ChangeLifetime;
        }
        public static void ChangeLifetime(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "HealthDecay"),
                x => x.MatchLdcI4(30)
            );
            c.Index += 1;
            c.Next.Operand = dur;
        }

        public static void ChangeAS(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterMaster>("get_inventory"),
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "BoostAttackSpeed"),
                x => x.MatchLdcI4(10)
            );
            c.Index += 2;
            c.Next.Operand = aspd;
        }
    }
}
