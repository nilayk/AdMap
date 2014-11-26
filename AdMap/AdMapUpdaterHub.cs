using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace AdMap
{
	[HubName("adMapUpdaterHub")]
	public class AdMapUpdaterHub : Hub
	{
		private readonly AdMapUpdater _adMapUpdater;

		public AdMapUpdaterHub() : this(AdMapUpdater.Instance) { }

		public AdMapUpdaterHub(AdMapUpdater adMapUpdater)
		{
			_adMapUpdater = adMapUpdater;
		}

		public IEnumerable<Ad> InitAdMap()
		{
			return _adMapUpdater.InitAdMap();
		}
	}
}