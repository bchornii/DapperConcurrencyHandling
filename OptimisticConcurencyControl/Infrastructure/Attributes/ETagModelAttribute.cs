using System;

namespace OptimisticConcurencyControl.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ETagModelAttribute :Attribute {}
}