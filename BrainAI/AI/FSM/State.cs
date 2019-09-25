namespace BrainAI.AI.FSM
{
    public abstract class State<T>
    {
        protected StateMachine<T> Machine;
        protected T Context;

        internal void SetMachineAndContext( StateMachine<T> machine, T context )
        {
            this.Machine = machine;
            this.Context = context;
            this.OnInitialized();
        }

        /// <summary>
        /// called directly after the machine and context are set allowing the state to do any required setup
        /// </summary>
        public virtual void OnInitialized()
        {}

        /// <summary>
        /// called when the state becomes the active state
        /// </summary>
        public virtual void Begin()
        {}

        /// <summary>
        /// called every frame this state is the active state
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// called when this state is no longer the active state
        /// </summary>
        public virtual void End()
        {}
    }
}

