namespace WellRoundedBalance.Mechanics.Teleporter
{
    internal class KillChargeIncrease : MechanicBase<KillChargeIncrease>
    {
        public override string Name => ":: Mechanics ::::::::::::::::: Teleporter Kill Charge Increase";

        [ConfigField("Base On Kill Teleporter Charge Percent Increase", "Decimal.", 0.02f)]
        public static float baseOnKillTeleporterChargePercentIncrease;

        [ConfigField("Base Max Health Scalar", "2100 Base Max Health Enemy will give this much charge multiplied by the base.", 5f)]
        public static float baseMaxHealthScalar;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RoR2.TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport report)
        {
            var attackerBody = report.attackerBody;
            if (!attackerBody)
            {
                return;
            }

            var victimBody = report.victimBody;
            if (!victimBody)
            {
                return;
            }

            if (!victimBody.master)
            {
                return;
            }

            if (!TeleporterInteraction.instance)
            {
                return;
            }

            if (!TeleporterInteraction.instance.holdoutZoneController)
            {
                return;
            }

            var holdoutZoneController = TeleporterInteraction.instance.holdoutZoneController;
            if (holdoutZoneController.GetComponent<TeleporterChargeIncreaseController>() == null)
            {
                return;
            }

            if (holdoutZoneController.IsBodyInChargingRadius(attackerBody))
            {
                holdoutZoneController.GetComponent<TeleporterChargeIncreaseController>().AddCharge(victimBody);
            }
        }

        private void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction teleporterInteraction)
        {
            if (teleporterInteraction.GetComponent<TeleporterChargeIncreaseController>() == null)
            {
                teleporterInteraction.gameObject.AddComponent<TeleporterChargeIncreaseController>();
            }
        }
    }

    public class TeleporterChargeIncreaseController : MonoBehaviour
    {
        public HoldoutZoneController holdoutZoneController;

        public void Start()
        {
            holdoutZoneController = gameObject.GetComponent<HoldoutZoneController>();
        }

        public void AddCharge(CharacterBody victimBody)
        {
            if (holdoutZoneController == null)
            {
                return;
            }

            holdoutZoneController.charge += Util.Remap(victimBody.baseMaxHealth, 0f, 2100f, 0f, KillChargeIncrease.baseOnKillTeleporterChargePercentIncrease * KillChargeIncrease.baseMaxHealthScalar);
        }
    }
}