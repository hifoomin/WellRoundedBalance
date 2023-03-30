using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class HuntersHarpoon : ItemBase
    {
        public static BuffDef speedBuff;

        public override string DescText => "Killing an enemy increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(movementSpeedGainPerBuff) + "</style>, up to <style=cIsUtility>" + baseMaxBuffCount + "</style>" +
                                           (maxBuffCountPerStack > 0 ? " <style=cStack>(+" + maxBuffCountPerStack + " per stack)</style>" : "") + " times, fading over <style=cIsUtility>" + baseBuffDuration + "</style>" +
                                           (buffDurationPerStack > 0 ? " <style=cStack>(+" + buffDurationPerStack + " per stack)</style>" : "") +
                                           " seconds.";

        public override ItemDef InternalPickup => DLC1Content.Items.MoveSpeedOnKill;
        public override string Name => ":: Items :: Greens :: Hunters Harpoon";

        public override string PickupText => "Killing an enemy gives you a burst of movement speed.";

        [ConfigField("Movement Speed Gain Per Buff", "Decimal.", 0.16f)]
        public static float movementSpeedGainPerBuff;

        [ConfigField("Base Max Buff Count", 3)]
        public static int baseMaxBuffCount;

        [ConfigField("Max Buff Count Per Stack", 2)]
        public static int maxBuffCountPerStack;

        [ConfigField("Base Buff Duration", 5f)]
        public static float baseBuffDuration;

        [ConfigField("Buff Duration Per Stack", 0f)]
        public static float buffDurationPerStack;

        private static int GetMaxBuffs(int c) => baseMaxBuffCount + (maxBuffCountPerStack * (c - 1));
        private static float GetDuration(int c) => baseBuffDuration + (buffDurationPerStack * (c - 1));

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
            args.moveSpeedMultAdd += movementSpeedGainPerBuff * sender.GetBuffCount(speedBuff);
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            var attackerBody = damageReport.attackerBody;
            if (!attackerBody)
            {
                return;
            }

            var inventory = attackerBody.inventory;
            if (!inventory)
            {
                return;
            }

            var stack = inventory.GetItemCount(DLC1Content.Items.MoveSpeedOnKill);
            if (stack > 0)
            {
                var maxDuration = baseBuffDuration + buffDurationPerStack * (stack - 1);

                HandleBuffLogic(damageReport.attackerBody);

                EffectData effectData = new()
                {
                    origin = attackerBody.corePosition
                };

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
                if (!adjustRotation)
                {
                    effectData.rotation = attackerBody.transform.rotation;
                }
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MoveSpeedOnKillActivate"), effectData, true);
            }
        }

        private static void HandleBuffLogic(CharacterBody target) {
            int harpoons = target.inventory.GetItemCount(DLC1Content.Items.MoveSpeedOnKill);
            int maxBuffs = GetMaxBuffs(harpoons);
            float duration = GetDuration(harpoons);
            int toApply = target.HasBuff(speedBuff) ? 1 : 2;

            for (int i = 0; i < toApply; i++) {
                target.AddTimedBuff(speedBuff, duration, maxBuffs);
            }

            for (int i = 0; i < target.timedBuffs.Count; i++) {
                CharacterBody.TimedBuff buff = target.timedBuffs[i];
                if (buff.buffIndex != speedBuff.buffIndex) {
                    continue;
                }
                buff.timer = duration;
            }
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(DLC1Content.Items), "MoveSpeedOnKill")))

            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessItem));
            }
            else
            {
                Logger.LogError("Failed to apply Hunter's Harpoon Deletion hook");
            }
        }
    }
}