namespace BrainAI.AI.UtilityAI
{
    public interface IIntentContainer<T>
    {
        IIntent<T> Intent { get; set; }
    }
}