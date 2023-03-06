using System.Threading.Tasks;

namespace CAE.Demo
{
    public class Userinfo : NotifyModel
    {
        private string username = "cae";
        private string password = "123";
        private string serverAddress = "192.168.12.131";
        private int? port = 8010;
        private string token;
        private bool isAdmin = false;
        private string newPassword;

        public string Username { get => username; set { username = value; OnPropertyChanged(); } }

        public string Password { get => password; set { password = value; OnPropertyChanged(); } }

        public string IP { get => serverAddress; set { serverAddress = value; OnPropertyChanged(); } }

        public int? Port { get => port; set { port = value; OnPropertyChanged(); } }

        public string Token { get => token; set { token = value; OnPropertyChanged(); IsAdmin = !string.IsNullOrEmpty(token) && Username == "Admin"; } }

        public bool IsAdmin { get => isAdmin; set { isAdmin = value; OnPropertyChanged(); } }

        public string NewPassword { get => newPassword; set { newPassword = value;OnPropertyChanged(); } }

        public string AddUserName { get; set; }
        public string AddPassword { get; set; }
    }
}