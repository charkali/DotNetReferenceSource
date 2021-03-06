#include "Utils.hxx"
#include <shlwapi.h>

#if _MANAGED
using namespace System::Security;
using namespace System::Security::Permissions;
#endif

// These constants are cloned in
// wpf\src\Shared\MS\Internal\Registry.cs
// Should these reg keys change the above file should be also modified to reflect that.
#define FRAMEWORK_REGKEY        L"Software\\Microsoft\\Net Framework Setup\\NDP\\v4\\Client"
#define FRAMEWORK_INSTALLPATH_REGVALUE L"InstallPath"
#define WPF_SUBDIR              L"WPF"

#define DOTNET_FRAMEWORK_REGKEY L"Software\\Microsoft\\.NETFramework"
#define DOTNET_FRAMEWORK_INSTALLROOT_REGVALUE L"InstallRoot"

#define COMPLUS_Version         L"COMPLUS_Version"
#define COMPLUS_InstallRoot     L"COMPLUS_InstallRoot"

namespace WPFUtils {


//
// Reads a string value from the registry.
// If the function succeeds, the return value is ERROR_SUCCESS.
// If the function fails, the return value is a nonzero error code defined in Winerror.h
//
// <SecurityNote>
// Critical -- Calls native methods RegOpenKeyEx, RegQueryValueEx, and RegCloseKey
// </SecurityNote>
#if _MANAGED
[SecurityCritical]
#endif
LONG ReadRegistryString(__in HKEY rootKey, __in LPCWSTR keyName, __in LPCWSTR valueName,
                                     __out LPWSTR value, size_t cchMax)
{
    HKEY key = NULL;

    LONG result = RegOpenKeyEx(rootKey, keyName, 0, KEY_READ, &key);

    if (result == ERROR_SUCCESS)
    {
        if( cchMax > INT_MAX)
        {
            result = ERROR_INVALID_PARAMETER;
        }
        else
        {
            DWORD sizeInBytes = static_cast<DWORD>(cchMax) * sizeof(WCHAR);
            DWORD type;

            result = RegQueryValueEx(key, valueName, NULL, &type, (LPBYTE)value, &sizeInBytes);

            if (result == ERROR_SUCCESS && type != REG_SZ)
            {
                result = ERROR_UNSUPPORTED_TYPE;
            }

            RegCloseKey(key);
        }
    }

    return result;
}

#if _MANAGED
[SecurityCritical]
[SecurityPermission(SecurityAction::Assert, UnmanagedCode=true)]
#endif
// Dev11:#158013: Warning 4714 (__forceinline function not inlined)
// is expected here because WPFUtils::GetWPFInstallPath is marked with [SecurityCritical]
// and tries to inline HRESULT_FROM_WIN32.
// Starting with changeset 172903 (see also Dev11 bugs 4172, 134965, 134979),
// inlining is prevented when the caller or the callee
// are marked with any security attribute (critical, safecritical, treatassafecritical).
// This is over conservative and misses inlining opportunities occasionaly,
// but currently there is no way of determining accurately the transparency level of a function
// in the native compiler since there are no public APIs provided by CLR at the moment.
// Replicating CLR transparency rules on the native side is not ideal either.
// The solution chosen is to allow inlining only when there is clear evidence
// for the caller and the callee to be transparent.
#pragma warning (push)
#pragma warning (disable : 4714)

HRESULT GetWPFInstallPath(__out_ecount(cchMaxPath) LPWSTR pszPath, size_t cchMaxPath)
{
    HRESULT hr = S_OK;
    DWORD ch;
    WCHAR wszVersion[MAX_PATH];

    // The PathAppend function doesn't handle small buffers.
    if(cchMaxPath < MAX_PATH)
        return E_OUTOFMEMORY;

    // We support a "private CLR" which allows someone to use a different framework
    // location than what is specified in the registry.  The CLR support for this
    // involves two environment variable: COMPLUS_InstallRoot and COMPLUS_Version.
    //
    // The full path to the WPF assemblies is:
    // %COMPLUS_InstallRoot%\%COMPLUS_Version%\wpf

    // Check for the COMPLUS_Version environment variable.
    // 



    ch = GetEnvironmentVariableW(COMPLUS_Version, wszVersion, MAX_PATH);
    if (ch > 0)
    {
        // Check for the COMPLUS_InstallRoot environment variable.
        ch = GetEnvironmentVariableW(COMPLUS_InstallRoot, pszPath, static_cast<DWORD>(cchMaxPath));
        if (ch <= 0)
        {
            // The COMPLUS_Version environment variable was set, but the
            // COMPLUS_InstallRoot environment variable was not.  We fall back
            // to getting the framework install root from the registry, but
            // still use the private CLR version.
            LONG result = ReadRegistryString(HKEY_LOCAL_MACHINE, DOTNET_FRAMEWORK_REGKEY, DOTNET_FRAMEWORK_INSTALLROOT_REGVALUE, pszPath, static_cast<DWORD>(cchMaxPath));
            if (result != ERROR_SUCCESS)
            {
                hr = HRESULT_FROM_WIN32(result);
            }
        }

        // Append the InstallRoot and the Version
        if(SUCCEEDED(hr))
        {
#pragma prefast(suppress:25025, "We don't know of a better API to use in place of PathAppend. The OACR spreadsheet and MSDN do not suggest any either.")
            if (!::PathAppend(pszPath, wszVersion))
            {
                hr = E_OUTOFMEMORY;
            }
        }
    }
    else
    {
        // The COMPLUS_Version environment variable was not set.  We do not support
        // extracting the appropriate version ourselves, since this could come from
        // various places (app config, etc), so we default to 4.0.  The entire path
        // is stored in the registry, under the v4 key.
        LONG result = ReadRegistryString(HKEY_LOCAL_MACHINE, FRAMEWORK_REGKEY, FRAMEWORK_INSTALLPATH_REGVALUE, pszPath, cchMaxPath);
        if (result != ERROR_SUCCESS)
        {
            hr = HRESULT_FROM_WIN32(result);
        }
    }

    // WPF chose to make a subdirectory for its own DLLs under the framework directory.
    if (SUCCEEDED(hr))
    {
#pragma prefast(suppress:25025, "We don't know of a better API to use in place of PathAppend. The OACR spreadsheet and MSDN do not suggest any either.")
        if (!::PathAppend(pszPath, WPF_SUBDIR))
        {
            hr = E_OUTOFMEMORY;
        }
    }

    return hr;
}
#pragma warning (pop)

}//namespace
