namespace BrainAI.AI.GOAP
{
    /// <summary>
    /// convenince Action subclass with a typed context. This is useful when an Action requires validation so that it has some way to get
    /// the data it needs to do the validation.
    /// </summary>
    public class GOAPAction<T> : GOAPAction
    {
        protected T Context;


        public GOAPAction( T context, string name ) : base( name )
        {
            this.Context = context;
            this.Name = name;
        }


        public GOAPAction( T context, string name, int cost ) : this( context, name )
        {
            this.Cost = cost;
        }


        public virtual void Execute()
        {
        }
    }
}

