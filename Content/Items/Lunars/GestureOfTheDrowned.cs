using MonoMod.Cil;
using RoR2.Artifacts;

namespace WellRoundedBalance.Items.Lunars
{
    public class GestureOfTheDrowned : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Gesture of The Drowned";
        public override string InternalPickupToken => "autoCastEquipment";

        public override string PickupText => "Reduce Equipment cooldown... <color=#FF7F7F>BUT it automatically activates and randomizes.</color>";
        public override string DescText => "<style=cIsUtility>Reduce Equipment cooldown</style> by <style=cIsUtility>" + d(baseEquipmentCooldownReduction) + "</style>. Forces your Equipment to <style=cIsUtility>activate</style> and <style=cIsUtility>randomize</style> whenever it is off <style=cIsUtility>cooldown</style>.";

        [ConfigField("Base Equipment Cooldown Reduction", 0.3f)]
        public static float baseEquipmentCooldownReduction;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Inventory.CalculateEquipmentCooldownScale += Inventory_CalculateEquipmentCooldownScale;
            EquipmentSlot.onServerEquipmentActivated += EquipmentSlot_onServerEquipmentActivated;
        }

        private void EquipmentSlot_onServerEquipmentActivated(EquipmentSlot equipmentSlot, EquipmentIndex equipmentIndex)
        {
            var body = equipmentSlot.characterBody;
            if (body)
            {
                var inventory = body.inventory;
                if (inventory)
                {
                    var stack = inventory.GetItemCount(RoR2Content.Items.AutoCastEquipment);
                    if (stack > 0)
                    {
                        var randomEquipment = EnigmaArtifactManager.GetRandomEquipment(EnigmaArtifactManager.serverActivationEquipmentRng, (int)equipmentIndex);
                        equipmentSlot.characterBody.inventory.SetEquipmentIndex(randomEquipment);
                    }
                }
            }
        }

        private void Inventory_CalculateEquipmentCooldownScale(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.5f),
                    x => x.MatchLdcR4(0.85f)))
            {
                c.Next.Operand = 1 - baseEquipmentCooldownReduction;
                c.Index += 1;
                c.Next.Operand = 1f;
            }
            else
            {
                Logger.LogError("Failed to apply Gesture Of The Drowned Equipment Cooldown Reduction hook");
            }
        }
    }
}