using Inferno.Stat_AI;
using RoR2;
using System.ComponentModel;
using WolfoQualityOfLife;

namespace WellRoundedBalance.Artifacts.Vanilla
{
    internal class Command : ArtifactEditBase<Command>
    {
        public override string Name => ":: Artifacts :: Command";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
            On.RoR2.GenericPickupController.BodyHasPickupPermission += GenericPickupController_BodyHasPickupPermission;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            Changes();
        }

        private void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                var player = self.GetComponent<PlayerCharacterMasterController>();
                if (player)
                {
                    var master = self.GetComponent<CharacterMaster>();
                    if (master)
                    {
                        var body = master.GetBody();
                        if (body)
                        {
                            var ic = body.GetComponent<ItemCount>();
                            if (ic && (ic.items + 1) > body.level)
                            {
                                return;
                            }
                        }
                    }
                }
            }
            orig(self, itemIndex, count);
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (!RunArtifactManager.instance)
            {
                return;
            }
            if (!RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                return;
            }
            if (!body.isPlayerControlled)
            {
                return;
            }

            var inventory = body.inventory;
            if (!inventory)
            {
                return;
            }

            if (body.GetComponent<ItemCount>() == null)
            {
                body.gameObject.AddComponent<ItemCount>();
            }

            var ic = body.GetComponent<ItemCount>();

            ic.items = 0;

            for (int i = 0; i < inventory.itemStacks.Length; i++)
            {
                var index = inventory.itemStacks[i];
                if (index > 0)
                {
                    var def = ItemCatalog.GetItemDef((ItemIndex)i);
                    if (def.hidden == true || def.deprecatedTier == ItemTier.NoTier)
                    {
                        continue;
                    }
                    ic.items += index;
                }
            }
        }

        private bool GenericPickupController_BodyHasPickupPermission(On.RoR2.GenericPickupController.orig_BodyHasPickupPermission orig, CharacterBody body)
        {
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                var player = body.isPlayerControlled;
                var inventory = body.inventory;
                if (player && inventory)
                {
                    var ic = body.GetComponent<ItemCount>();
                    if (ic && (ic.items + 1) > body.level)
                    {
                        return false;
                    }
                }
            }
            return orig(body);
        }

        private void Changes()
        {
            LanguageAPI.Add("ARTIFACT_COMMAND_DESCRIPTION", "Choose your items, but their amount is limited to your current level.");
        }
    }

    public class ItemCount : MonoBehaviour
    {
        public int items = 0;
    }
}