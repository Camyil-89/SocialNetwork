
namespace SocialNetwork.Models
{
	public class Chat
	{
		public string Id { get; set; }
		public string IdUserAdmin { get; set; }
		public List<User> Users { get; set; } = new List<User>();
		public List<Message> Messages { get; set; } = new List<Message>();
	}

	public class Message
	{
		public string Id { get; set; }
		public string IdChat { get; set; }
		public string IdUser { get; set; }
		public string Text { get; set; }
		public object File { get; set; }
	}
}
