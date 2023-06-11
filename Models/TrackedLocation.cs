using Auxiliary;

namespace Foundations.Models
{
	public class TrackedLocation : BsonModel
	{
		private string _accountName = String.Empty;
		public string AccountName
		{

			get
				=> _accountName;

			set
			{
				_ = this.SaveAsync(x => x.AccountName, value);
				_accountName = value;
			}
		}

		private float _x;

		public float X
		{
			get
				=> _x;

			set
			{
				_ = this.SaveAsync(x => x.X, value);
				_x = value;
			}
		}

		private float _y;

		public float Y
		{
			get
				=> _y;

			set
			{
				_ = this.SaveAsync(x => x.Y, value);
				_y = value;
			}
		}

		private DateTime _time;

		public DateTime Time
		{
			get
		=> _time;

			set
			{
				_ = this.SaveAsync(x => x.Time, value);
				_time = value;
			}
		}

		private bool _reverted;
		
		public bool Reverted
		{
			get
				=> _reverted;

			set
			{
				_ = this.SaveAsync(x => x.Reverted, value);
				_reverted = value;
			}
		}

	}
}
