namespace Zametek.Data.ProjectPlan.v0_4_0
{
    [Serializable]
    public record ResourceTrackerModel
    {
        public int Time { get; init; }

        public int ResourceId { get; init; }

        public List<ResourceActivityTrackerModel> ActivityTrackers { get; init; } = [];
    }
}
