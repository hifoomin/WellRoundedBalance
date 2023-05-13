using System;

namespace WellRoundedBalance.Allies
{
    public abstract class AllyBase<T> : AllyBase where T : AllyBase<T>
    {
        public static T instance { get; set; }

        public AllyBase()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}