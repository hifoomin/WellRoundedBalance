using MonoMod.Cil;
using System;
using RoR2.UI;
using UnityEngine.UI;

namespace WellRoundedBalance.Interactables
{
    internal class VoidCradle : InteractableBase
    {
        public override string Name => ":: Interactables : Void Cradle";
        public CostTypeIndex costTypeIndex = (CostTypeIndex)19;
        public CostTypeDef def;
        public GameObject optionPanel;
        public static InteractableSpawnCard vradle;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            GameObject VoidCradle = Utils.Paths.GameObject.VoidChest.Load<GameObject>();
            PurchaseInteraction interaction = VoidCradle.GetComponent<PurchaseInteraction>();
            interaction.costType = costTypeIndex;
            interaction.cost = 0;
            interaction.contextToken = "WRB_VOIDCHEST_CONTEXT";
            VoidCradle.RemoveComponent<ChestBehavior>();
            VoidCradle.AddComponent<NetworkUIPromptController>();
            VoidCradle.AddComponent<PickupIndexNetworker>();
            PickupPickerController controller = VoidCradle.AddComponent<PickupPickerController>();
            controller.cutoffDistance = 10;
            optionPanel = Utils.Paths.GameObject.OptionPickerPanel.Load<GameObject>().InstantiateClone("VoidCradleOptionPicker", false);
            Transform bg = optionPanel.transform.Find("MainPanel").Find("Juice").Find("BG, Colored");
            Transform bgCenter = bg.Find("BG, Colored Center");
            bg.GetComponent<Image>().color = new Color32(237, 127, 205, 255);
            bgCenter.GetComponent<Image>().color = new Color32(237, 127, 205, 255);
            Transform label = optionPanel.transform.Find("MainPanel").Find("Juice").Find("Label");
            label.GetComponent<HGTextMeshProUGUI>().text = "Awaiting Transmutation...";
            controller.panelPrefab = optionPanel;
            LanguageAPI.Add("WRB_VOIDCHEST_CONTEXT", "Open?");
            VoidCradle.AddComponent<CradleManager>();
            VoidCradle.RemoveComponent<ScriptedCombatEncounter>();

            vradle = Utils.Paths.InteractableSpawnCard.iscVoidChest.Load<InteractableSpawnCard>();

            def = new();
            def.buildCostString = delegate (CostTypeDef def, CostTypeDef.BuildCostStringContext c)
            {
                c.stringBuilder.Append("<style=cDeath>10% Curse</style>");
            };

            def.isAffordable = delegate (CostTypeDef def, CostTypeDef.IsAffordableContext c)
            {
                return HasAtLeastOneItem(c.activator.GetComponent<CharacterBody>().inventory);
            };

            def.payCost = delegate (CostTypeDef def, CostTypeDef.PayCostContext c)
            {
            };

            On.RoR2.CostTypeCatalog.Init += (orig) =>
            {
                orig();
                CostTypeCatalog.Register(costTypeIndex, def);
            };

            IL.RoR2.CostTypeCatalog.Init += (il) =>
            {
                ILCursor c = new(il);
                bool found = c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcI4(15)
                );

                if (found)
                {
                    c.Index++;
                    c.EmitDelegate<Func<int, int>>((c) =>
                    {
                        return 20;
                    });
                }
                else
                {
                    Logger.LogError("Failed to apply CostTypeCatalog IL hook");
                }
            };

