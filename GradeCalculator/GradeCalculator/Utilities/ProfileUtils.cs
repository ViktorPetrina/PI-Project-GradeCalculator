namespace GradeCalculator.Utilities
{
    public class ProfileUtils
    {
        public static string GetLoggedInUserId(HttpContext httpContext) 
        {
            var id= httpContext.User.Identity.Name;
            return id;
        }
    }
}
