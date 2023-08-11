using Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundations.Models
{
	public class FoundationUser : BsonModel
	{
		private string _accName = String.Empty;

		public string Account
		{
			get
				=> _accName;

			set
			{
				_ = this.SaveAsync(x => x.Account, value);
				_accName = value;
			}
		}

		private PlayerTime _ptime = new();
		public PlayerTime PlayerTime
		{
			get
				=> _ptime;

			set
			{
				_ = this.SaveAsync(x => x.PlayerTime, value);
				_ptime = value;
			}
		}
	}
}
