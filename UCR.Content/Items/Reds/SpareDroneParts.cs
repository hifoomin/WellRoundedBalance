using MonoMod.Cil;

namespace UltimateCustomRun.Items.Reds
{
    public class SpareDroneParts : ItemBase
    {
        public static float AttackSpeed;
        public static float CooldownReduction;
        public static int ChaingunBounces;
        public static float ChaingunDamage;
        public static float MissileChance;
        public static float MissileDamage;

        public override string Name => ":: Items ::: Reds :: Spare Drone Parts";
        public override string InternalPickupToken => "droneWeapons";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Gain <style=cIsDamage>Col. Droneman</style>. " +
                                           (AttackSpeed > 0 ? "\nDrones gain <style=cIsDamage>+" + d(AttackSpeed) + "</style> <style=cStack>(+" + d(AttackSpeed) + " per stack)</style> attack speed" : "") +
                                           (CooldownReduction > 0 || MissileChance > 0 || MissileDamage > 0 || ChaingunDamage > 0 ? " and" : ".") +
                                           (CooldownReduction > 0 ? " <style=cIsDamage>+" + d(CooldownReduction) + "</style> <style=cStack>(+" + d(CooldownReduction) + " per stack)</style> cooldown reduction," : "") +
                                           (MissileChance > 0 || MissileDamage > 0 ? " <style=cIsDamage>" + MissileChance + "%</style> chance to fire a <style=cIsDamage>missile</style> on hit, that deals <style=cIsDamage>" + d(MissileDamage) + "</style> TOTAL damage" : "") +
                                           (CooldownReduction > 0 || MissileChance > 0 || MissileDamage > 0 || ChaingunDamage > 0 ? " and" : ".") +
                                           (ChaingunDamage > 0 ? (ChaingunBounces > 0 ? " an <style=cIsDamage>automatic chain gun</style> that deals <style=cIsDamage>6x" + d(ChaingunDamage) + "</style> damage, bouncing to <style=cIsDamage>2</style> enemies." : "an <style=cIsDamage>automatic chain gun</style> that deals <style=cIsDamage>6x" + d(ChaingunDamage) + "</style>damage") : ".");

        public override void Init()
        {
            AttackSpeed = ConfigOption(0.5f, "Attack Speed Increase", "Per Stack. Vanilla is 0.5");
            CooldownReduction = ConfigOption(0.5f, "Cooldown Reduction", "Decimal. Per Stack. Vanilla is 0.5");
            ChaingunBounces = ConfigOption(1, "Additional Chaingun Bounces", "Vanilla is 1");
            ChaingunDamage = ConfigOption(1f, "Chaingun Damage", "Decimal. Vanilla is 1");
            MissileChance = ConfigOption(10f, "Missile Chance", "Vanilla is 10");
            MissileDamage = ConfigOption(3f, "Missile TOTAL Damage", "Decimal. Vanilla is 3");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.DroneWeaponsChainGun.FireChainGun.OnEnter += ChaingunChanges;
            IL.RoR2.DroneWeaponsBoostBehavior.OnEnemyHit += MissileChanges;
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
        }

        public static void MissileChanges(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(10f)))
            {
                c.Next.Operand = MissileChance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Spare Drone Parts Missile Chance hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(3f)))
            {
                c.Next.Operand = MissileDamage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Spare Drone Parts Missile Damage hook");
            }
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(46),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.5f),
                x => x.MatchMul()))
            {
                c.Index += 2;
                c.Next.Operand = AttackSpeed;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Spare Drone Parts Attack Speed hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(87),
                    x => x.MatchLdcR4(0.5f),
                    x => x.MatchMul(),
                    x => x.MatchStloc(87)))
            {
                c.Index += 1;
                c.Next.Operand = 1f - CooldownReduction;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Spare Drone Parts Cooldown Reduction hook");
            }
        }

        public static void ChaingunChanges(On.EntityStates.DroneWeaponsChainGun.FireChainGun.orig_OnEnter orig, EntityStates.DroneWeaponsChainGun.FireChainGun self)
        {
            self.damageCoefficient = ChaingunDamage;
            self.additionalBounces = ChaingunBounces;
            orig(self);
        }
    }
}