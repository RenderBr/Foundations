using Auxiliary;

namespace Foundations.Models
{
	public class Departure : BsonModel
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

		private string _greeting = String.Empty;

		public string GreetingMsg
		{
			get
				=> _greeting;

			set
			{
				_ = this.SaveAsync(x => x.GreetingMsg, value);
				_greeting = value;
			}
		}

		private string _departure = String.Empty;

		public string DepartureMsg
		{
			get
				=> _departure;

			set
			{
				_ = this.SaveAsync(x => x.DepartureMsg, value);
				_departure = value;
			}
		}

	}
}
