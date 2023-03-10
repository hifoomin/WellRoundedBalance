using MonoMod.Cil;

namespace WellRoundedBalance.Equipment
{
    public class BlastShower : EquipmentBase
    {
        public override string Name => ":: Equipment :: Blast Shower";
        public override string InternalPickupToken => "cleanse";

        public override string PickupText => "Cleanse all negative effects.";

        public override string DescText => "<style=cIsUtility>Cleanse</style> all negative effects. Includes debuffs, damage over time, and nearby projectiles.";

        [ConfigField("Cooldown", "", 13f)]
        public static float cooldown;

        [ConfigField("Projectile Removal Range", "", 20f)]
        public static float projectileRemovalRange;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Util.CleanseBody += ChangeRemovalRange;

            var BlastShower = Utils.Paths.EquipmentDef.Cleanse.Load<EquipmentDef>();
            BlastShower.cooldown = cooldown;
        }

        private void ChangeRemovalRange(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(6f)))
            {
                c.Next.Operand = projectileRemovalRange;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Blast Shower Projectile Removal hook");
            }
        }
    }
}