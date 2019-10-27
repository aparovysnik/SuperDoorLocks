namespace SuperDoorLocks.DomainModels.ServiceResultModels
{
    public class GetActionOutcome<T>: ServiceActionOutcome
    {
        public T Item { get; set; }
    }
}
