namespace WellRoundedBalance.Items.Reds
{
    public class SpareDroneParts : ItemBase<SpareDroneParts>
    {
        public override string Name => ":: Items ::: Reds :: Spare Drone Parts";
        public override ItemDef InternalPickup => DLC1Content.Items.DroneWeapons;

        public override string PickupText => "Your drones fire faster, have less cooldowns, shoot missiles, and gain a bonus chaingun.";

        public override string DescText => "Gain <style=cIsDamage>Col. Droneman.</style> Drones gain <style=cIsDamage>+" + d(attackSpeedCdr) + "</style> <style=cStack>(+" + d(attackSpeedCdr) + " per stack)</style> attack speed and cooldown reduction, a <style=cIsDamage>10%</style> chance to fire a <style=cIsDamage>missile</style> on hit, dealing <style=cIsDamage>300%</style> TOTAL damage and an <style=cIsDamage>automatic chain gun</style> that deals <style=cIsDamage>6x" + d(chaingunDamage) + " damage</style>" + (chaingunBounces > 0 ? " and <style=cIsDamage>bounces</style> up to " + chaingunBounces + " times." : ".");

        [ConfigField("Chaingun Bounces", 0)]
        public static int chaingunBounces;

        [ConfigField("Chaingun Damage", "Decimal.", 0.3f)]
        public static float chaingunDamage;

        [ConfigField("Attack Speed and Cooldown Reduction Gain", "Decimal.", 0.3f)]
        public static float attackSpeedCdr;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.DroneWeaponsChainGun.FireChainGun.OnEnter += ChaingunChanges;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(46),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.5f),
                x => x.MatchMul()))
            {
                c.Index += 2;
                c.Next.Operand = attackSpeedCdr;
            }
            else
            {
                Logger.LogError("Failed to apply Spare Drone Parts Attack Speed hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcR4(0.5f),
                    x => x.MatchMul(),
                    x => x.MatchStloc(out _),
                    x => x.MatchLdloc(90)))
            {
                c.Index += 1;
                c.Next.Operand = 1f - attackSpeedCdr;
            }
            else
            {
                Logger.LogError("Failed to apply Spare Drone Parts Cooldown Reduction hook");
            }
        }

        public static void ChaingunChanges(On.EntityStates.DroneWeaponsChainGun.FireChainGun.orig_OnEnter orig, EntityStates.DroneWeaponsChainGun.FireChainGun self)
        {
            self.additionalBounces = chaingunBounces;
            self.damageCoefficient = chaingunDamage;
            orig(self);
        }
    }
}