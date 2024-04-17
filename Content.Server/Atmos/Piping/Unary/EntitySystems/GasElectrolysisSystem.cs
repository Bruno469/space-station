using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.Atmos.Reactions;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System.Collections.Frozen;
using System.Linq;
using System.Security.Permissions;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems;

[UsedImplicitly]
public sealed class GasElectrolysisSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private readonly PowerReceiverSystem _power = default!;
    [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    
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

        for (var i = 0; i < Atmospherics.TotalNumberOfGases; i++)
        {
            if (_atmosphereSystem.GetGas(i) is not { } Tilegas)
                continue;

            var moleTemp = entity.Comp.MaxTempMultiplier;
            double EficiencTemp = environment.Temperature <= moleTemp ? environment.Temperature / moleTemp : 1.0;
            double molsToConvert = environment.TotalMoles;

            environment.AdjustMoles(gasId:9, (float) molsToConvert);
        }
    }
}