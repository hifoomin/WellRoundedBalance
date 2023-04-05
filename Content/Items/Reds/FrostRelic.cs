namespace WellRoundedBalance.Items.Reds
{
    public class FrostRelic : ItemBase<FrostRelic>
    {
        public override string Name => ":: Items ::: Reds :: Frost Relic";
        public override ItemDef InternalPickup => RoR2Content.Items.Icicle;

        public override string PickupText => "Killing enemies surrounds you with an ice storm.";

        public override string DescText => "Killing an enemy surrounds you with an <style=cIsDamage>ice storm</style> that deals <style=cIsDamage>" + d(baseDamagePerTick * 4) + " damage per second</style> and <style=cIsUtility>slows</style> enemies by <style=cIsUtility>45%</style> for <style=cIsUtility>1.5s</style>. The storm <style=cIsDamage>grows with every kill</style>, increasing its radius by <style=cIsDamage>2m</style>. Stacks up to <style=cIsDamage>18m</style> <style=cStack>(+12m per stack)</style>.";

        [ConfigField("Base Damage Per Tick", "Formula for DPS: Base Damage Per Tick * 4", 2.25f)]
        public static float baseDamagePerTick;

        [ConfigField("Proc Coefficient", 0.25f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var relic = Utils.Paths.GameObject.IcicleAura.Load<GameObject>();
            var icicleAuraController = relic.GetComponent<IcicleAuraController>();
            icicleAuraController.icicleDamageCoefficientPerTick = baseDamagePerTick;
            icicleAuraController.icicleProcCoefficientPerTick = procCoefficient * globalProc;
        }
    }
}