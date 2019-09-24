using System;
using System.Collections.Specialized;
using System.Configuration;

namespace tmg.equinox.identitymanagement
{
    internal static class SecurityConfig
    {
        #region Private Memebers
        private static readonly NameValueCollection securitySection;
        private static readonly bool HideOrDisableSections;
        private static readonly bool HideContainer;
        private static readonly bool EnableFolderLockSettings;
        private static readonly bool SetDefaultPermissions;
        private static readonly bool StopScrollFloatingHeaders;
        #endregion Private Members

        #region Constructor
        static SecurityConfig()
        {
            securitySection = (NameValueCollection)ConfigurationManager.GetSection("security");
            HideOrDisableSections = Convert.ToBoolean(securitySection["HideOrDisableSection"] == "true");
            HideContainer = Convert.ToBoolean(securitySection["HideContainer"] == "true");
            EnableFolderLockSettings = Convert.ToBoolean(securitySection["FolderLockSettings"] == "true");
            SetDefaultPermissions = Convert.ToBoolean(securitySection["SetDefaultPermissions"] == "true");
            StopScrollFloatingHeaders = Convert.ToBoolean(securitySection["StopScrollFloatingHeader"] == "true");
        }
        #endregion Constructor

        #region Public Properties
        internal static bool IsHiddenOrDisableSections
        {
            get
            {
                return HideOrDisableSections;
            }
        }
        internal static bool IsHiddenContainer
        {
            get
            {
                return HideContainer;
            }
        }
        internal static bool IsEnableFolderLockSettings
        {
            get 
            {
                return EnableFolderLockSettings;
            }
        }
        internal static bool IsDefaultPermissionsApplicable
        {
            get
            {
                return SetDefaultPermissions;
            }
        }
        internal static bool IsStopScrollFloatingHeaders
        {
            get
            {
                return StopScrollFloatingHeaders;
            }
        }
        #endregion Public Properties

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods


    }
}
