using MonoMod.Cil;

namespace WellRoundedBalance.Equipment.Lunar
{
    public class HelfireTincture : EquipmentBase
    {
        public override string Name => "::: Equipment ::: Helfire Tincture";
        public override string InternalPickupToken => "burnNearby";

        public override string PickupText => "Ignite everything nearby... <color=#FF7F7F>including you and allies.</color>\n";

        public override string DescText => "<style=cIsDamage>Ignite</style> ALL characters within " + range + "m for " + duration + " seconds. Deal <style=cIsDamage>" + d(selfDamage * 5) + " of your maximum health/second as burning</style> to yourself. The burn is <style=cIsDamage>" + d(Mathf.Abs(damageToAllies - selfDamage / Mathf.Abs(selfDamage))) + "</style>" +
                                           (selfDamage > damageToAllies ? " weaker" : " stronger") +
                                           " on allies, and <style=cIsDamage>" + d(Mathf.Abs(damageToEnemies - selfDamage / Mathf.Abs(selfDamage))) + "</style>" +
                                           (selfDamage > damageToEnemies ? " weaker" : " stronger") +
                                            " on enemies.";

        [ConfigField("Cooldown", "", 55f)]
        public static float cooldown;

        [ConfigField("Self Damage", "Decimal.", 0.007f)]
        public static float selfDamage;

        [ConfigField("Damage to Allies", "Decimal.", 0.0025f)]
        public static float damageToAllies;

        [ConfigField("Damage To Enemies", "Decimal.", 0.2f)]
        public static float damageToEnemies;

        [ConfigField("Range", "", 15f)]
        public static float range;

        [ConfigField("Duration", "", 12f)]
        public static float duration;

        [ConfigField("Burn Duration", "", 3f)]
        public static float burnDuration;

        public override void Init()
        {
            // Interval = ConfigOption(5f, "Fire Rate", "Vanilla is 5);
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireBurnNearby += ChangeDuration;
            Changes();
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(12f)))
            {
                c.Next.Operand = duration;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Helfire Tincture Duration hook");
            }
        }

        private void Changes()
        {
            var hel = Utils.Paths.GameObject.HelfireController.Load<GameObject>().GetComponent<HelfireController>();
            hel.baseRadius = range;
            hel.dotDuration = burnDuration;
            //hel.interval = FireRate;
            // how the hell does this work, the interval is literally 0.25 in game and it attacks 5x per sec???
            hel.healthFractionPerSecond = selfDamage * 10f;
            hel.allyDamageScalar = 1f / (selfDamage * 10f / (damageToAllies * 10f));
            hel.enemyDamageScalar = damageToEnemies * 100f;
        }
    }
}