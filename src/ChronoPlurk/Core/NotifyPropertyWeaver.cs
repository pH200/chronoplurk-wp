using System;

namespace NotifyPropertyWeaver
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementPropertyChangedAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AlsoNotifyForAttribute : Attribute
    {
        public AlsoNotifyForAttribute(string property) { }
        public AlsoNotifyForAttribute(string property, params string[] otherProperties) { }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class DoNotNotifyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class DependsOnAttribute : Attribute
    {
        public DependsOnAttribute(string dependency) { }
        public DependsOnAttribute(string dependency, params string[] otherDependencies) { }
    }
}
