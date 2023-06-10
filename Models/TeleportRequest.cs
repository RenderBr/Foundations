using TShockAPI;

namespace Foundations
{
	public class TeleportRequest
	{
		public TSPlayer Requester { get; set; }
		public TSPlayer Target { get; set; }
		public DateTime RequestTime { get; set; }
		public bool Accepted { get; set; }

	}
}