            On.RoR2.UI.PickupPickerPanel.OnCreateButton += (orig, self, i, button) =>
            {
                orig(self, i, button);
                if (!self.gameObject.name.Contains("VoidCradle"))
                {
                    return;
                }
                TooltipProvider tp = button.gameObject.AddComponent<TooltipProvider>();
                TooltipContent c = new();
                ItemDef def = ItemCatalog.GetItemDef(GetCorruption(self.pickerController.options[i].pickupIndex.itemIndex));
                if (!def)
                {
                    return;
                }
                c.bodyColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.VoidItem);
                c.titleColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.VoidItemDark);
                c.overrideTitleText = "Transmutes into: " + Language.GetString(def.nameToken);
                c.bodyToken = def.descriptionToken;
                tp.SetContent(c);
            };

            On.RoR2.SceneDirector.SelectCard += (orig, self, deck, max) => {
                DirectorCard card = null;
                for (int i = 0; i < 10; i++) {
                    DirectorCard next = orig(self, deck, max);
                    if (next.spawnCard == vradle && ShouldBlockCradles()) {
                        Main.WRBLogger.LogInfo("No players have corruptible items, blocking vradle spawn");
                        continue;
                    }
                    card = next;
                }

                return card == null ? orig(self, deck, max) : card; // failsafe in the event cradles are the literal only thing it can afford (eg. void locus)
            };
        }

        public static bool ShouldBlockCradles() {
            foreach (PlayerCharacterMasterController pmc in PlayerCharacterMasterController.instances) {
                if (pmc.master && HasAtLeastOneItem(pmc.master.inventory)) {
                    return false;
                }
            }

            return true;
        }

        public static bool HasAtLeastOneItem(Inventory inventory)
        {
            foreach (ItemIndex index in inventory.itemAcquisitionOrder)
            {
                if (IsCorruptible(index))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsCorruptible(ItemIndex index)
        {
            if (ItemCatalog.GetItemDef(index).tier == ItemTier.Boss) { // boss items cant be selected by vradles so dont return true 
                return false;
            }
            ItemIndex item = RoR2.Items.ContagiousItemManager.GetTransformedItemIndex(index);
            return item != ItemIndex.None;
        }

        public static ItemIndex GetCorruption(ItemIndex index)
        {
            return RoR2.Items.ContagiousItemManager.GetTransformedItemIndex(index);
        }

        private class CradleManager : MonoBehaviour
        {
            public float timer;
            public float interval = 1f;
            public bool wasDisabled = false;
            public PurchaseInteraction interaction => GetComponent<PurchaseInteraction>();
            public PickupPickerController controller => GetComponent<PickupPickerController>();
            public List<PickupPickerController.Option> options = new();
            public bool hasSet = false;

            private void Start()
            {
                interaction.onPurchase.AddListener(OnPurchase);
                controller.onPickupSelected.AddListener(Corrupt);
            }

            public void Corrupt(int i)
            {
                PickupIndex index = new(i);
                ItemIndex def = index.itemIndex;
                Interactor interactor = interaction.lastActivator;
                CharacterBody body = interactor.GetComponent<CharacterBody>();
                int c = body.inventory.GetItemCount(def);
                body.inventory.RemoveItem(def, c);
                body.inventory.GiveItem(GetCorruption(def), c);
                CharacterMasterNotificationQueue.PushItemTransformNotification(body.master, def, GetCorruption(def), CharacterMasterNotificationQueue.TransformationType.ContagiousVoid);
                interaction.SetAvailable(false);
                float amount = body.healthComponent.fullCombinedHealth * 0.1f;
                float curse = Mathf.RoundToInt(amount / body.healthComponent.fullCombinedHealth * 100f);
                controller.networkUIPromptController.SetParticipantMaster(null);
                for (int j = 0; j < curse; j++)
                {
                    body.AddBuff(RoR2Content.Buffs.PermanentCurse);
                }

<<<<<<< HEAD
                EntityStateMachine machine = GetComponent<EntityStateMachine>();
                if (machine) {
                    machine.SetNextState(new EntityStates.Barrel.Opening());
=======
                EffectManager.SpawnEffect(Utils.Paths.GameObject.ExplodeOnDeathVoidExplosionEffect.Load<GameObject>(), new EffectData
                {
                    origin = transform.position,
                    scale = 3f
                }, true);

                gameObject.SetActive(false);

                var playerCharacterMasterController = body.master.GetComponent<PlayerCharacterMasterController>();
                if (playerCharacterMasterController)
                {
                    var networkUser = playerCharacterMasterController.networkUser;
                    if (networkUser)
                    {
                        var localUser = networkUser.localUser;
                        if (localUser != null)
                        {
                            localUser.userProfile.DiscoverPickup(ItemCatalog.GetItemDef(GetCorruption(def)).GetPickupIndex());
                            // Logger.LogError("getcorruption if itemindex is " + GetCorruption(def));
                            // Logger.LogError("getitemdef of getcorruption of itemindex is " + ItemCatalog.GetItemDef(GetCorruption(def)));
                            // Logger.LogError("getpickupindex of getitemdef of getcorruption of itemindex is " + ItemCatalog.GetItemDef(GetCorruption(def)).GetPickupIndex());
                        }
                    }
>>>>>>> 12bc3b7c0ffd56caaf2e2b9f99807b85c66ac14c
                }
            }

            public void OnPurchase(Interactor interactor)
            {
                if (interactor.GetComponent<CharacterBody>())
                {
                    CharacterBody body = interactor.GetComponent<CharacterBody>();
                    int c = 0;
                    foreach (ItemIndex index in body.inventory.itemAcquisitionOrder.OrderBy(x => UnityEngine.Random.value))
                    {
                        if (hasSet) {
                            continue;
                        }
                        if (IsCorruptible(index))
                        {
                            ItemDef def = ItemCatalog.GetItemDef(index);
                            if (def.tier == ItemTier.Boss || c >= 3)
                            {
                                continue;
                            }
                            options.Add(new PickupPickerController.Option
                            {
                                pickupIndex = PickupCatalog.FindPickupIndex(index),
                                available = true
                            });
                            c++;
                        }
                    }

                    if (options.Count >= 1)
                    {
<<<<<<< HEAD
                        hasSet = true;
                        Debug.Log("starting UI");
=======
                        // Debug.Log("starting UI");
>>>>>>> 12bc3b7c0ffd56caaf2e2b9f99807b85c66ac14c
                        controller.SetOptionsInternal(options.ToArray());
                        controller.SetOptionsServer(options.ToArray());
                        controller.OnInteractionBegin(interactor);
                    }
                    interaction.SetAvailableTrue();
                }
            }

            public void FixedUpdate()
            {
                timer += Time.fixedDeltaTime;
                if (timer >= interval)
                {
                    var teleporter = TeleporterInteraction.instance;
                    if (teleporter && teleporter.activationState == TeleporterInteraction.ActivationState.Charged && !wasDisabled)
                    {
                        EffectManager.SpawnEffect(Utils.Paths.GameObject.ExplodeOnDeathVoidExplosionEffect.Load<GameObject>(), new EffectData
                        {
                            origin = transform.position,
                            scale = 3f
                        }, true);

                        gameObject.SetActive(false);

                        wasDisabled = true;
                    }
                }
            }
        }
    }
}