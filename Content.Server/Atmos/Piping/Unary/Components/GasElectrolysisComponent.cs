using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Content.Shared.Chemistry.Components;

namespace Content.Server.Atmos.Piping.Unary.Components;

/// <summary>
/// Used for an entity that converts moles of gas into units of reagent.
/// </summary>
[RegisterComponent]
[Access(typeof(GasElectrolysisSystem))]
public sealed partial class GasElectrolysisComponent : Component
{
    /// <summary>
    /// For a condenser, how many U of reagents are given per each mole of gas.
    /// </summary>
    /// <remarks>
    /// Derived from a standard of 500u per canister:
    /// 1 mol h2o -> 2 (h2) || 1 (o2)
    /// </remarks>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float MaxTempMultiplier = 0.2137f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float MaxEnergy = 500;
}
