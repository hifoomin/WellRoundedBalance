namespace WellRoundedBalance.Utils
{
    public static class Extensions
    {
        public static PickupIndex GetPickupIndex(this ItemDef def)
        {
            return PickupCatalog.FindPickupIndex(def.itemIndex);
        }

        public static void RemoveComponent<T>(this GameObject go) where T : Component
        {
            GameObject.Destroy(go.GetComponent<T>());
        }

        public static void RemoveComponents<T>(this GameObject go) where T : Component
        {
            T[] coms = go.GetComponents<T>();
            for (int i = 0; i < coms.Length; i++)
            {
                GameObject.Destroy(coms[i]);
            }
        }

        public static bool IsAboveFraction(this HealthComponent healthComponent, float fraction) {
            float newFraction = fraction * 0.01f;
            float health = healthComponent.fullHealth * newFraction;
            return healthComponent.combinedHealth > health;
        }

        public static T GetRandom<T>(this List<T> list, Xoroshiro128Plus rng = null)
        {
            if (list.Count == 0)
            {
                return default(T);
            }
            if (rng == null)
            {
                return list[UnityEngine.Random.RandomRangeInt(0, list.Count)];
            }
            else
            {
                return list[rng.RangeInt(0, list.Count)];
            }
        }

        public static T GetRandom<T>(this T[] array)
        {
            int index = UnityEngine.Random.Range(0, array.Length);
            return array[index];
        }

        public static string ToPercentage(this float self)
        {
            return (self * 100).ToString() + "%";
        }

        public static bool CheckLoS(Vector3 victimPosition, Vector3 attackerPosition, float maxRange)
        {
            var vector = victimPosition - attackerPosition;
            if (vector.magnitude >= maxRange) return false; // < 120m + LoS check
            return !Physics.Raycast(victimPosition, vector, out RaycastHit raycastHit, vector.magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
        }
    }
}