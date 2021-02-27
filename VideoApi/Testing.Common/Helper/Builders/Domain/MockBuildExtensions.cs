using System;
using System.Reflection;
using FluentAssertions.Common;

namespace Testing.Common.Helper.Builders.Domain
{
    public static class MockBuildExtensions
    {
        /// <summary>
        /// Helper to set a protected member for testing purposes, for example navigation properties
        /// </summary>
        public static void SetProtectedProperty(this object instance, string propertyName, object value)
        {
            var instanceType = instance.GetType();
            var property = instanceType.GetProperty(propertyName);
            if (property == null) throw new InvalidOperationException($"No property '{propertyName}' found on object of type '{instanceType.Name}'");
            property.SetValue(instance, value);
        }
        
        /// <summary>
        /// Helper to set a protected member for testing purposes, for example navigation properties
        /// </summary>
        public static void SetProtectedField(this object instance, string fieldName, object value)
        {
            var instanceType = instance.GetType();
            var field = instanceType.FindField(fieldName, value.GetType());
            if (field == null) throw new InvalidOperationException($"No field '{fieldName}' found on object of type '{instanceType.Name}'");
            field.SetValue(instance, value);
        }
    }
}
