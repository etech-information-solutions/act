using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace ACT.Monitor
{
    [RunInstaller( true )]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void ACTMonitorInstaller_AfterInstall( object sender, InstallEventArgs e )
        {

        }

        private void ACTMonitorServiceProcessInstaller_AfterInstall( object sender, InstallEventArgs e )
        {

        }
    }
}
