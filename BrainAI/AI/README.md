AI
==========
BrainAI includes several different options for setting up AI ranging from a super simple transitionless finite state machine (FSM) to extendable behavior trees to ultra-flexible Utility Based AI. 
You can mix and match them as you see fit.
For better understanding check ai tests section.

State Machine
==========
The next step up is `StateMachine` which implements the "states as objects" pattern. 
`StateMachine` uses separate classes for each state so it is a better choice for more complex systems.

We start to get into the concept of a **context** with `StateMachine`. 
In coding, the context is just the class used to satisfy a generic constraint. 
In a `List<string>` the *string* would be the context class, the class that the list operates on. 
With all of the rest of the AI solutions you get to specify the context class. 
It could be your Enemy class, Player class or a helper object that contains any information relevant to your AI (such as the Player, a list of Enemies, navigation information, etc).

Here is a simple example showing the usage (with the State subclasses omitted for brevity):

```csharp
// define a state machine that will work with an object of type MinerState as the focus with an initial state of MineState
public class MinerStateMachine : StateMachine<MinerState>
{
    public MinerStateMachine() : base(new MinerState(), new MineState())
    {
        // we can now add any additional states
        this.AddState(new SleepState());
        this.AddState(new DrinkState());
    }
}

// create defined state machine.
var machine = new MinerStateMachine();

// this method typically called in a loop
machine.Tick();

// change state
machine.changeState<SleepState>();
```



Behavior Trees
==========
The de facto standard for composing AI for the last decade. 
Behavior trees are composed of a tree of nodes. 
Nodes can make decisions and perform actions based on the state of the world. 
BrainAI includes a `BehaviorTreeBuilder` class that provides a fluent API for setting up a behavior tree. 
The `BehaviorTreeBuilder` is a great way to reduce the barrier of entry to using behavior trees and get up and running quickly.


## Composites
Composites are parent nodes in a behavior tree. They house 1 or more children and execute them in different ways.

- **Sequence<T>:** returns failure as soon as one of its children returns failure. If a child returns success it will sequentially run the next child in the next tick of the tree.
- **Selector<T>:** returns success as soon as one of its child tasks return success. If a child task returns failure then it will sequentially run the next child in the next tick.
- **Parallel<T>:** runs each child until a child returns failure. It differs from `Sequence` only in that it runs all children every tick
- **ParallelSelector<T>:** like a `Selector` except it will run all children every tick


## Conditionals
Conditionals are binary success/failure nodes. They are identified by the IConditional interface. They check some condition of your game world and either return success or failure. These are inherently game specific so BrainAI only provides a single generic Conditional out of the box and a helper Conditional that wraps an Action so you can avoid having to make a separate class for each Conditional.

- **ExecuteActionConditional<T>:** wraps a Func and executes it as the Conditional. Useful for prototyping and to avoid creating separate classes for simple Conditionals.


## Decorators
Decorators are wrapper tasks that have a single child. They can modify the behavior of the child task in various ways such as inverting the result, running it until failure, etc.

- **AlwaysFail<T>:** always returns failure regardless of the child result
- **AlwaysSucceed<T>:** always returns success regardless of the child result
- **ConditionalDecorator<T>:** wraps a Conditional and will only run its child if a condition is met
- **Inverter<T>:** inverts the result of its child
- **Repeater<T>:** repeats its child task a specified number of times
- **UntilFail<T>:** keeps executing its child task until it returns failure
- **UntilSuccess<T>:** keeps executing its child task until it returns success


## Actions
Actions are the leaf nodes of the behavior tree. This is where stuff happens such as playing an animation, triggering an event, etc.

- **ExecuteAction<T>:** wraps a Func and executes it as its action. Useful for prototyping and to avoid creating separate classes for simple Actions.
- **WaitAction<T>:** waits a specified amount of time
- **LogAction<T>:** logs a string to the console. Useful for debugging.
- **BehaviorTreeReference<T>:** runs another BehaviorTree<T>



Goal Oriented Action Planning (GOAP)
==========
GOAP differs quite a bit from the other AI solutions. 
With GOAP, you provide the planner with a list of the actions that the AI can perform, the current world state and the desired world state (goal state). 
GOAP will then attempt to find a series of actions that will get the AI to the goal state.

GOAP was made popular by the old FPS F.E.A.R. 
The AI in F.E.A.R. consisted of a GOAP and a state machine with just 3 states: GoTo, Animate, UseSmartObject. 
Jeff Orkin's [web page](http://alumni.media.mit.edu/~jorkin/goap.html) is a treasure trove of great information.


## ActionPlanner
The brains of the operation. 
You give the ActionPlanner all of your Actions, the current world state and your goal state and it will give you back the best possible plan to achieve the goal state.


## Action/ActionT
Actions define a list of pre conditions that they require and a list of post conditions that will occur when the Action is performed. 
ActionT is just a subclass of Action with a handy context object of type T.


## Agent
Agent is a helper class that encapsulates an AI agent. 
It keeps a list of available Actions and a reference to the ActionPlanner. 
Agent is abstract and requires you to define the `GetWorldState` and `GetGoalState` methods. 
With those in place getting a plan is as simple as calling `agent.Plan()`.



Utility Based AI
==========
Utility Theory for games. 
The most complex of the AI solutions. 
Best used in very dynamic environments where its scoring system works best. 
Utility based AI are more appropriate in situations where there are a large number of potentially competing actions the AI can take such as in a RTS. 
A great overview of utility AI is [available here](http://www.gdcvault.com/play/1012410/Improving-AI-Decision-Modeling-Through).


## Reasoner
Selects the best Consideration from a list of Considerations attached to the Reasoner. The root of a utility AI.


## Consideration
Houses a list of Appraisals and an Action. Calculates a score that represents numerically the utility of its Action.


## Appraisal
One or more Appraisals can be added to a Consideration. They calculate and return a score which is used by the Consideration.


## Action
The action that the AI executes when a specific Consideration is selected.
