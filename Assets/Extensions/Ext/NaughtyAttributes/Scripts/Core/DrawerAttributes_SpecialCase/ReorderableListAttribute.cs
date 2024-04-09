using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReorderableListAttribute : SpecialCaseDrawerAttribute
    {
        public int Space { get; private set; }

        public ReorderableListAttribute(int space = 10)
        {
            Space = space;
        }
    }
}
