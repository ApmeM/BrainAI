namespace BrainAI.AI.UtilityAI
{
    using System;
    using System.Collections;
    using System.Linq;
    using BrainAI.AI;

    public class Coroutine : IAITurn
    {
        public static IEnumerator Wait(float seconds)
        {
            var startAt = DateTime.Now;
            while ((DateTime.Now - startAt).TotalSeconds < seconds)
            {
                yield return null;
            }
        }

        public static IEnumerator Empty()
        {
            yield return null;
        }

        public static IEnumerator ParallelCoroutines(params IEnumerator[] enumerators)
        {
            var coroutines = enumerators.Select(a => new Coroutine(a)).ToList();
            if (coroutines.Count == 0)
            {
                yield break;
            }

            do
            {
                foreach (var cor in coroutines)
                {
                    cor.Tick();
                }

                coroutines.RemoveAll(a => a.IsDone);
                if (coroutines.Any())
                {
                    yield return null;
                }
            } while (coroutines.Any());
        }

        public readonly IEnumerator enumerator;

        public bool IsDone = false;

        private Coroutine waitForCoroutine = null;

        /// <summary>
        /// Basic CoroutineManager. Coroutines can do the following:
        /// - yield return null (tick again the next frame)
        /// - yield return IEnumerator (tick again after dependent enumerator finished, enumerator will start in the same tick)
        /// - yield return Coroutine (tick again after dependent coroutine finished, coroutine will start in the same tick)
        /// To stop coroutine before it is finished set IsDone = true;
        /// </summary>
        public Coroutine(IEnumerator enumerator)
        {
            this.enumerator = enumerator;
        }

        public void Tick()
        {
            var coroutine = this;

            if (coroutine.IsDone)
            {
                return;
            }

            while (true)
            {
                if (coroutine.waitForCoroutine != null)
                {
                    if (coroutine.waitForCoroutine.IsDone == true)
                    {
                        coroutine.waitForCoroutine = null;
                    }
                    else
                    {
                        coroutine.waitForCoroutine.Tick();
                        if (!coroutine.waitForCoroutine.IsDone)
                        {
                            return;
                        }

                        coroutine.waitForCoroutine = null;
                    }
                }

                if (!coroutine.enumerator.MoveNext())
                {
                    coroutine.IsDone = true;
                    return;
                }

                if (coroutine.enumerator.Current == null)
                {
                    return;
                }

                if (coroutine.enumerator.Current is Coroutine current)
                {
                    coroutine.waitForCoroutine = current;
                }
                else if (coroutine.enumerator.Current is IEnumerator enumerator)
                {
                    coroutine.waitForCoroutine = new Coroutine(enumerator);
                }
            }
        }
    }
}

