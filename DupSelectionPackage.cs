using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace ChrisLajoie.DupSelection
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Constants.guidDupSelectionPkgString)]
    public sealed class DupSelectionPackage : Package
    {
        public DupSelectionPackage()
        {
        }

        protected override void Initialize()
        {
            DupSelectionCommand.Initialize(this);
            base.Initialize();
        }
    }
}
