using System;

/// <summary>
/// Represents a treatment entity.
/// </summary>
[Serializable]
public class Treatment
{
    /// <summary>
    /// Gets or sets the unique identifier for the treatment.
    /// </summary>
    public Guid ID { get; set; }

    /// <summary>
    /// Gets or sets the name of the treatment.
    /// </summary>
    public string Name { get; set; }
}
