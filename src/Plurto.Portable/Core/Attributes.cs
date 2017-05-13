// -----------------------------------------------------------------------
// <copyright file="Attributes.cs" company="Ting-Yu Lin">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Plurto.Core
{
    /// <summary>
    /// This command requires login for API 1.0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequireLoginAttribute : Attribute
    {
    }

    /// <summary>
    /// This command should be using HTTPS for API 1.0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SecureRequestAttribute : Attribute
    {
    }

    /// <summary>
    /// This command can only be used with LegacyClient
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class LegacyCommandAttribute : Attribute
    {
    }

    /// <summary>
    /// This command is only available for API 2.0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Api2Attribute : Attribute
    {
    }

    /// <summary>
    /// This command requires user's access token for OAuth.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequireAccessTokenAttribute : Attribute
    {
    }

    /// <summary>
    /// This member is not available if it's received from legacy commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class NotAvailableOnLegacyAttribute : Attribute
    {
    }
}
