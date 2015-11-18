using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3837
{
	public class Item
    {
        public Guid Id { get; set; }

        public Int32 Version { get; private set; }

        public string Description { get; set; }

        public IList<Bid> Bids { get; set; }

        internal void AddBid(Bid bid)
        {
            if (Bids == null)
            {
                Bids = new List<Bid>();
            }

            Bids.Add(bid);

        }
    }

    public class Bid
    {
        public Guid Id { get; set; }
        
        public Int32 Version { get; private set; }

        public string Description { get; set; }

        public Item Item { get; set; }
    }
}
