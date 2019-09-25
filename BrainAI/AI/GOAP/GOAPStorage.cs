namespace BrainAI.AI.GOAP
{
    using System;

    public class GOAPStorage
    {
        // The maximum number of nodes we can store
        const int MaxNodes = 128;

        private readonly GOAPNode[] opened = new GOAPNode[MaxNodes];
        private readonly GOAPNode[] closed = new GOAPNode[MaxNodes];

        private int numOpened;
        private int numClosed;

        private int lastFoundOpened;
        private int lastFoundClosed;

        internal GOAPStorage()
        {
        }

        public void Clear()
        {
            for( var i = 0; i < this.numOpened; i++ )
            {
                this.opened[i] = null;
            }

            for( var i = 0; i < this.numClosed; i++ )
            {
                this.closed[i] = null;
            }

            this.numOpened = this.numClosed = 0;
            this.lastFoundClosed = this.lastFoundOpened = 0;
        }


        public GOAPNode FindOpened( GOAPNode node )
        {
            for( var i = 0; i < this.numOpened; i++ )
            {
                var care = node.WorldState.DontCare ^ -1L;
                if( ( node.WorldState.Values & care ) == ( this.opened[i].WorldState.Values & care ) )
                {
                    this.lastFoundClosed = i;
                    return this.closed[i];
                }
            }
            return null;
        }


        public GOAPNode FindClosed( GOAPNode node )
        {
            for( var i = 0; i < this.numClosed; i++ )
            {
                long care = node.WorldState.DontCare ^ -1L;
                if( ( node.WorldState.Values & care ) == ( this.closed[i].WorldState.Values & care ) )
                {
                    this.lastFoundClosed = i;
                    return this.closed[i];
                }
            }
            return null;
        }


        public bool HasOpened()
        {
            return this.numOpened > 0;
        }


        public void RemoveOpened( GOAPNode node )
        {
            if( this.numOpened > 0 )
                this.opened[this.lastFoundOpened] = this.opened[this.numOpened - 1];
            this.numOpened--;
        }


        public void RemoveClosed( GOAPNode node )
        {
            if( this.numClosed > 0 )
                this.closed[this.lastFoundClosed] = this.closed[this.numClosed - 1];
            this.numClosed--;
        }


        public bool IsOpen( GOAPNode node )
        {
            return Array.IndexOf( this.opened, node ) > -1;
        }


        public bool IsClosed( GOAPNode node )
        {
            return Array.IndexOf( this.closed, node ) > -1;
        }


        public void AddToOpenList( GOAPNode node )
        {
            this.opened[this.numOpened++] = node;
        }


        public void AddToClosedList( GOAPNode node )
        {
            this.closed[this.numClosed++] = node;
        }


        public GOAPNode RemoveCheapestOpenNode()
        {
            var lowestVal = int.MaxValue;
            this.lastFoundOpened = -1;
            for( var i = 0; i < this.numOpened; i++ )
            {
                if( this.opened[i].CostSoFarAndHeuristicCost < lowestVal )
                {
                    lowestVal = this.opened[i].CostSoFarAndHeuristicCost;
                    this.lastFoundOpened = i;
                }
            }
            var val = this.opened[this.lastFoundOpened];
            this.RemoveOpened( val );

            return val;
        }
    
    }
}

