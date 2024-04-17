using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;
using Robust.Server.GameObjects;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems;

[UsedImplicitly]
public sealed class GasElectrolysisSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private readonly PowerReceiverSystem _power = default!;
    [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GasElectrolysisComponent, AtmosDeviceUpdateEvent>(OnElectrolysisUpdated);
    }

    private void OnElectrolysisUpdated(Entity<GasElectrolysisComponent> entity, ref AtmosDeviceUpdateEvent args)
    {
        if (args.Grid is not {} grid)
                return;

        var position = _transformSystem.GetGridTilePositionOrDefault(entity);
        var environment = _atmosphereSystem.GetTileMixture(grid, args.Map, position, true);

        if (environment == null)
                return;

        
    }

    public float NumberOfMolesToConvert(ApcPowerReceiverComponent comp, GasMixture mix, float dt)
    {
        var hc = _atmosphereSystem.GetHeatCapacity(mix, true);
        var alpha = 0.8f; // tuned to give us 1-ish u/second of reagent conversion
        // ignores the energy needed to cool down the solution to the condensation point, but that probably adds too much difficulty and so let's not simulate that
        var energy = comp.Load * dt;
        return energy / (alpha * hc);
    }
}
