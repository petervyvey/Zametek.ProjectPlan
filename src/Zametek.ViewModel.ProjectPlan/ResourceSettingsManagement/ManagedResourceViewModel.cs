﻿using Avalonia.Data;
using ReactiveUI;
using System.ComponentModel;
using Zametek.Common.ProjectPlan;
using Zametek.Contract.ProjectPlan;
using Zametek.Maths.Graphs;

namespace Zametek.ViewModel.ProjectPlan
{
    public class ManagedResourceViewModel
        : ViewModelBase, IManagedResourceViewModel, IEditableObject
    {
        #region Fields

        private readonly IResourceSettingsManagerViewModel m_ResourceSettingsManagerViewModel;

        #endregion

        #region Ctors

        public ManagedResourceViewModel(
            IResourceSettingsManagerViewModel resourceSettingsManagerViewModel,
            ResourceModel resource)
        {
            ArgumentNullException.ThrowIfNull(resourceSettingsManagerViewModel);
            ArgumentNullException.ThrowIfNull(resource);
            m_ResourceSettingsManagerViewModel = resourceSettingsManagerViewModel;
            Id = resource.Id;
            m_Name = resource.Name;
            m_IsExplicitTarget = resource.IsExplicitTarget;
            m_IsInactive = resource.IsInactive;
            m_InterActivityAllocationType = resource.InterActivityAllocationType;
            m_UnitCost = resource.UnitCost;
            m_AllocationOrder = resource.AllocationOrder;
            m_DisplayOrder = resource.DisplayOrder;
            m_ColorFormat = resource.ColorFormat;
        }

        #endregion

        #region IManagedResourceViewModel Members

        public int Id { get; }

        private string m_Name;
        public string Name
        {
            get => m_Name;
            set => this.RaiseAndSetIfChanged(ref m_Name, value);
        }

        private bool m_IsExplicitTarget;
        public bool IsExplicitTarget
        {
            get => m_IsExplicitTarget;
            set
            {
                if (m_IsExplicitTarget != value)
                {
                    BeginEdit();
                    m_IsExplicitTarget = value;
                    EndEdit();
                }
                this.RaisePropertyChanged();
            }
        }

        private bool m_IsInactive;
        public bool IsInactive
        {
            get => m_IsInactive;
            set
            {
                if (m_IsInactive != value)
                {
                    BeginEdit();
                    m_IsInactive = value;
                    EndEdit();
                }
                this.RaisePropertyChanged();
            }
        }

        private InterActivityAllocationType m_InterActivityAllocationType;
        public InterActivityAllocationType InterActivityAllocationType
        {
            get => m_InterActivityAllocationType;
            set => this.RaiseAndSetIfChanged(ref m_InterActivityAllocationType, value);
        }




        public List<int> InterActivityPhases
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }





        private double m_UnitCost;
        public double UnitCost
        {
            get => m_UnitCost;
            set
            {
                if (value < 0)
                {
                    throw new DataValidationException(Resource.ProjectPlan.Messages.Message_UnitCostMustBeGreaterThanZero);
                }
                this.RaiseAndSetIfChanged(ref m_UnitCost, value);
            }
        }

        private int m_DisplayOrder;
        public int DisplayOrder
        {
            get => m_DisplayOrder;
            set => this.RaiseAndSetIfChanged(ref m_DisplayOrder, value);
        }

        private int m_AllocationOrder;
        public int AllocationOrder
        {
            get => m_AllocationOrder;
            set => this.RaiseAndSetIfChanged(ref m_AllocationOrder, value);
        }

        private ColorFormatModel m_ColorFormat;
        public ColorFormatModel ColorFormat
        {
            get => m_ColorFormat;
            set
            {
                if (m_ColorFormat != value)
                {
                    BeginEdit();
                    m_ColorFormat = value;
                    EndEdit();
                }
                this.RaisePropertyChanged();
            }
        }

        public object CloneObject()
        {
            return new Resource<int>(Id, Name, IsExplicitTarget, IsInactive, InterActivityAllocationType, UnitCost, AllocationOrder);
        }

        #endregion

        #region IEditableObject Members

        private bool m_isDirty;

        public void BeginEdit()
        {
            // Bug Fix: Windows Controls call EndEdit twice; Once
            // from IEditableCollectionView, and once from BindingGroup.
            // This makes sure it only happens once after a BeginEdit.
            m_isDirty = true;
        }

        public void EndEdit()
        {
            if (m_isDirty)
            {
                m_isDirty = false;
                m_ResourceSettingsManagerViewModel.AreSettingsUpdated = true;
            }
        }

        public void CancelEdit()
        {
            m_isDirty = false;
        }

        #endregion

        #region IDisposable Members

        private bool m_Disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                //m_ProjectStartSub?.Dispose();
                //m_ResourceSettingsSub?.Dispose();
                //m_DateTimeCalculatorSub?.Dispose();
                //m_CompilationSub?.Dispose();
                //ResourceSelector.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            m_Disposed = true;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
