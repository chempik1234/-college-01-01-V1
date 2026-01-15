namespace AppTkani.DataModel
{
	public class User
    {
        public int UserId { get; set; }
        public string UserSurname { get; set; }
        public string UserName { get; set; }
        public string UserPatronymic { get; set; }
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }
        = string.Empty;

        public int UserRole { get; set; }
    }
}
