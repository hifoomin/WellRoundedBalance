using System;
using MonoMod.Cil;
using RoR2.Artifacts;

namespace WellRoundedBalance.Items.Lunars
{
    public class GestureOfTheDrowned : ItemBase<GestureOfTheDrowned>
    {
        public override string Name => ":: Items ::::: Lunars :: Gesture of The Drowned";
        public override ItemDef InternalPickup => RoR2Content.Items.AutoCastEquipment;

        public override string PickupText => "Gain a large amount of equipment charges. Using all charges <style=cDeath>destroys</style> your equipment.";
        public override string DescText => $"Gain <style=cIsDamage>{baseCharges}</style> <style=cStack>(+{stackCharges} per stack)</style> <style=cIsUtility>additional equipment charges</style>, with a <style=cDeath>delay</style> equal to {baseCd * 100f}% <style=cStack>(+{stackCd * 100f}% per stack) of the cooldown between uses. <style=cDeath>Charges do not regenerate, and consuming them all destroys the equipment.";

        [ConfigField("Base Charge Gain", 6)]
        public static int baseCharges;
        [ConfigField("Stack Charge Gain", 3)]
        public static int stackCharges;
        [ConfigField("Between Charge CD Base", 0.1f)]
        public static float baseCd;
        [ConfigField("Between Charge CD Stack", 0.0065f)]
        public static float stackCd;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Inventory.CalculateEquipmentCooldownScale += Inventory_CalculateEquipmentCooldownScale;
            On.RoR2.CharacterBody.OnInventoryChanged += InvChanged;
            On.RoR2.Inventory.GetEquipmentSlotMaxCharges += AddCharges;
            On.RoR2.Inventory.UpdateEquipment += Destroy;
            IL.RoR2.EquipmentSlot.FixedUpdate += EnforceDelay;
            On.RoR2.EquipmentSlot.Execute += TriggerDelay;
        }

        private void TriggerDelay(On.RoR2.EquipmentSlot.orig_Execute orig, EquipmentSlot self)
        {
            orig(self);

            if (self.GetComponent<GestureDelayTracker>()) {
                self.GetComponent<GestureDelayTracker>().didWeFire = true;
            }
        }

        private void EnforceDelay(ILContext il)
        {
            ILCursor c = new(il);

            c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(RoR2Content.Items), nameof(RoR2Content.Items.AutoCastEquipment)));
            c.Index++;
            c.EmitDelegate<Func<int, int>>((inp) => {
                return 0;
            });

            c.TryGotoNext(MoveType.After, x => x.MatchStloc(0));
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<bool, EquipmentSlot, bool>>((input, self) => {
                if (self.GetComponent<GestureDelayTracker>()) {
                    return self.GetComponent<GestureDelayTracker>().isEquipmentAllowed;
                }

                return input;
            });
        }

        private void Destroy(On.RoR2.Inventory.orig_UpdateEquipment orig, Inventory self)
        {
            orig(self);

            if (self.GetItemCount(RoR2Content.Items.AutoCastEquipment) > 0) {
                if (self.GetEquipment(self.activeEquipmentSlot).charges <= 0) {
                    self.SetEquipmentIndex(EquipmentIndex.None);
                    AkSoundEngine.PostEvent(Events.Play_item_proc_delicateWatch_break, self.gameObject);
                }
            }
        }

        private int AddCharges(On.RoR2.Inventory.orig_GetEquipmentSlotMaxCharges orig, Inventory self, byte slot)
        {
            int c = orig(self, slot);

            if (self.GetItemCount(RoR2Content.Items.AutoCastEquipment) > 0) {
                int added = baseCharges + ((self.GetItemCount(RoR2Content.Items.AutoCastEquipment) - 1) * stackCharges);

                return c + added;
            }

            return c;
        }

        private float Inventory_CalculateEquipmentCooldownScale(On.RoR2.Inventory.orig_CalculateEquipmentCooldownScale orig, Inventory self)
        {
            if (self.GetItemCount(RoR2Content.Items.AutoCastEquipment) > 0) {
                return 0f;
            }

            return orig(self);
        }

        private void InvChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);

            if (!self.inventory) {
                return;
            }

            int gestures = self.inventory.GetItemCount(RoR2Content.Items.AutoCastEquipment);
            GestureDelayTracker gestureDelayTracker = self.GetComponent<GestureDelayTracker>();

            if (gestures > 0 && !gestureDelayTracker) {
                gestureDelayTracker = self.gameObject.AddComponent<GestureDelayTracker>();
            }

            if (gestures <= 0 && gestureDelayTracker) {
                GameObject.Destroy(gestureDelayTracker);
                return;
            }

            EquipmentDef def = EquipmentCatalog.GetEquipmentDef(self.inventory.GetEquipmentIndex());

            if (!def) {
                return;
            }

            float delay = GetHyperbolic(baseCd, 1f, stackCd);
            float cd = def.cooldown * delay;
            gestureDelayTracker.delay = cd;
        }

        private class GestureDelayTracker : MonoBehaviour {
            public float stopwatch;
            public float delay;
            public bool isEquipmentAllowed => stopwatch <= 0f;
            public bool didWeFire = false;
            public void FixedUpdate() {
                stopwatch -= Time.fixedDeltaTime;

                if (didWeFire) {
                    stopwatch = delay;
                    didWeFire = false;
                }
            }
        }
    }
}