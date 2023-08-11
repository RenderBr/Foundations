using Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundations.Models
{
	public class PlayerTime : BsonModel
	{
		public bool Day { get; set; }
		public int Frames { get; set;}
		public bool Enabled { get; set; }
	}
	
	public enum Time : int
	{
		NOON = 27000,
		DAY = 0,
		NIGHT = 0,
		MIDNIGHT = 16200
	}
}
