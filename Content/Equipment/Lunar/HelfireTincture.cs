using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Equipment.Lunar
{
    public class HelfireTincture : EquipmentBase<HelfireTincture>
    {
        public override string Name => ":: Equipment ::: Helfire Tincture";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.BurnNearby;

        public override string PickupText => "Ignite everything nearby... <color=#FF7F7F>including you and allies.</color>\n";

        public override string DescText => "<style=cIsDamage>Ignite</style> ALL characters within " + range + "m for " + duration + " seconds." +
                                           " Deal <style=cIsDamage>" + Math.Round(selfDamage * 100, 1) + "% of your maximum health per second as burning</style> to yourself," +
                                           " <style=cIsDamage>" + Math.Round(damageToEnemies * 100, 1) + "%</style> to enemies and" +
                                           " <style=cIsDamage>" + Math.Round(damageToAlliesMultiplier * selfDamage * 100, 1) + "%</style> to allies";

        [ConfigField("Cooldown", "", 55f)]
        public static float cooldown;

        [ConfigField("Self Damagee", "Decimal.", 0.05f)]
        public static float selfDamage;

        [ConfigField("Damage to Allies Multiplierr", ".", 1f)]
        public static float damageToAlliesMultiplier;

        [ConfigField("Damage To Enemiess", "Decimal.", 0.75f)]
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
                Logger.LogError("Failed to apply Helfire Tincture Duration hook");
            }
        }

        private void Changes()
        {
            var hel = Utils.Paths.GameObject.HelfireController.Load<GameObject>().GetComponent<HelfireController>();
            hel.gameObject.transform.localScale = new Vector3(range, range, range);
            hel.baseRadius = range;
            hel.dotDuration = burnDuration;
            //hel.interval = FireRate;
            // how the hell does this work, the interval is literally 0.25 in game and it attacks 5x per sec???
            hel.healthFractionPerSecond = selfDamage * 0.2f;
            hel.allyDamageScalar = damageToAlliesMultiplier * selfDamage * 20f;
            hel.enemyDamageScalar = damageToEnemies * 20f; // this is correct, at damageToEnemies being 0.18 it dealt 180k per tick
        }
    }
}