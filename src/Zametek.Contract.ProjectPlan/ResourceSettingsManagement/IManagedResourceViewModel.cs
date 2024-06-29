﻿using System.ComponentModel;
using Zametek.Common.ProjectPlan;
using Zametek.Maths.Graphs;

namespace Zametek.Contract.ProjectPlan
{
    public interface IManagedResourceViewModel
        : IDisposable, INotifyPropertyChanged
    {
        int Id { get; }

        string Name { get; set; }

        bool IsExplicitTarget { get; set; }

        bool IsInactive { get; set; }

        InterActivityAllocationType InterActivityAllocationType { get; set; }

        List<int> InterActivityPhases { get; set; }

        double UnitCost { get; set; }

        int AllocationOrder { get; set; }

        int DisplayOrder { get; set; }

        ColorFormatModel ColorFormat { get; set; }
    }
}
