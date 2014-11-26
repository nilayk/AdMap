using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace AdMap
{
	public class AdMapUpdater
	{
		private readonly static Lazy<AdMapUpdater> _instance = new Lazy<AdMapUpdater>(() => new AdMapUpdater(GlobalHost.ConnectionManager.GetHubContext<AdMapUpdaterHub>().Clients));

		private readonly ConcurrentDictionary<string, Ad> _ads = new ConcurrentDictionary<string, Ad>();

		private readonly object _updateAdMapLock = new object();

		private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(400);

		private readonly Timer _timer;
		private volatile bool _updatingAdMap = false;

		private readonly Random RandomNumber = new Random();

		private AdMapUpdater(IHubConnectionContext<dynamic> clients)
		{
			Clients = clients;

			_ads.Clear();
			var ads = new List<Ad>
				{
					new Ad {City = "Delhi", Latitude = 28.6139, Longitude = 77.2089},
					new Ad {City = "New York", Latitude = 40.7528, Longitude = -73.9725},
					new Ad {City = "Tokyo", Latitude = 35.6895, Longitude = 139.6917},
					new Ad {City = "London", Latitude = 51.5072, Longitude = 0.1275},
					new Ad {City = "Los Angeles", Latitude = 34.0396, Longitude = -118.2661}
				};
			ads.ForEach(ad => _ads.TryAdd(ad.City, ad));

			_timer = new Timer(UpdateAdMap, null, _updateInterval, _updateInterval);
		}

		public static AdMapUpdater Instance
		{
			get { return _instance.Value; }
		}

		public IEnumerable<Ad> InitAdMap()
		{
			return _ads.Values;
		}

		private IHubConnectionContext<dynamic> Clients { get; set; }

		private void UpdateAdMap(object state)
		{
			lock (_updateAdMapLock)
			{
				if (!_updatingAdMap)
				{
					_updatingAdMap = true;
					var ad = new Ad();
					if (TryGetAd(ad))
					{
						BroadcastAdMap(ad);
					}
					_updatingAdMap = false;
				}
			}
		}

		private bool TryGetAd(Ad ad)
		{
			ad.City = Guid.NewGuid().ToString();
			ad.Latitude = RandomNumber.NextDouble() * 360 - 180;
			ad.Longitude = RandomNumber.NextDouble() * 360 - 180;
			return true;
		}

		private void BroadcastAdMap(Ad ad)
		{
			Clients.All.updateAdMap(ad);
		}
	}
}