﻿using Zametek.Maths.Graphs;

namespace Zametek.Data.ProjectPlan.v0_1_0
{
    [Serializable]
    public record ResourceModel
    {
        public int Id { get; init; }

        public string? Name { get; init; }

        public bool IsExplicitTarget { get; init; }

        public InterActivityAllocationType InterActivityAllocationType { get; init; }

        public double UnitCost { get; init; }

        public int DisplayOrder { get; init; }

        public ColorFormatModel? ColorFormat { get; init; }
    }
}
