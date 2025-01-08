/*
using System.Collections;

namespace WellRoundedBalance.Mechanics.Teleporter
{
    internal class KillRadiusIncrease : MechanicBase<KillRadiusIncrease>
    {
        public override string Name => ":: Mechanics ::::::::::::::::: Teleporter Kill Radius Increase";

        [ConfigField("On Kill Teleporter Radius Percent Increase", "Decimal.", 0.06f)]
        public static float onKillRadiusPercentGain;

        [ConfigField("Maximum Teleporter Radius Percent Increase", "Decimal.", 1.25f)]
        public static float maximumRadius;

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
            if (holdoutZoneController.GetComponent<TeleporterRadiusIncreaseController>() == null)
            {
                return;
            }

            if (holdoutZoneController.IsBodyInChargingRadius(attackerBody))
            {
                holdoutZoneController.GetComponent<TeleporterRadiusIncreaseController>().ExpandRadius();
            }
        }

        private void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction teleporterInteraction)
        {
            if (teleporterInteraction.GetComponent<TeleporterRadiusIncreaseController>() == null)
            {
                teleporterInteraction.gameObject.AddComponent<TeleporterRadiusIncreaseController>();
            }
        }
    }

    public class TeleporterRadiusIncreaseController : MonoBehaviour
    {
        public float maxRadius;
        public HoldoutZoneController holdoutZoneController;
        public bool initialized = false;

        public void Start()
        {
            holdoutZoneController = gameObject.GetComponent<HoldoutZoneController>();
            StartCoroutine(Init());
        }

        public IEnumerator Init()
        {
            yield return new WaitForSeconds(holdoutZoneController.radiusSmoothTime * 3f);
            maxRadius = holdoutZoneController.currentRadius * KillRadiusIncrease.maximumRadius;
            initialized = true;
        }

        public void ExpandRadius()
        {
            if (holdoutZoneController == null || !initialized)
            {
                return;
            }

            holdoutZoneController.currentRadius = Mathf.Min(maxRadius, holdoutZoneController.currentRadius + (holdoutZoneController.currentRadius * KillRadiusIncrease.onKillRadiusPercentGain));
        }
    }
}
*/