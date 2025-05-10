using System.Windows;

namespace LynxUI_Main.Helpers
{
    public static class FreezableHelper
    {
        /// <summary>
        /// Attempts to freeze a Freezable safely. If freezing fails, it silently ignores.
        /// </summary>
        public static T SafeFreeze<T>(this T freezable) where T : Freezable
        {
            try
            {
                if (freezable != null && freezable.CanFreeze)
                {
                    freezable.Freeze();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SafeFreeze] Cannot freeze object: {ex.Message}");
            }
            return freezable;
        }
    }
}
