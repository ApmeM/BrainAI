namespace BrainAI.AI.BehaviorTrees.Decorators
{
    public abstract class Decorator<T> : Behavior<T>
    {
        public Behavior<T> Child;

        public override void Invalidate()
        {
            base.Invalidate();
            this.Child.Invalidate();
        }
    }
}

