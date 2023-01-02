using System;

namespace WellRoundedBalance.Mechanic
{
    public abstract class GlobalBase<T> : GlobalBase where T : GlobalBase<T>
    {
        public static T instance { get; set; }

        public GlobalBase()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}