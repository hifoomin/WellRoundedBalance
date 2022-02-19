using System;
namespace UltimateCustomRun
{
    public abstract class ItemBase<T> : ItemBase where T : ItemBase<T>
    {
        public static T instance { get; set; }
        public ItemBase()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}
