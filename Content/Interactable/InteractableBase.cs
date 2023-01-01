using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace WellRoundedBalance.Interactable
{
    public abstract class InteractableBase
    {
        public abstract string Name { get; }
        public virtual bool isEnabled { get; } = true;

        public abstract void Hooks();

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public virtual void Init()
        {
            Hooks();
        }
    }
}