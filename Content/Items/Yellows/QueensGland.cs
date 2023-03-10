using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Items;
using System;

namespace WellRoundedBalance.Items.Yellows
{
    public class QueensGland : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Queens Gland";
        public override string InternalPickupToken => "beetleGland";

        public override string PickupText => "Recruit 3 Beetle Guards.";

        public override string DescText => "<style=cIsUtility>Summon 3 Beetle Guards</style> with bonus <style=cIsDamage>200%</style> <style=cStack>(+100% per stack)</style> damage and <style=cIsHealing>100%</style> <style=cIsHealing>health</style>.";

        [ConfigField("Beetle Guard Count", 3)]
        public static int beetleGuardCount;

        [ConfigField("Beetle Guard Base Damage", "Formula for final damage: (Beetle Guard Base Damage + Beetle Guard Damage Per Stack * (Queen's Gland - 1)) * 10", 22)]
        public static int beetleGuardBaseDamage;

        [ConfigField("Beetle Guard Damage Per Stack", "Formula for final damage: (Beetle Guard Base Damage + Beetle Guard Damage Per Stack * (Queen's Gland - 1)) * 10", 11)]
        public static int beetleGuardDamagePerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += ChangeLimit2;
            IL.RoR2.Items.BeetleGlandBodyBehavior.FixedUpdate += FuckYou;
            On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
        }

        private void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);
            var stack = Util.GetItemCountForTeam(body.teamComponent.teamIndex, RoR2Content.Items.BeetleGland.itemIndex, false);
            if (self.name == "BeetleGuardAllyMaster(Clone)")
            {
                self.inventory.RemoveItem(RoR2Content.Items.BoostDamage, 30);
                self.inventory.GiveItem(RoR2Content.Items.BoostDamage, beetleGuardBaseDamage + beetleGuardDamagePerStack * (stack - 1));
                // this works I checked :smirk_cat:
            }
        }

        private void FuckYou(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld("RoR2.Items.BaseItemBodyBehavior", "stack")))
            {
                c.Index += 1;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<int, BaseItemBodyBehavior, int>>((useless, self) =>
                {
                    return 3;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Queens Gland Count hook");
            }
        }

        private int ChangeLimit2(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.BeetleGuardAlly)
            {
                return 3;
            }
            else
            {
                return orig(self, slot);
            }
        }
    }
}