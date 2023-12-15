using System;

namespace ImageNormalizer.Attributes;

/// <summary>
/// Represents a stateless service implementation, which can be registered as a Singleton dependency,
/// assuming all of its own encapsulated dependencies are themselves stateless service implementations.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class StatelessServiceAttribute : Attribute
{
    public string? Remark { get; set; }
}
