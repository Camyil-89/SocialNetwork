namespace SocialNetwork.Models
{
	[Serializable]
	public class User
	{
		public string Id { get; set; }
		public string Name_1 { get; set; }
		public string Name_2 { get; set; }
		public string Name_3 { get; set; }
		public string ImageUrl { get; set; } = "/images/no_image.png";

		public List<User> Friends { get; set; } = new List<User>();
	}
}
