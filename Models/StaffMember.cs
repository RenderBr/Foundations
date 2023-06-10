using TShockAPI;

namespace Foundations.Models
{
	public class StaffMember
	{
		public TSPlayer Player { get; set; }
		public Group Rank { get; set; }

		public StaffMember(TSPlayer player, Group rank)
		{
			Player = player;
			Rank = rank;
		}

	}
}
