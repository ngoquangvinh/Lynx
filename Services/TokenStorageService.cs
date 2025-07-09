using System.IO;

namespace LynxUI_Main.Services
{
    public static class TokenStorage
    {
        private static string tokenFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "lynx_token.txt"
        );

        public static void SaveToken(string token)
        {
            File.WriteAllText(tokenFile, token);
        }

        public static string? LoadToken()
        {
            return File.Exists(tokenFile) ? File.ReadAllText(tokenFile) : null;
        }

        public static void ClearToken()
        {
            if (File.Exists(tokenFile))
                File.Delete(tokenFile);
        }
    }

}
