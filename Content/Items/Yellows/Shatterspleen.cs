using Inferno.Stat_AI;
using WellRoundedBalance.Misc;

namespace WellRoundedBalance.Items.Yellows
{
    public class Shatterspleen : ItemBase<Shatterspleen>
    {
        public override string Name => ":: Items :::: Yellows :: Shatterspleen";
        public override ItemDef InternalPickup => RoR2Content.Items.BleedOnHitAndExplode;

        public override string PickupText => "Critical strikes always bleed enemies. Bleeding enemies now explode.";

        public override string DescText => "<style=cIsDamage>Critical Strikes bleed</style> enemies up to <style=cIsDamage>" + baseBleedCapPerTarget + "</style>" +
            (bleedCapPerTargetPerStack > 0 ? " <style=cStack>(+" + bleedCapPerTargetPerStack + " per stack)</style>" : "") +
            " times for <style=cIsDamage>240%</style> base damage. <style=cIsDamage>Bleeding</style> enemies <style=cIsDamage>explode</style> on death for <style=cIsDamage>" + d(baseExplosionDamage) + "</style> <style=cStack>(+" + d(baseExplosionDamage) + " per stack)</style> damage.";

        [ConfigField("Base Explosion Damage", "Decimal.", 2f)]
        public static float baseExplosionDamage;

        [ConfigField("Base Bleed Cap Per Target", "", 16)]
        public static int baseBleedCapPerTarget;

        [ConfigField("Bleed Cap Per Target Per Stack", "", 0)]
        public static int bleedCapPerTargetPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += Changes;
            RecalculateEvent.RecalculateBleedCap += (object sender, RecalculateEventArgs args) =>
            {
                if (args.BleedCap)
                {
                    var body = args.BleedCap.body;
                    if (body)
                    {
                        var inventory = args.BleedCap.body.inventory;
                        if (inventory)
                        {
                            var stack = inventory.GetItemCount(RoR2Content.Items.BleedOnHitAndExplode);
                            if (stack > 0)
                            {
                                args.BleedCap.bleedCapAdd += baseBleedCapPerTarget + bleedCapPerTargetPerStack * (stack - 1);
                            }
                        }
                    }
                }
            };
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(4f),
                    x => x.MatchLdcI4(1),
                    x => x.MatchLdloc(out _)))
            {
                c.Next.Operand = baseExplosionDamage;
            }
            else
            {
                Logger.LogError("Failed to apply Shatterspleen Base Explosion Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.15f),
                    x => x.MatchLdcI4(1),
                    x => x.MatchLdloc(out _)))
            {
                c.Next.Operand = 0f;
            }
            else
            {
                Logger.LogError("Failed to apply Shatterspleen Percent Explosion Damage hook");
            }
        }
    }
}