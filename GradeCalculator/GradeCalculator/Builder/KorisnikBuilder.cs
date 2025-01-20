using GradeCalculator.ViewModels;

namespace GradeCalculator.Builder
{
    public class KorisnikBuilder
    {
        private ShowKorisnikVM _viewModel;

        public KorisnikBuilder()
        {
            _viewModel = new ShowKorisnikVM();
        }

        public KorisnikBuilder SetId(int id)
        {
            _viewModel.Id = id;
            return this;
        }

        public KorisnikBuilder SetUserName(string userName)
        {
            _viewModel.UserName = userName;
            return this;
        }

        public KorisnikBuilder SetEmail(string email)
        {
            _viewModel.Email = email;
            return this;
        }

        public KorisnikBuilder SetTotalGrade(double totalGrade)
        {
            _viewModel.TotalGrade = totalGrade;
            return this;
        }

        public KorisnikBuilder SetRoleName(string roleName)
        {
            _viewModel.RoleName = roleName;
            return this;
        }

        public ShowKorisnikVM Build()
        {
            return _viewModel;
        }
    }
}
