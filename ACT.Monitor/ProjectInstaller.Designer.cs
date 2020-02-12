namespace ACT.Monitor
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ACTMonitorServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.ACTMonitorInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // ACTMonitorServiceProcessInstaller
            // 
            this.ACTMonitorServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.ACTMonitorServiceProcessInstaller.Password = null;
            this.ACTMonitorServiceProcessInstaller.Username = null;
            this.ACTMonitorServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.ACTMonitorServiceProcessInstaller_AfterInstall);
            // 
            // ACTMonitorInstaller
            // 
            this.ACTMonitorInstaller.Description = "The ACT Monitor Service";
            this.ACTMonitorInstaller.DisplayName = "ACT Monitor";
            this.ACTMonitorInstaller.ServiceName = "ACT Monitor";
            this.ACTMonitorInstaller.StartType = System.ServiceProcess.ServiceStartMode.Manual;
            this.ACTMonitorInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.ACTMonitorInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ACTMonitorServiceProcessInstaller,
            this.ACTMonitorInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ACTMonitorServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller ACTMonitorInstaller;
    }
}