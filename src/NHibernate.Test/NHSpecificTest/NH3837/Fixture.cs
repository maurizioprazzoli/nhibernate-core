using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.Linq;
using System.IO;

namespace NHibernate.Test.NHSpecificTest.NH3837
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
        public override string BugNumber
        {
            get
            {
                return "NH3837";
            }
        }

        protected override void Configure(Cfg.Configuration configuration)
        {
            cfg.SetProperty("cache.use_second_level_cache", "true");
        }

        protected override void ApplyCacheSettings(Cfg.Configuration configuration)
        {

        }

        [Test]
        public void AfterInsertingEntitiesAndCollectionNoSQLSelectShouldHaveBeenIssued()
        {
            Guid item_guid = Guid.NewGuid();

            Guid bid1_guid = Guid.NewGuid();
            Guid bid2_guid = Guid.NewGuid();

            using (ISession session = OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    // item
                    Item item = new Item();
                    item.Id = item_guid;
                    item.Description = "ItemNote";

                    // bid1
                    Bid bid1 = new Bid();
                    bid1.Id = bid1_guid;
                    bid1.Description = "Bid1Description";
                    bid1.Item = item;

                    // bid2
                    Bid bid2 = new Bid();
                    bid2.Id = bid2_guid;
                    bid2.Description = "Bid2Description";
                    bid2.Item = item;

                    item.AddBid(bid1);
                    item.AddBid(bid2);

                    session.Save(item);

                    tx.Commit();
                }
            }

            using (ISession session = OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
                    {
                        session.Get<Item>(item_guid);
                        tx.Commit();

                        Assert.AreEqual(0, GetNumberOfSelectStatements(sqlLogSpy), "No SQL select statements should have been issued");
                    }
                }
            }
        }

        [Test]
        public void AfterInsertingAndUpdateEntitiesAndCollectionNoSQLSelectShouldHaveBeenIssued()
        {
            Guid item_guid = Guid.NewGuid();

            using (ISession session = OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    // item
                    Item item = new Item();
                    item.Id = item_guid;
                    item.Description = "ItemNote";

                    // bid1
                    Bid bid1 = new Bid();
                    bid1.Id = Guid.NewGuid(); ;
                    bid1.Description = "Bid1Description";
                    bid1.Item = item;

                    // bid2
                    Bid bid2 = new Bid();
                    bid2.Id = Guid.NewGuid();
                    bid2.Description = "Bid2Description";
                    bid2.Item = item;

                    item.AddBid(bid1);
                    item.AddBid(bid2);

                    session.Save(item);

                    tx.Commit();
                }
            }

            using (ISession session = OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
                    {
                        Item item = session.Get<Item>(item_guid);

                        // bid3
                        Bid bid3 = new Bid();
                        bid3.Id = Guid.NewGuid();
                        bid3.Description = "Bid3Description";
                        bid3.Item = item;

                        item.AddBid(bid3);
                        tx.Commit();

                        Assert.AreEqual(0, GetNumberOfSelectStatements(sqlLogSpy), "No SQL select statements should have been issued");
                    }
                }
            }
        }

        protected override void OnTearDown()
        {
            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                session.Delete("from Item");
                session.Delete("from Bid");
                tx.Commit();
            }
        }

        private Int32 GetNumberOfSelectStatements(SqlLogSpy sqlLogSpy)
        {
            Int32 numberOfSelectStatements = 0;
            TextWriter wr = new StringWriter();

            foreach (var sqlEvent in sqlLogSpy.Appender.GetEvents())
            {
                sqlEvent.WriteRenderedMessage(wr);
                if (wr.ToString().TrimStart().StartsWith("SELECT"))
                {
                    numberOfSelectStatements++;
                }
            }
            return numberOfSelectStatements;
        }
    }
}
