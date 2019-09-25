namespace BrainAI.AI.BehaviorTrees
{
    using System;
    using System.Collections.Generic;

    using BrainAI.AI.BehaviorTrees.Actions;
    using BrainAI.AI.BehaviorTrees.Composites;
    using BrainAI.AI.BehaviorTrees.Conditionals;
    using BrainAI.AI.BehaviorTrees.Decorators;

    /// <summary>
    /// helper for building a BehaviorTree using a fluent API. Leaf nodes need to first have a parent added. Parents can be Composites or
    /// Decorators. Decorators are automatically closed when a leaf node is added. Composites must have endComposite called to close them.
    /// </summary>
    public class BehaviorTreeBuilder<T>
    {
        private readonly T context;

        /// <summary>
        /// Last node created.
        /// </summary>
        private Behavior<T> currentNode;

        /// <summary>
        /// Stack nodes that we are build via the fluent API.
        /// </summary>
        private readonly Stack<Behavior<T>> parentNodeStack = new Stack<Behavior<T>>();


        public BehaviorTreeBuilder( T context )
        {
            this.context = context;
        }


        public static BehaviorTreeBuilder<T> Begin( T context )
        {
            return new BehaviorTreeBuilder<T>( context );
        }

        public BehaviorTreeBuilder<T> AddChildBehavior( Behavior<T> child )
        {
            if (this.parentNodeStack.Count == 0)
            {
                throw new InvalidOperationException("Can't create an unnested Behavior node. It must be a leaf node.");
            }

            var parent = this.parentNodeStack.Peek();
            if( parent is Composite<T> )
            {
                ( parent as Composite<T> ).AddChild( child );
            }
            else if( parent is Decorator<T> )
            {
                // Decorators have just one child so end it automatically
                ( parent as Decorator<T> ).Child = child;
                this.EndDecorator();
            }

            return this;
        }


        /// <summary>
        /// pushes a Composite or Decorator on the stack
        /// </summary>
        /// <returns>The parent node.</returns>
        /// <param name="composite">Composite.</param>
        private BehaviorTreeBuilder<T> PushParentNode( Behavior<T> composite )
        {
            if( this.parentNodeStack.Count > 0 )
                this.AddChildBehavior( composite );

            this.parentNodeStack.Push( composite );
            return this;
        }

        private void EndDecorator()
        {
            this.currentNode = this.parentNodeStack.Pop();
        }


        #region Leaf Nodes (actions and sub trees)

        public BehaviorTreeBuilder<T> Action( Func<T,TaskStatus> func )
        {
            return this.AddChildBehavior( new ExecuteAction<T>( func ) );
        }


        /// <summary>
        /// Like an action node but the function can return true/false and is mapped to success/failure.
        /// </summary>
        public BehaviorTreeBuilder<T> Action( Func<T,bool> func )
        {
            return this.Action( t => func( t ) ? TaskStatus.Success : TaskStatus.Failure );
        }


        public BehaviorTreeBuilder<T> Conditional( Func<T,TaskStatus> func )
        {
            return this.AddChildBehavior( new ExecuteActionConditional<T>( func ) );
        }


        /// <summary>
        /// Like a conditional node but the function can return true/false and is mapped to success/failure.
        /// </summary>
        public BehaviorTreeBuilder<T> Conditional( Func<T,bool> func )
        {
            return this.Conditional( t => func( t ) ? TaskStatus.Success : TaskStatus.Failure );
        }
        
        /// <summary>
        /// Splice a sub tree into the parent tree.
        /// </summary>
        public BehaviorTreeBuilder<T> SubTree( BehaviorTree<T> subTree )
        {
            return this.AddChildBehavior( new BehaviorTreeReference<T>( subTree ) );
        }

        #endregion


        #region Decorators

        public BehaviorTreeBuilder<T> ConditionalDecorator( Func<T,TaskStatus> func, bool shouldReevaluate = true )
        {
            var conditional = new ExecuteActionConditional<T>( func );
            return this.PushParentNode( new ConditionalDecorator<T>( conditional, shouldReevaluate ) );
        }


        /// <summary>
        /// Like a conditional decorator node but the function can return true/false and is mapped to success/failure.
        /// </summary>
        public BehaviorTreeBuilder<T> ConditionalDecorator( Func<T,bool> func, bool shouldReevaluate = true )
        {
            return this.ConditionalDecorator( t => func( t ) ? TaskStatus.Success : TaskStatus.Failure, shouldReevaluate );
        }


        public BehaviorTreeBuilder<T> AlwaysFail()
        {
            return this.PushParentNode( new AlwaysFail<T>() );
        }


        public BehaviorTreeBuilder<T> AlwaysSucceed()
        {
            return this.PushParentNode( new AlwaysSucceed<T>() );
        }


        public BehaviorTreeBuilder<T> Inverter()
        {
            return this.PushParentNode( new Inverter<T>() );
        }


        public BehaviorTreeBuilder<T> Repeater( int count )
        {
            return this.PushParentNode( new Repeater<T>( count ) );
        }


        public BehaviorTreeBuilder<T> UntilFail()
        {
            return this.PushParentNode( new UntilFail<T>() );
        }


        public BehaviorTreeBuilder<T> UntilSuccess()
        {
            return this.PushParentNode( new UntilSuccess<T>() );
        }

        #endregion


        #region Composites

        public BehaviorTreeBuilder<T> Parallel()
        {
            return this.PushParentNode( new ParallelSequence<T>() );
        }


        public BehaviorTreeBuilder<T> ParallelSelector()
        {
            return this.PushParentNode( new ParallelSelector<T>() );
        }


        public BehaviorTreeBuilder<T> Selector( AbortTypes abortType = AbortTypes.None )
        {
            return this.PushParentNode( new Selector<T>( abortType ) );
        }

        
        public BehaviorTreeBuilder<T> Sequence( AbortTypes abortType = AbortTypes.None )
        {
            return this.PushParentNode( new Sequence<T>( abortType ) );
        }

        public BehaviorTreeBuilder<T> EndComposite()
        {
            if (!(this.parentNodeStack.Peek() is Composite<T>))
            {
                throw new InvalidOperationException("attempting to end a composite but the top node is not a composite");
            }

            this.currentNode = this.parentNodeStack.Pop();
            return this;
        }

        #endregion


        public BehaviorTree<T> Build( )
        {
            if (this.currentNode == null)
            {
                throw new InvalidOperationException("Can't create a behaviour tree with zero nodes");
            }

            return new BehaviorTree<T>( this.context, this.currentNode );
        }
    }
}

