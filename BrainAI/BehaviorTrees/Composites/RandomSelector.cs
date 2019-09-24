namespace BrainAI.BehaviorTrees.Composites
{
    /// <summary>
    /// Same as Selector except it shuffles the children when started
    /// </summary>
    public class RandomSelector<T> : Selector<T>
    {
        public override void OnStart()
        {
            var n = this.Children.Count;
            while (n > 1)
            {
                n--;
                int k = Configuration.Random.Range(0, n + 1);
                var value = this.Children[k];
                this.Children[k] = this.Children[n];
                this.Children[n] = value;
            }
        }
    }
}

