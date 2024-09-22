﻿using ReactiveUI;
using System.Collections.ObjectModel;
using Zametek.Common.ProjectPlan;
using Zametek.Contract.ProjectPlan;

namespace Zametek.ViewModel.ProjectPlan
{
    public class WorkStreamSelectorViewModel
        : ViewModelBase, IWorkStreamSelectorViewModel
    {
        #region Fields

        private readonly object m_Lock;
        private readonly bool m_PhaseOnly;
        private static readonly EqualityComparer<ISelectableWorkStreamViewModel> s_EqualityComparer =
            EqualityComparer<ISelectableWorkStreamViewModel>.Create(
                    (x, y) =>
                    {
                        if (x is null)
                        {
                            return false;
                        }
                        if (y is null)
                        {
                            return false;
                        }
                        return x.Id == y.Id;
                    },
                    x => x.Id);

        private static readonly Comparer<ISelectableWorkStreamViewModel> s_SortComparer =
            Comparer<ISelectableWorkStreamViewModel>.Create(
                    (x, y) =>
                    {
                        if (x is null)
                        {
                            if (y is null)
                            {
                                return 0;
                            }
                            return -1;
                        }
                        if (y is null)
                        {
                            return 1;
                        }

                        return x.Id.CompareTo(y.Id);
                    });

        #endregion

        #region Ctors

        public WorkStreamSelectorViewModel()
            : this(false)
        {
        }

        public WorkStreamSelectorViewModel(bool phaseOnly)
        {
            m_Lock = new object();
            m_PhaseOnly = phaseOnly;
            m_TargetWorkStreams = new(s_EqualityComparer);
            m_ReadOnlyTargetWorkStreams = new(m_TargetWorkStreams);
            m_SelectedTargetWorkStreams = new(s_EqualityComparer);
        }

        #endregion

        #region Properties

        private readonly ObservableUniqueCollection<ISelectableWorkStreamViewModel> m_TargetWorkStreams;
        private readonly ReadOnlyObservableCollection<ISelectableWorkStreamViewModel> m_ReadOnlyTargetWorkStreams;
        public ReadOnlyObservableCollection<ISelectableWorkStreamViewModel> TargetWorkStreams => m_ReadOnlyTargetWorkStreams;

        // Use ObservableUniqueCollection to prevent selected
        // items appearing twice in the Urse MultiComboBox.
        private readonly ObservableUniqueCollection<ISelectableWorkStreamViewModel> m_SelectedTargetWorkStreams;
        public ObservableCollection<ISelectableWorkStreamViewModel> SelectedTargetWorkStreams => m_SelectedTargetWorkStreams;

        public string TargetWorkStreamsString
        {
            get
            {
                lock (m_Lock)
                {
                    return string.Join(
                        DependenciesStringValidationRule.Separator,
                        TargetWorkStreams
                            .Where(x => (!m_PhaseOnly && x.IsSelected)
                                    || (m_PhaseOnly && x.IsSelected && x.IsPhase))
                            .Select(x => x.DisplayName));
                }
            }
        }

        public IList<int> SelectedWorkStreamIds
        {
            get
            {
                lock (m_Lock)
                {
                    return TargetWorkStreams
                        .Where(x => (!m_PhaseOnly && x.IsSelected)
                                || (m_PhaseOnly && x.IsSelected && x.IsPhase))
                        .Select(x => x.Id)
                        .ToList();
                }
            }
        }

        #endregion

        #region Public Methods

        public void SetTargetWorkStreams(
            IEnumerable<WorkStreamModel> targetWorkStreams,
            HashSet<int> selectedTargetWorkStreams)
        {
            ArgumentNullException.ThrowIfNull(targetWorkStreams);
            ArgumentNullException.ThrowIfNull(selectedTargetWorkStreams);
            lock (m_Lock)
            {
                IEnumerable<WorkStreamModel> correctTargetWorkStreams =
                    targetWorkStreams.Where(x => (!m_PhaseOnly) || (m_PhaseOnly && x.IsPhase));

                {
                    // Find target view models that have been removed.
                    List<ISelectableWorkStreamViewModel> removedViewModels = m_TargetWorkStreams
                        .ExceptBy(correctTargetWorkStreams.Select(x => x.Id), x => x.Id)
                        .ToList();

                    // Delete the removed items from the target and selected collections.
                    foreach (ISelectableWorkStreamViewModel vm in removedViewModels)
                    {
                        m_TargetWorkStreams.Remove(vm);
                        m_SelectedTargetWorkStreams.Remove(vm);
                        vm.Dispose();
                    }

                    // Find the selected view models that have been removed.
                    List<ISelectableWorkStreamViewModel> removedSelectedViewModels = m_SelectedTargetWorkStreams
                        .ExceptBy(selectedTargetWorkStreams, x => x.Id)
                        .ToList();

                    // Delete the removed selected items from the selected collections.
                    foreach (ISelectableWorkStreamViewModel vm in removedSelectedViewModels)
                    {
                        vm.IsSelected = false;
                        m_SelectedTargetWorkStreams.Remove(vm);
                    }
                }
                {
                    // Find the target models that have been added.
                    List<WorkStreamModel> addedModels = correctTargetWorkStreams
                        .ExceptBy(m_TargetWorkStreams.Select(x => x.Id), x => x.Id)
                        .ToList();

                    List<ISelectableWorkStreamViewModel> addedViewModels = [];

                    // Create a collection of new view models.
                    foreach (WorkStreamModel model in addedModels)
                    {
                        var vm = new SelectableWorkStreamViewModel(
                              model.Id,
                              model.Name,
                              model.IsPhase,
                              selectedTargetWorkStreams.Contains(model.Id),
                              this);

                        m_TargetWorkStreams.Add(vm);
                        if (vm.IsSelected)
                        {
                            m_SelectedTargetWorkStreams.Add(vm);
                        }
                    }
                }
                {
                    // Update names.
                    Dictionary<int, WorkStreamModel> targetWorkStreamLookup = correctTargetWorkStreams.ToDictionary(x => x.Id);

                    foreach (ISelectableWorkStreamViewModel vm in m_TargetWorkStreams)
                    {
                        if (targetWorkStreamLookup.TryGetValue(vm.Id, out WorkStreamModel? value))
                        {
                            vm.Name = value.Name;
                        }
                    }
                }

                m_TargetWorkStreams.Sort(s_SortComparer);
            }
            RaiseTargetWorkStreamsPropertiesChanged();
        }

        public void ClearTargetWorkStreams()
        {
            lock (m_Lock)
            {
                foreach (IDisposable targetWorkStream in TargetWorkStreams)
                {
                    targetWorkStream.Dispose();
                }
                m_TargetWorkStreams.Clear();
            }
        }

        public void ClearSelectedTargetWorkStreams()
        {
            lock (m_Lock)
            {
                foreach (IDisposable targetWorkStream in SelectedTargetWorkStreams)
                {
                    targetWorkStream.Dispose();
                }
                m_SelectedTargetWorkStreams.Clear();
            }
        }

        public void RaiseTargetWorkStreamsPropertiesChanged()
        {
            this.RaisePropertyChanged(nameof(TargetWorkStreams));
            this.RaisePropertyChanged(nameof(TargetWorkStreamsString));
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return TargetWorkStreamsString;
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
                ClearTargetWorkStreams();
                ClearSelectedTargetWorkStreams();
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
