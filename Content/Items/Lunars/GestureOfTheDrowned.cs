using System;
using BepInEx;
using MonoMod.Cil;
using RoR2.Artifacts;
using RoR2.UI;

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
        [ConfigField("Between Charge CD Stack", 0.065f)]
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
            IL.RoR2.EquipmentSlot.MyFixedUpdate += EnforceDelay;
            On.RoR2.EquipmentSlot.Execute += TriggerDelay;
            On.RoR2.Inventory.SetEquipmentIndex += FullStockCharges;
            On.RoR2.Inventory.SetEquipmentInternal += SetEquipment;
            IL.RoR2.Inventory.UpdateEquipment += UpdateEquipment;
            IL.RoR2.UI.EquipmentIcon.SetDisplayData += UpdateUI;
        }

        private void UpdateUI(ILContext il)
        {
            ILCursor c = new(il);
            VariableDefinition definition = new(Main.WRBAssembly.MainModule.Import(typeof(GestureDelayTracker)));
            c.Body.Variables.Add(definition);
            
            c.Index = 0;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<EquipmentIcon, GestureDelayTracker>>((slot) => {
                if (slot.targetEquipmentSlot) {
                    GestureDelayTracker tracker = slot.targetEquipmentSlot.GetComponent<GestureDelayTracker>();
                    return tracker;
                }

                return null;
            });
            c.Emit(OpCodes.Stloc, definition.Index);

            c.GotoNext(MoveType.After,
                x => x.MatchLdarg(1),
                x => x.MatchLdfld(typeof(EquipmentIcon.DisplayData), nameof(EquipmentIcon.DisplayData.stock)),
                x => x.MatchLdcI4(0)
            );
            c.Index--;
            c.Emit(OpCodes.Ldloc, definition.Index);
            c.EmitDelegate<Func<int, GestureDelayTracker, int>>((input, tracker) => {
                if (tracker) {
                    return tracker.isEquipmentAllowed ? 1 : 0;
                }

                return input;
            });

            c.GotoNext(MoveType.After,
                x => x.MatchCallOrCallvirt(typeof(EquipmentIcon.DisplayData), "get_showCooldown")
            );

            c.Emit(OpCodes.Ldloc, definition.Index);
            c.EmitDelegate<Func<bool, GestureDelayTracker, bool>>((input, tracker) => {
                if (tracker) {
                    return !tracker.isEquipmentAllowed;
                }

                return input;
            });

            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(1),
                x => x.MatchLdfld(typeof(EquipmentIcon.DisplayData), nameof(EquipmentIcon.DisplayData.cooldownValue)),
                x => x.MatchLdarg(0)
            );

            int index = c.Index;
            c.GotoNext(MoveType.After, x => x.MatchBeq(out _));
            ILLabel label = c.MarkLabel();

            c.Index = index;

            c.Emit(OpCodes.Ldloc, definition.Index);
            c.EmitDelegate<Func<GestureDelayTracker, bool>>((tracker) => {
                if (tracker) {
                    return tracker.isEquipmentAllowed;
                }
                
                return true;
            });
            c.Emit(OpCodes.Brfalse_S, label);

            c.GotoNext(MoveType.Before, 
                x => x.MatchLdfld(typeof(EquipmentIcon.DisplayData), nameof(EquipmentIcon.DisplayData.cooldownValue)),
                x => x.MatchLdcI4(1),
                x => x.MatchLdcI4(out _)
            );

            c.Index++;
            c.Emit(OpCodes.Ldloc, definition.Index);
            c.EmitDelegate<Func<int, GestureDelayTracker, int>>((input, tracker) => {
                if (tracker) {
                    return (int)tracker.stopwatch;
                }

                return input;
            });

            Debug.Log(il);
        }

        private void UpdateEquipment(ILContext il)
        {
            ILCursor c = new(il);

            c.TryGotoNext(MoveType.Before, x => x.MatchStloc(1));

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<byte, Inventory, byte>>((input, inv) => {
                int c = inv.GetItemCount(RoR2Content.Items.AutoCastEquipment);

                if (c > 0) {
                    input += (byte)(baseCharges + ((c - 1) * stackCharges));
                }

                return input;
            });

            c.TryGotoNext(MoveType.After,
                x => x.MatchLdloc(5),
                x => x.MatchLdcI4(1)
            );

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<int, Inventory, int>>((input, inv) => {
                int c = inv.GetItemCount(RoR2Content.Items.AutoCastEquipment);

                if (c > 0) {
                    return 0;
                }

                return input;
            });
        }

        private bool SetEquipment(On.RoR2.Inventory.orig_SetEquipmentInternal orig, Inventory self, EquipmentState equipmentState, uint slot)
        {
            return orig(self, equipmentState, slot);
        }

        private void FullStockCharges(On.RoR2.Inventory.orig_SetEquipmentIndex orig, Inventory self, EquipmentIndex newEquipmentIndex)
        {
            orig(self, newEquipmentIndex);

            if (self.GetItemCount(RoR2Content.Items.AutoCastEquipment) > 0) {
                int added = baseCharges + ((self.GetItemCount(RoR2Content.Items.AutoCastEquipment) - 1) * stackCharges);
                self.RestockEquipmentCharges(self.activeEquipmentSlot, 1 + added);
            }
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

            c.TryGotoNext(MoveType.Before, x => x.MatchStloc(0));
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<bool, EquipmentSlot, bool>>((input, self) => {
                if (self.GetComponent<GestureDelayTracker>()) {
                    return self.GetComponent<GestureDelayTracker>().isEquipmentAllowed && input;
                }

                return input;
            });
        }

        private void Destroy(On.RoR2.Inventory.orig_UpdateEquipment orig, Inventory self)
        {
            orig(self);

            if (self.GetItemCount(RoR2Content.Items.AutoCastEquipment) > 0) {
                if (self.GetEquipment(self.activeEquipmentSlot).equipmentDef && self.GetEquipment(self.activeEquipmentSlot).charges <= 0) {
                    self.SetEquipmentIndex(EquipmentIndex.None);
                    if (!self.GetComponent<CharacterMaster>().bodyInstanceObject) {
                        return;
                    }
                    AkSoundEngine.PostEvent(Events.Play_env_vase_shatter, self.GetComponent<CharacterMaster>().bodyInstanceObject);
                    EffectManager.SimpleEffect(Utils.Paths.GameObject.ShieldBreakEffect.Load<GameObject>(), self.GetComponent<CharacterMaster>().bodyInstanceObject.transform.position, Quaternion.identity, false);
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
                int added = baseCharges + ((self.inventory.GetItemCount(RoR2Content.Items.AutoCastEquipment) - 1) * stackCharges);
                self.inventory.RestockEquipmentCharges(self.inventory.activeEquipmentSlot, 1 + added);
            }

            if (gestures <= 0 && gestureDelayTracker) {
                GameObject.Destroy(gestureDelayTracker);
                return;
            }

            if (!gestureDelayTracker) return;

            EquipmentDef def = EquipmentCatalog.GetEquipmentDef(self.inventory.GetEquipmentIndex());

            if (!def) {
                gestureDelayTracker.stopwatch = 0f;
                return;
            }

            float scale = 1f * (baseCd * Mathf.Pow(stackCd, gestures - 1));
            gestureDelayTracker.delay = def.cooldown * scale;
        }

        private class GestureDelayTracker : MonoBehaviour {
            public float stopwatch;
            public float delay;
            public bool isEquipmentAllowed => stopwatch <= 0f;
            public bool didWeFire = false;
            public void FixedUpdate() {
                if (stopwatch < 0f) {
                    stopwatch = 0f;
                }
                else {
                    stopwatch -= Time.fixedDeltaTime;
                }

                if (didWeFire) {
                    stopwatch = delay;
                    didWeFire = false;
                }
            }
        }
    }
}