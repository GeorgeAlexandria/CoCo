using System;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace CoCo
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading =true)]
    [Guid("b933474d-306e-434f-952d-a820c849ed07")]
    public sealed class VsPackage : Package
    {
    }
}