using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Diagnostics;
using System.ComponentModel;

namespace FFramework.FUtilities
{
    public class RunAsAdmin
    {
        ///<summary>Runs application with Administrator rights. NOTE: After using this function, have in mind you should use "Environment.Exit(x)" or "Application.Exit()" in order to successfully start an application with Administrator rights.</summary>
        ///<param name="process_name">A path for executable you want to start with Administrator rights. NOTE: The best way to use it is "Application.ExecutablePath" as this parameter</param>
        ///<returns>(nothing)</returns>
        public static void RunAppWithAdminRights(string executable_path)
        {
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!hasAdministrativeRight) RunElevated(executable_path);
        }

        ///<summary>Internal function, usage not advised!</summary>
        public static bool RunElevated(string fileName)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.Verb = "runas";
            processInfo.FileName = fileName;
            try
            {
                Process.Start(processInfo);
                return true;
            }
            catch (Win32Exception) { }
            return false;
        }
    }
}
