using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor.Tools
{
    public static class UWPPlatformTool
    {
        public static bool IsMobile
        {
            get
            {
                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToLower().Equals("windows.mobile"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
