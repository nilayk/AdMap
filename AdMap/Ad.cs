using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdMap
{
	public class Ad
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public string City { get; set; }
		public Uri MediaUrl { get; set; }
	}
}