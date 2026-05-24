using IT_Helpdesk.Models;

namespace IT_Helpdesk
{
    public static class SessionManager
    {
        public static User? CurrentUser { get; private set; }
        public static int CurrentLogID { get; private set; }

        public static void StartSession(User user, int logId)
        {
            CurrentUser = user;
            CurrentLogID = logId;
        }

        public static void EndSession()
        {
            CurrentUser = null;
            CurrentLogID = 0;
        }

        public static bool IsAuthenticated => CurrentUser != null;
        public static string? CurrentRole => CurrentUser?.Role;
    }
}
