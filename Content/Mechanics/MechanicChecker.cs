using System;

namespace WellRoundedBalance.Mechanics
{
    public abstract class MechanicBase<T> : MechanicBase where T : MechanicBase<T>
    {
        public static T instance { get; set; }

        public MechanicBase()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}