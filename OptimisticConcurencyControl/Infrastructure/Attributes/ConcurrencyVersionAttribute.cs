using System;

namespace OptimisticConcurencyControl.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConcurrencyVersionAttribute : Attribute {}
}
