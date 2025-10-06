namespace Zenkoi.DAL.Entities
{
	public class RefreshToken
	{
		public Guid Id { get; set; }
		public int UserId { get; set; }
		public string JwtId { get; set; }
		public string Token { get; set; }
		public bool IsUsed { get; set; }
		public bool IsRevoked { get; set; }
		public DateTime IssuedAt { get; set; }
		public DateTime ExpiredAt { get; set; }

		public virtual ApplicationUser ApplicationUser { get; set; }
	}
}
