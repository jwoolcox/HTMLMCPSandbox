namespace HTMLMCPSandbox
{
    partial class frmWebBrowse
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWebBrowse));
            webBrowser = new CefSharp.WinForms.ChromiumWebBrowser();
            SuspendLayout();
            // 
            // webBrowser
            // 
            webBrowser.ActivateBrowserOnCreation = false;
            webBrowser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webBrowser.Location = new Point(10, 9);
            webBrowser.Name = "webBrowser";
            webBrowser.Size = new Size(778, 417);
            webBrowser.TabIndex = 1;
            // 
            // frmWebBrowse
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(797, 450);
            Controls.Add(webBrowser);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "frmWebBrowse";
            Text = "HTMLMCPSandbox";
            ResumeLayout(false);
        }

        #endregion

        private CefSharp.WinForms.ChromiumWebBrowser webBrowser;
    }
}
