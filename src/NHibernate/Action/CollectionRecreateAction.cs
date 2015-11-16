using System;
using System.Diagnostics;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Persister.Collection;
using NHibernate.Cache;
using NHibernate.Cache.Entry;

namespace NHibernate.Action
{
	[Serializable]
	public sealed class CollectionRecreateAction : CollectionAction
	{
		public CollectionRecreateAction(IPersistentCollection collection, ICollectionPersister persister, object key, ISessionImplementor session)
			: base(persister, collection, key, session) { }

		/// <summary> Execute this action</summary>
		/// <remarks>
		/// This method is called when a new non-null collection is persisted
		/// or when an existing (non-null) collection is moved to a new owner
		/// </remarks>
		public override void Execute()
		{
			bool statsEnabled = Session.Factory.Statistics.IsStatisticsEnabled;
			Stopwatch stopwatch = null;
			if (statsEnabled)
			{
				stopwatch = Stopwatch.StartNew();
			}
			IPersistentCollection collection = Collection;

			PreRecreate();

			Persister.Recreate(collection, Key, Session);

			Session.PersistenceContext.GetCollectionEntry(collection).AfterAction(collection);

			Evict();

			PostRecreate();
			if (statsEnabled)
			{
				stopwatch.Stop();
				Session.Factory.StatisticsImplementor.RecreateCollection(Persister.Role, stopwatch.Elapsed);
			}
		}

		private void PreRecreate()
		{
			IPreCollectionRecreateEventListener[] preListeners = Session.Listeners.PreCollectionRecreateEventListeners;
			if (preListeners.Length > 0)
			{
				PreCollectionRecreateEvent preEvent = new PreCollectionRecreateEvent(Persister, Collection, (IEventSource)Session);
				for (int i = 0; i < preListeners.Length; i++)
				{
					preListeners[i].OnPreRecreateCollection(preEvent);
				}
			}
		}

		private void PostRecreate()
		{
			IPostCollectionRecreateEventListener[] postListeners = Session.Listeners.PostCollectionRecreateEventListeners;
			if (postListeners.Length > 0)
			{
				PostCollectionRecreateEvent postEvent = new PostCollectionRecreateEvent(Persister, Collection, (IEventSource)Session);
				for (int i = 0; i < postListeners.Length; i++)
				{
					postListeners[i].OnPostRecreateCollection(postEvent);
				}
			}
		}

        public override AfterTransactionCompletionProcessDelegate AfterTransactionCompletionProcess
		{
			get
			{
				return new AfterTransactionCompletionProcessDelegate((success) =>
				{
					// NH Different behavior: to support unlocking collections from the cache.(r3260)
					if (Persister.HasCache)
					{
						CacheKey ck = Session.GenerateCacheKey(Key, Persister.KeyType, Persister.Role);

						if (success)
						{
							// we can't disassemble a collection if it was uninitialized 
							// or detached from the session
							if (Collection.WasInitialized && Session.PersistenceContext.ContainsCollection(Collection))
							{
								CollectionCacheEntry entry = new CollectionCacheEntry(Collection, Persister);
								bool put = Persister.Cache.AfterUpdate(ck, entry, null, Lock);
		
								if (put && Session.Factory.Statistics.IsStatisticsEnabled)
								{
									Session.Factory.StatisticsImplementor.SecondLevelCachePut(Persister.Cache.RegionName);
								}
							}
						}
						else
						{
							Persister.Cache.Release(ck, Lock);
						}
					}
				});
			}
		}

	}
}