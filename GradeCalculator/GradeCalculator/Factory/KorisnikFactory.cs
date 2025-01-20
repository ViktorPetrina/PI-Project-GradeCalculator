namespace GradeCalculator.Factory
{
    public class KorisnikFactory
    {
        public static IKorisnikFactory GetKorisnik(string role) 
        {
            switch (role.ToLower())
            {
                case "basic":
                    return new BasicKorisnik();
                case "admin":
                    return new AdminKorisnik();
                default:
                    throw new Exception("Invalid user type");
            }
        }
    }
}
