using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class HuntersHarpoon : ItemBase<HuntersHarpoon>
    {
        public static BuffDef speedBuff;

        public override ItemDef InternalPickup => DLC1Content.Items.MoveSpeedOnKill;
        public override string Name => ":: Items :: Greens :: Hunters Harpoon";
        public override string PickupText => "Kills increase movement speed up to 3 times.";

        public override string DescText => $"Killing an enemy increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>{d(movementSpeed)}</style>" +
            StackDesc(maxCount, maxCountStack, init => $", up to <style=cIsUtility>{init}</style>{{Stack}} times", noop) +
            StackDesc(duration, durationStack, init => $" for <style=cIsUtility>{init}</style>{{Stack}} seconds", noop) + ".";

        [ConfigField("Duration", "", 5f)]
        public static float duration;

        [ConfigField("Duration per Stack", "", 0f)]
        public static float durationStack;

        [ConfigField("Duration is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float durationIsHyperbolic;

        [ConfigField("Movement Speed for Initial Stack", "Decimal.", 0.16f)]
        public static float movementSpeed;

        [ConfigField("Movement Speed for Additional Stacks", "Decimal.", 0.16f)]
        public static float movementSpeedStack;

        [ConfigField("Movement Speed is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float movementSpeedIsHyperbolic;

        [ConfigField("Max Count", "", 3)]
        public static int maxCount;

        [ConfigField("Max Count per Stack", "", 2)]
        public static int maxCountStack;

        [ConfigField("Max Count is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float maxCountIsHyperbolic;

        public override void Init()
        {
            var arrow = Utils.Paths.Texture2D.texBuffKillMoveSpeed.Load<Texture2D>();

            speedBuff = ScriptableObject.CreateInstance<BuffDef>();
            speedBuff.canStack = true;
            speedBuff.isDebuff = false;
            speedBuff.isCooldown = false;
            speedBuff.isHidden = false;
            speedBuff.buffColor = new Color32(157, 217, 226, 255);
            speedBuff.iconSprite = Sprite.Create(arrow, new Rect(0f, 0f, arrow.width, arrow.height), new Vector2(0f, 0f));
            speedBuff.name = "Hunter's Harpoon Movement Speed";

            ContentAddition.AddBuffDef(speedBuff);
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            args.moveSpeedMultAdd += StackAmount(movementSpeed, movementSpeedStack, sender.GetBuffCount(speedBuff), movementSpeedIsHyperbolic);
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            var attackerBody = damageReport.attackerBody;
            if (!attackerBody) return;
            var inventory = attackerBody.inventory;
            if (!inventory) return;

            var stack = inventory.GetItemCount(DLC1Content.Items.MoveSpeedOnKill);
            if (stack <= 0) return;
            var maxDuration = StackAmount(duration, durationStack, stack, durationIsHyperbolic);
            var cap = Mathf.Min((int)StackAmount(maxCount, maxCountStack, stack, maxCountIsHyperbolic), (attackerBody.HasBuff(speedBuff) ? attackerBody.GetBuffCount(speedBuff) : 0) + 1);
            for (var i = maxDuration; i > 0; i -= maxDuration / cap) attackerBody.AddTimedBuff(speedBuff, i, cap);

            EffectData effectData = new() { origin = attackerBody.corePosition };

            var characterMotor = attackerBody.characterMotor;
            var adjustRotation = false;

            if (characterMotor)
            {
                var moveDirection = characterMotor.moveDirection;
                if (moveDirection != Vector3.zero)
                {
                    effectData.rotation = Util.QuaternionSafeLookRotation(moveDirection);
                    adjustRotation = true;
                }
            }
            if (!adjustRotation) effectData.rotation = attackerBody.transform.rotation;
            EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MoveSpeedOnKillActivate"), effectData, true);
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DLC1Content.Items), nameof(DLC1Content.Items.MoveSpeedOnKill))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessItem));
            }
            else Logger.LogError("Failed to apply Hunter's Harpoon Deletion hook");
        }
    }
}