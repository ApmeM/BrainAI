﻿namespace BrainAI.AI.BehaviorTrees
{
    /// <summary>
    /// inverts the result of the child node
    /// </summary>
    public class InverterDecorator<T> : Decorator<T>
    {
        public override TaskStatus Update( T context )
        {
            var status = this.Child.Tick( context );

            if( status == TaskStatus.Success )
                return TaskStatus.Failure;
            
            if( status == TaskStatus.Failure )
                return TaskStatus.Success;
            
            return TaskStatus.Running;
        }
    }
}

