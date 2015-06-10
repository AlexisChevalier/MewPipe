namespace MewPipe.DataFeeder.Entities
{
	public class ExcelUser
	{
		public int Index { get; set; }
		public string FullName { get; set; } //i.e "John Doe"
		public int VideoGameInterest { get; set; }
		public int SportInterest { get; set; }
		public int MusicInterest { get; set; }

		#region Extra getters

		public string FirstName
		{
			get { return FullName.Split(null)[0]; } //i.e "John"
		}

		public string LastName
		{
			get { return FullName.Split(null)[1]; } //i.e "Doe"
		}

		public string UserName
		{
			get { return FirstName.Substring(0, 1).ToLower() + LastName; } //i.e "jDoe"
		}

		public string Email
		{
			get { return UserName.ToLower() + "@gmail.com"; } //i.e "jdoe@gmail.com"
		}

		#endregion
	}
}