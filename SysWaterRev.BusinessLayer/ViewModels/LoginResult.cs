namespace SysWaterRev.BusinessLayer.ViewModels
{
    public class LoginResult
    {
        public bool IsLoggedIn { get; set; }

        public string ErrorMessage { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string UserName { get; set; }

        public string Role { get; set; }

        public string EmployeeNumber { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string Surname { get; set; }

        public string Gender { get; set; }
    }
}