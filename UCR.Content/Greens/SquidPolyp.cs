using MonoMod.Cil;

namespace UltimateCustomRun
{
    static class SquidPolyp
    {
        public static void ChangeLifetime(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "HealthDecay"),
                x => x.MatchLdcI4(30)
            );
            c.Index += 1;
            c.Next.Operand = Main.SquidPolypDuration.Value;
        }

        // [Error: Unity Log] KeyNotFoundException: The given key was not present in the dictionary.
        // Stack trace:
        // Mono Mod.Cil.ILCursor.GotoNext (MonoMod.Cil.MoveType moveType, System.Func`2[Mono.Cecil.Cil.Instruction, System.Boolean][] predicates) (at<b6be78b585f34b8294ca7a4c1df868d5>:IL_000A)
        // UltimateCustomRun.SquidPolyp.ChangeLifetime(MonoMod.Cil.ILContext il) (at<8f0cbb31e6494c82a49add25aa309552>:IL_0008)
        // MonoMod.Cil.ILContext.Invoke(MonoMod.Cil.ILContext+Manipulator manip) (at<b6be78b585f34b8294ca7a4c1df868d5>:IL_0087)
        // MonoMod.RuntimeDetour.ILHook+Context.InvokeManipulator(Mono.Cecil.MethodDefinition def, MonoMod.Cil.ILContext+Manipulator cb) (at<cc55d7b241764177b4632875c41ea959>:IL_0012)
        // DMD<Refresh>?-790811392._MonoMod_RuntimeDetour_ILHook+Context::Refresh(MonoMod.RuntimeDetour.ILHook+Context this) (at<42789215ecc74ad4aed7bbd21d994af0>:IL_00EA)
        // DMD<>?-790811392.Trampoline<MonoMod.RuntimeDetour.ILHook+Context::Refresh>?62959616 (System.Object ) (at<3800cbfe6ded4565b0a1b3d81becf1ba>:IL_0020)
        // HarmonyLib.Internal.RuntimeFixes.StackTraceFixes.OnILChainRefresh(System.Object self) (at<4c7064deb21340f6835f67d7a798b5ef>:IL_0000)
        // MonoMod.RuntimeDetour.ILHook.Apply() (at<cc55d7b241764177b4632875c41ea959>:IL_0059)
        // MonoMod.RuntimeDetour.ILHook..ctor(System.Reflection.MethodBase from, MonoMod.Cil.ILContext+Manipulator manipulator, MonoMod.RuntimeDetour.ILHookConfig& config) (at<cc55d7b241764177b4632875c41ea959>:IL_0148)
        // MonoMod.RuntimeDetour.ILHook..ctor(System.Reflection.MethodBase from, MonoMod.Cil.ILContext+Manipulator manipulator, MonoMod.RuntimeDetour.ILHookConfig config) (at<cc55d7b241764177b4632875c41ea959>:IL_0000)
        // MonoMod.RuntimeDetour.ILHook..ctor(System.Reflection.MethodBase from, MonoMod.Cil.ILContext+Manipulator manipulator) (at<cc55d7b241764177b4632875c41ea959>:IL_001C)
        // MonoMod.RuntimeDetour.HookGen.HookEndpoint._NewILHook(System.Reflection.MethodBase from, MonoMod.Cil.ILContext+Manipulator to) (at<cc55d7b241764177b4632875c41ea959>:IL_0000)
        // MonoMod.RuntimeDetour.HookGen.HookEndpoint._Add[TDelegate] (System.Func`3[T1, T2, TResult] gen, TDelegate hookDelegate) (at<cc55d7b241764177b4632875c41ea959>:IL_003C)
        // MonoMod.RuntimeDetour.HookGen.HookEndpoint.Modify(System.Delegate hookDelegate) (at<cc55d7b241764177b4632875c41ea959>:IL_0013)
        // MonoMod.RuntimeDetour.HookGen.HookEndpointManager.Modify(System.Reflection.MethodBase method, System.Delegate callback) (at<cc55d7b241764177b4632875c41ea959>:IL_0028)
        // MonoMod.RuntimeDetour.HookGen.HookEndpointManager.Modify[T] (System.Reflection.MethodBase method, System.Delegate callback) (at<cc55d7b241764177b4632875c41ea959>:IL_0000)
        // IL.RoR2.GlobalEventManager.add_OnInteractionBegin(MonoMod.Cil.ILContext+Manipulator ) (at<05837eaf68f74df1aac4745731e63ec5>:IL_000A)
        // UltimateCustomRun.Main.Awake() (at<8f0cbb31e6494c82a49add25aa309552>:IL_15E6)
        // UnityEngine.GameObject:AddComponent(Type)
        // BepInEx.Bootstrap.Chainloader:Start()
        // UnityEngine.Application:.cctor()
        // RoR2.Console:RegisterLogHandler()

        public static void ChangeAS(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterMaster>("get_inventory"),
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "BoostAttackSpeed"),
                x => x.MatchLdcI4(10)
            );
            c.Index += 2;
            c.Next.Operand = Main.SquidPolypAS.Value;
        }

        // [Error: Unity Log] KeyNotFoundException: The given key was not present in the dictionary.
        // Stack trace:
        // MonoMod.Cil.ILCursor.GotoNext (MonoMod.Cil.MoveType moveType, System.Func`2[Mono.Cecil.Cil.Instruction, System.Boolean][] predicates) (at<b6be78b585f34b8294ca7a4c1df868d5>:IL_000A)
        // UltimateCustomRun.SquidPolyp.ChangeAS(MonoMod.Cil.ILContext il) (at<ae96a854c5f3410fa474cc68502172b6>:IL_0008)
        // MonoMod.Cil.ILContext.Invoke(MonoMod.Cil.ILContext+Manipulator manip) (at<b6be78b585f34b8294ca7a4c1df868d5>:IL_0087)
        // MonoMod.RuntimeDetour.ILHook+Context.InvokeManipulator(Mono.Cecil.MethodDefinition def, MonoMod.Cil.ILContext+Manipulator cb) (at<cc55d7b241764177b4632875c41ea959>:IL_0012)
        // DMD<Refresh>?1905601792._MonoMod_RuntimeDetour_ILHook+Context::Refresh(MonoMod.RuntimeDetour.ILHook+Context this) (at<29cec792ea3643c0be3656c198c6b5e1>:IL_00EA)
        // DMD<>?1905601792.Trampoline<MonoMod.RuntimeDetour.ILHook+Context::Refresh>?-1535594496 (System.Object ) (at<3afd8e961faf4f72b4eac4a6e626d9a9>:IL_0020)
        // HarmonyLib.Internal.RuntimeFixes.StackTraceFixes.OnILChainRefresh(System.Object self) (at<4c7064deb21340f6835f67d7a798b5ef>:IL_0000)
        // MonoMod.RuntimeDetour.ILHook.Apply() (at<cc55d7b241764177b4632875c41ea959>:IL_0059)
        // MonoMod.RuntimeDetour.ILHook..ctor(System.Reflection.MethodBase from, MonoMod.Cil.ILContext+Manipulator manipulator, MonoMod.RuntimeDetour.ILHookConfig& config) (at<cc55d7b241764177b4632875c41ea959>:IL_0148)
        // MonoMod.RuntimeDetour.ILHook..ctor(System.Reflection.MethodBase from, MonoMod.Cil.ILContext+Manipulator manipulator, MonoMod.RuntimeDetour.ILHookConfig config) (at<cc55d7b241764177b4632875c41ea959>:IL_0000)
        // MonoMod.RuntimeDetour.ILHook..ctor(System.Reflection.MethodBase from, MonoMod.Cil.ILContext+Manipulator manipulator) (at<cc55d7b241764177b4632875c41ea959>:IL_001C)
        // MonoMod.RuntimeDetour.HookGen.HookEndpoint._NewILHook(System.Reflection.MethodBase from, MonoMod.Cil.ILContext+Manipulator to) (at<cc55d7b241764177b4632875c41ea959>:IL_0000)
        // MonoMod.RuntimeDetour.HookGen.HookEndpoint._Add[TDelegate] (System.Func`3[T1, T2, TResult] gen, TDelegate hookDelegate) (at<cc55d7b241764177b4632875c41ea959>:IL_003C)
        // MonoMod.RuntimeDetour.HookGen.HookEndpoint.Modify(System.Delegate hookDelegate) (at<cc55d7b241764177b4632875c41ea959>:IL_0013)
        // MonoMod.RuntimeDetour.HookGen.HookEndpointManager.Modify(System.Reflection.MethodBase method, System.Delegate callback) (at<cc55d7b241764177b4632875c41ea959>:IL_0028)
        // MonoMod.RuntimeDetour.HookGen.HookEndpointManager.Modify[T] (System.Reflection.MethodBase method, System.Delegate callback) (at<cc55d7b241764177b4632875c41ea959>:IL_0000)
        // IL.RoR2.GlobalEventManager.add_OnInteractionBegin(MonoMod.Cil.ILContext+Manipulator ) (at<05837eaf68f74df1aac4745731e63ec5>:IL_000A)
        // UltimateCustomRun.Main.Awake() (at<ae96a854c5f3410fa474cc68502172b6>:IL_15E6)
        // UnityEngine.GameObject:AddComponent(Type)
        // BepInEx.Bootstrap.Chainloader:Start()
        // UnityEngine.Application:.cctor()
        // RoR2.Console:RegisterLogHandler()
    }
}
