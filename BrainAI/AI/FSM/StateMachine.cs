namespace BrainAI.AI.FSM
{
    using System;
    using System.Collections.Generic;

    using BrainAI.AI;

    public class StateMachine<T> : IAITurn
    {
        public event Action OnStateChanged;

        public State<T> PreviousState { get; private set; }

        public State<T> CurrentState { get; private set; }

        public State<T> NextState { get; private set; }

        protected T Context;

        private readonly Dictionary<Type, State<T>> states = new Dictionary<Type, State<T>>();

        public StateMachine( T context, State<T> initialState)
        {
            this.Context = context;

            // setup our initial state
            this.AddState( initialState );
            this.NextState = initialState;
        }

        /// <summary>
        /// adds the state to the machine
        /// </summary>
        public void AddState( State<T> state )
        {
            state.SetMachineAndContext( this, this.Context );
            this.states[state.GetType()] = state;
        }

        /// <summary>
        /// ticks the state machine with the provided delta time
        /// </summary>
        public void Tick()
        {
            if (this.NextState != null)
            {
                // only call end if we have a currentState
                this.CurrentState?.End();

                // swap states and call begin
                this.PreviousState = this.CurrentState;
                this.CurrentState = this.NextState;
                this.NextState = null;
                this.CurrentState.Begin();

                // fire the changed event if we have a listener
                this.OnStateChanged?.Invoke();
            }

            this.CurrentState.Update();
        }

        /// <summary>
        /// changes the current state
        /// </summary>
        public void ChangeState<TR>() where TR : State<T>
        {
            // avoid changing to the same state
            var newType = typeof( TR );
            if (this.CurrentState.GetType() == newType)
                return;

            this.NextState = this.states[newType];
        }
    }
}

