# Windows API Helper

This library was created to assist me in some of my other projects where I
required the use of windows specific api functions. The main function of this
library is the concept of impersonation.

## Impersonation

Windows Impersonation allows for the executing code to act as a different
windows user. The Impersonator class in this library has two ways to get the
impersonation context.

The most straight forward method allows for the caller to provide a username,
password, and domain in the constructor. This method utilizes the LogonUserA
function call to get a primary token from windows. This token is duplicated and
then used to start the impersonation process.

The alternative method of impersonation is designed to be used in a special
case. Specifically when it is desired to impersonate the current logged on user.
This is useful when running an application from a service account or another
instance where the application could be running as a different user.

### Impersonator Examples

The Impersonator class implements the IDisposable interface and is designed to
be used within a using block where anything inside the using block will be
executed as if it was the impersonated user.

Example 1:

```C#
try
{
    using (Impersonator imp = new Impersonator("username", "password", "domain"))
    {
        Console.WriteLine("UserName: {0}", Environment.UserName);
    }
}
catch (Exception ex) when (ex is Win32ApiException || ex is ArgumentException)
{
    Console.WriteLine(ex);
}
```

Example 2:

```C#
try
{
    using (Impersonator imp = new Impersonator(new Win32ApiService()))
    {
        Console.WriteLine("UserName: {0}", Environment.UserName);
    }
}
catch (Exception ex) when (ex is Win32ApiException || ex is ArgumentException)
{
    Console.WriteLine(ex);
}
```

## Launching Applications

In certain niche situations, its desired to launch a GUI application via a
windows service. Launching a process using the standard Process class within
dotnet will not present the GUI of the launched application on the desktop of
the logged in user. To work around this, I was able to leverage the standard
Windows API to get a token using the current logged in user's session. This
token is then duplicated and the elevated to the same permission level as the
service. This token is then used to launch the process as the specified user
which will allow the application to be shown on the screen. It is important that
if this service is not running as LocalSystem, the account has the appropriate
user rights assignments to create and manage process level tokens.

Example:

```C#
try
{
    var win32ApiService = new Win32ApiService();
    win32ApiService.StartProcessAndBypassUAC("calc.exe", out PROCESS_INFORMATION procInfo),
}
catch (Win32ApiException ex)
{
    Console.WriteLine(ex);
}
```
