namespace ultimate.mailer
{
    partial class FormMailing
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMailing));
            this.panelMailingMain = new System.Windows.Forms.Panel();
            this.buttonMaillingStart = new System.Windows.Forms.Button();
            this.panelMainSeparator4 = new System.Windows.Forms.Panel();
            this.buttonMaillingStop = new System.Windows.Forms.Button();
            this.pictureBoxMailingMainTitle = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanelTrackers = new System.Windows.Forms.TableLayoutPanel();
            this.circularProgressBarTrackersError = new CircularProgressBar.CircularProgressBar();
            this.labelTrackersErrorValue = new System.Windows.Forms.Label();
            this.labelTrackersSentValue = new System.Windows.Forms.Label();
            this.labelTrackersPerformedValue = new System.Windows.Forms.Label();
            this.labelMailingGeneralSent = new System.Windows.Forms.Label();
            this.circularProgressBarTrackersSent = new CircularProgressBar.CircularProgressBar();
            this.labelMailingGeneralPerformed = new System.Windows.Forms.Label();
            this.circularProgressBarTrackersPerformed = new CircularProgressBar.CircularProgressBar();
            this.labelMailingGeneralError = new System.Windows.Forms.Label();
            this.labelTrackers = new System.Windows.Forms.Label();
            this.panelTrackersTitle = new System.Windows.Forms.Panel();
            this.pictureBoxTrackersLoading = new System.Windows.Forms.PictureBox();
            this.comboBoxTrackersExport = new System.Windows.Forms.ComboBox();
            this.buttonMailingTrackersExport = new System.Windows.Forms.Button();
            this.labelLogs = new System.Windows.Forms.Label();
            this.panelLogsTitle = new System.Windows.Forms.Panel();
            this.panelMailingLogs = new System.Windows.Forms.Panel();
            this.richTextBoxLogs = new System.Windows.Forms.RichTextBox();
            this.panelMailingMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMailingMainTitle)).BeginInit();
            this.tableLayoutPanelTrackers.SuspendLayout();
            this.panelTrackersTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTrackersLoading)).BeginInit();
            this.panelLogsTitle.SuspendLayout();
            this.panelMailingLogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMailingMain
            // 
            this.panelMailingMain.BackColor = System.Drawing.Color.MidnightBlue;
            this.panelMailingMain.Controls.Add(this.buttonMaillingStart);
            this.panelMailingMain.Controls.Add(this.panelMainSeparator4);
            this.panelMailingMain.Controls.Add(this.buttonMaillingStop);
            this.panelMailingMain.Controls.Add(this.pictureBoxMailingMainTitle);
            this.panelMailingMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMailingMain.Location = new System.Drawing.Point(0, 0);
            this.panelMailingMain.Margin = new System.Windows.Forms.Padding(4);
            this.panelMailingMain.Name = "panelMailingMain";
            this.panelMailingMain.Padding = new System.Windows.Forms.Padding(9, 7, 9, 7);
            this.panelMailingMain.Size = new System.Drawing.Size(1045, 62);
            this.panelMailingMain.TabIndex = 0;
            // 
            // buttonMaillingStart
            // 
            this.buttonMaillingStart.BackColor = System.Drawing.Color.SeaGreen;
            this.buttonMaillingStart.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonMaillingStart.Enabled = false;
            this.buttonMaillingStart.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonMaillingStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMaillingStart.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMaillingStart.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonMaillingStart.Location = new System.Drawing.Point(828, 7);
            this.buttonMaillingStart.Margin = new System.Windows.Forms.Padding(4);
            this.buttonMaillingStart.Name = "buttonMaillingStart";
            this.buttonMaillingStart.Size = new System.Drawing.Size(133, 48);
            this.buttonMaillingStart.TabIndex = 0;
            this.buttonMaillingStart.Text = "Resume";
            this.buttonMaillingStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonMaillingStart.UseVisualStyleBackColor = false;
            this.buttonMaillingStart.Click += new System.EventHandler(this.ButtonMaillingStart_Click);
            // 
            // panelMainSeparator4
            // 
            this.panelMainSeparator4.BackColor = System.Drawing.Color.Transparent;
            this.panelMainSeparator4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelMainSeparator4.Location = new System.Drawing.Point(961, 7);
            this.panelMainSeparator4.Margin = new System.Windows.Forms.Padding(4);
            this.panelMainSeparator4.Name = "panelMainSeparator4";
            this.panelMainSeparator4.Size = new System.Drawing.Size(8, 48);
            this.panelMainSeparator4.TabIndex = 20;
            // 
            // buttonMaillingStop
            // 
            this.buttonMaillingStop.BackColor = System.Drawing.Color.Crimson;
            this.buttonMaillingStop.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonMaillingStop.Enabled = false;
            this.buttonMaillingStop.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonMaillingStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMaillingStop.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMaillingStop.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonMaillingStop.Location = new System.Drawing.Point(969, 7);
            this.buttonMaillingStop.Margin = new System.Windows.Forms.Padding(4);
            this.buttonMaillingStop.Name = "buttonMaillingStop";
            this.buttonMaillingStop.Size = new System.Drawing.Size(67, 48);
            this.buttonMaillingStop.TabIndex = 1;
            this.buttonMaillingStop.Text = "Stop";
            this.buttonMaillingStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonMaillingStop.UseVisualStyleBackColor = false;
            this.buttonMaillingStop.Click += new System.EventHandler(this.ButtonMaillingStop_Click);
            // 
            // pictureBoxMailingMainTitle
            // 
            this.pictureBoxMailingMainTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBoxMailingMainTitle.Image = global::ultimate.mailer.Properties.Resources.logo;
            this.pictureBoxMailingMainTitle.Location = new System.Drawing.Point(9, 7);
            this.pictureBoxMailingMainTitle.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxMailingMainTitle.Name = "pictureBoxMailingMainTitle";
            this.pictureBoxMailingMainTitle.Size = new System.Drawing.Size(320, 48);
            this.pictureBoxMailingMainTitle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMailingMainTitle.TabIndex = 17;
            this.pictureBoxMailingMainTitle.TabStop = false;
            // 
            // tableLayoutPanelTrackers
            // 
            this.tableLayoutPanelTrackers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelTrackers.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tableLayoutPanelTrackers.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanelTrackers.ColumnCount = 3;
            this.tableLayoutPanelTrackers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelTrackers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelTrackers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelTrackers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanelTrackers.Controls.Add(this.circularProgressBarTrackersError, 2, 1);
            this.tableLayoutPanelTrackers.Controls.Add(this.labelTrackersErrorValue, 0, 2);
            this.tableLayoutPanelTrackers.Controls.Add(this.labelTrackersSentValue, 0, 2);
            this.tableLayoutPanelTrackers.Controls.Add(this.labelTrackersPerformedValue, 0, 2);
            this.tableLayoutPanelTrackers.Controls.Add(this.labelMailingGeneralSent, 1, 0);
            this.tableLayoutPanelTrackers.Controls.Add(this.circularProgressBarTrackersSent, 1, 1);
            this.tableLayoutPanelTrackers.Controls.Add(this.labelMailingGeneralPerformed, 0, 0);
            this.tableLayoutPanelTrackers.Controls.Add(this.circularProgressBarTrackersPerformed, 0, 1);
            this.tableLayoutPanelTrackers.Controls.Add(this.labelMailingGeneralError, 2, 0);
            this.tableLayoutPanelTrackers.Location = new System.Drawing.Point(21, 111);
            this.tableLayoutPanelTrackers.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanelTrackers.Name = "tableLayoutPanelTrackers";
            this.tableLayoutPanelTrackers.RowCount = 3;
            this.tableLayoutPanelTrackers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.63534F));
            this.tableLayoutPanelTrackers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60.30856F));
            this.tableLayoutPanelTrackers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.0561F));
            this.tableLayoutPanelTrackers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanelTrackers.Size = new System.Drawing.Size(1004, 202);
            this.tableLayoutPanelTrackers.TabIndex = 2;
            // 
            // circularProgressBarTrackersError
            // 
            this.circularProgressBarTrackersError.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.circularProgressBarTrackersError.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.circularProgressBarTrackersError.AnimationSpeed = 500;
            this.circularProgressBarTrackersError.BackColor = System.Drawing.Color.Transparent;
            this.circularProgressBarTrackersError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.circularProgressBarTrackersError.ForeColor = System.Drawing.SystemColors.ControlText;
            this.circularProgressBarTrackersError.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.circularProgressBarTrackersError.InnerMargin = -1;
            this.circularProgressBarTrackersError.InnerWidth = -1;
            this.circularProgressBarTrackersError.Location = new System.Drawing.Point(789, 57);
            this.circularProgressBarTrackersError.Margin = new System.Windows.Forms.Padding(4);
            this.circularProgressBarTrackersError.MarqueeAnimationSpeed = 2000;
            this.circularProgressBarTrackersError.Name = "circularProgressBarTrackersError";
            this.circularProgressBarTrackersError.OuterColor = System.Drawing.Color.Gray;
            this.circularProgressBarTrackersError.OuterMargin = -25;
            this.circularProgressBarTrackersError.OuterWidth = 25;
            this.circularProgressBarTrackersError.ProgressColor = System.Drawing.Color.Red;
            this.circularProgressBarTrackersError.ProgressWidth = 5;
            this.circularProgressBarTrackersError.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.circularProgressBarTrackersError.Size = new System.Drawing.Size(93, 86);
            this.circularProgressBarTrackersError.StartAngle = -90;
            this.circularProgressBarTrackersError.Step = 0;
            this.circularProgressBarTrackersError.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.circularProgressBarTrackersError.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
            this.circularProgressBarTrackersError.SubscriptText = "";
            this.circularProgressBarTrackersError.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.circularProgressBarTrackersError.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
            this.circularProgressBarTrackersError.SuperscriptText = "";
            this.circularProgressBarTrackersError.TabIndex = 10;
            this.circularProgressBarTrackersError.Text = "0 %";
            this.circularProgressBarTrackersError.TextMargin = new System.Windows.Forms.Padding(2, 2, 0, 0);
            this.circularProgressBarTrackersError.Value = 68;
            // 
            // labelTrackersErrorValue
            // 
            this.labelTrackersErrorValue.AutoSize = true;
            this.labelTrackersErrorValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTrackersErrorValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.labelTrackersErrorValue.Location = new System.Drawing.Point(674, 160);
            this.labelTrackersErrorValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTrackersErrorValue.Name = "labelTrackersErrorValue";
            this.labelTrackersErrorValue.Size = new System.Drawing.Size(324, 40);
            this.labelTrackersErrorValue.TabIndex = 2;
            this.labelTrackersErrorValue.Text = "0 / 0";
            this.labelTrackersErrorValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTrackersSentValue
            // 
            this.labelTrackersSentValue.AutoSize = true;
            this.labelTrackersSentValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTrackersSentValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTrackersSentValue.Location = new System.Drawing.Point(340, 160);
            this.labelTrackersSentValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTrackersSentValue.Name = "labelTrackersSentValue";
            this.labelTrackersSentValue.Size = new System.Drawing.Size(324, 40);
            this.labelTrackersSentValue.TabIndex = 11;
            this.labelTrackersSentValue.Text = "0 / 0";
            this.labelTrackersSentValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTrackersPerformedValue
            // 
            this.labelTrackersPerformedValue.AutoSize = true;
            this.labelTrackersPerformedValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTrackersPerformedValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTrackersPerformedValue.Location = new System.Drawing.Point(6, 160);
            this.labelTrackersPerformedValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTrackersPerformedValue.Name = "labelTrackersPerformedValue";
            this.labelTrackersPerformedValue.Size = new System.Drawing.Size(324, 40);
            this.labelTrackersPerformedValue.TabIndex = 5;
            this.labelTrackersPerformedValue.Text = "0 / 0";
            this.labelTrackersPerformedValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelMailingGeneralSent
            // 
            this.labelMailingGeneralSent.AutoSize = true;
            this.labelMailingGeneralSent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelMailingGeneralSent.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMailingGeneralSent.Location = new System.Drawing.Point(340, 2);
            this.labelMailingGeneralSent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMailingGeneralSent.Name = "labelMailingGeneralSent";
            this.labelMailingGeneralSent.Size = new System.Drawing.Size(324, 38);
            this.labelMailingGeneralSent.TabIndex = 3;
            this.labelMailingGeneralSent.Text = "Sent";
            this.labelMailingGeneralSent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // circularProgressBarTrackersSent
            // 
            this.circularProgressBarTrackersSent.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.circularProgressBarTrackersSent.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.circularProgressBarTrackersSent.AnimationSpeed = 500;
            this.circularProgressBarTrackersSent.BackColor = System.Drawing.Color.Transparent;
            this.circularProgressBarTrackersSent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.circularProgressBarTrackersSent.ForeColor = System.Drawing.SystemColors.ControlText;
            this.circularProgressBarTrackersSent.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.circularProgressBarTrackersSent.InnerMargin = -1;
            this.circularProgressBarTrackersSent.InnerWidth = -1;
            this.circularProgressBarTrackersSent.Location = new System.Drawing.Point(455, 57);
            this.circularProgressBarTrackersSent.Margin = new System.Windows.Forms.Padding(4);
            this.circularProgressBarTrackersSent.MarqueeAnimationSpeed = 2000;
            this.circularProgressBarTrackersSent.Name = "circularProgressBarTrackersSent";
            this.circularProgressBarTrackersSent.OuterColor = System.Drawing.Color.Gray;
            this.circularProgressBarTrackersSent.OuterMargin = -25;
            this.circularProgressBarTrackersSent.OuterWidth = 25;
            this.circularProgressBarTrackersSent.ProgressColor = System.Drawing.Color.LightGreen;
            this.circularProgressBarTrackersSent.ProgressWidth = 5;
            this.circularProgressBarTrackersSent.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.circularProgressBarTrackersSent.Size = new System.Drawing.Size(93, 86);
            this.circularProgressBarTrackersSent.StartAngle = -90;
            this.circularProgressBarTrackersSent.Step = 0;
            this.circularProgressBarTrackersSent.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.circularProgressBarTrackersSent.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
            this.circularProgressBarTrackersSent.SubscriptText = "";
            this.circularProgressBarTrackersSent.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.circularProgressBarTrackersSent.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
            this.circularProgressBarTrackersSent.SuperscriptText = "";
            this.circularProgressBarTrackersSent.TabIndex = 4;
            this.circularProgressBarTrackersSent.Text = "0 %";
            this.circularProgressBarTrackersSent.TextMargin = new System.Windows.Forms.Padding(2, 2, 0, 0);
            this.circularProgressBarTrackersSent.Value = 68;
            // 
            // labelMailingGeneralPerformed
            // 
            this.labelMailingGeneralPerformed.AutoSize = true;
            this.labelMailingGeneralPerformed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelMailingGeneralPerformed.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMailingGeneralPerformed.Location = new System.Drawing.Point(6, 2);
            this.labelMailingGeneralPerformed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMailingGeneralPerformed.Name = "labelMailingGeneralPerformed";
            this.labelMailingGeneralPerformed.Size = new System.Drawing.Size(324, 38);
            this.labelMailingGeneralPerformed.TabIndex = 0;
            this.labelMailingGeneralPerformed.Text = "Performed";
            this.labelMailingGeneralPerformed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // circularProgressBarTrackersPerformed
            // 
            this.circularProgressBarTrackersPerformed.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.circularProgressBarTrackersPerformed.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.circularProgressBarTrackersPerformed.AnimationSpeed = 500;
            this.circularProgressBarTrackersPerformed.BackColor = System.Drawing.Color.Transparent;
            this.circularProgressBarTrackersPerformed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.circularProgressBarTrackersPerformed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.circularProgressBarTrackersPerformed.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.circularProgressBarTrackersPerformed.InnerMargin = -1;
            this.circularProgressBarTrackersPerformed.InnerWidth = -1;
            this.circularProgressBarTrackersPerformed.Location = new System.Drawing.Point(121, 57);
            this.circularProgressBarTrackersPerformed.Margin = new System.Windows.Forms.Padding(4);
            this.circularProgressBarTrackersPerformed.MarqueeAnimationSpeed = 2000;
            this.circularProgressBarTrackersPerformed.Name = "circularProgressBarTrackersPerformed";
            this.circularProgressBarTrackersPerformed.OuterColor = System.Drawing.Color.Gray;
            this.circularProgressBarTrackersPerformed.OuterMargin = -25;
            this.circularProgressBarTrackersPerformed.OuterWidth = 25;
            this.circularProgressBarTrackersPerformed.ProgressColor = System.Drawing.Color.LightSkyBlue;
            this.circularProgressBarTrackersPerformed.ProgressWidth = 5;
            this.circularProgressBarTrackersPerformed.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.circularProgressBarTrackersPerformed.Size = new System.Drawing.Size(93, 86);
            this.circularProgressBarTrackersPerformed.StartAngle = -90;
            this.circularProgressBarTrackersPerformed.Step = 0;
            this.circularProgressBarTrackersPerformed.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.circularProgressBarTrackersPerformed.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
            this.circularProgressBarTrackersPerformed.SubscriptText = "";
            this.circularProgressBarTrackersPerformed.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.circularProgressBarTrackersPerformed.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
            this.circularProgressBarTrackersPerformed.SuperscriptText = "";
            this.circularProgressBarTrackersPerformed.TabIndex = 1;
            this.circularProgressBarTrackersPerformed.Text = "0 %";
            this.circularProgressBarTrackersPerformed.TextMargin = new System.Windows.Forms.Padding(2, 2, 0, 0);
            this.circularProgressBarTrackersPerformed.Value = 68;
            // 
            // labelMailingGeneralError
            // 
            this.labelMailingGeneralError.AutoSize = true;
            this.labelMailingGeneralError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelMailingGeneralError.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMailingGeneralError.Location = new System.Drawing.Point(674, 2);
            this.labelMailingGeneralError.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMailingGeneralError.Name = "labelMailingGeneralError";
            this.labelMailingGeneralError.Size = new System.Drawing.Size(324, 38);
            this.labelMailingGeneralError.TabIndex = 9;
            this.labelMailingGeneralError.Text = "Error";
            this.labelMailingGeneralError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTrackers
            // 
            this.labelTrackers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTrackers.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTrackers.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelTrackers.Location = new System.Drawing.Point(0, 0);
            this.labelTrackers.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTrackers.Name = "labelTrackers";
            this.labelTrackers.Padding = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.labelTrackers.Size = new System.Drawing.Size(684, 30);
            this.labelTrackers.TabIndex = 0;
            this.labelTrackers.Text = "» Trackers";
            this.labelTrackers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelTrackersTitle
            // 
            this.panelTrackersTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTrackersTitle.BackColor = System.Drawing.Color.CornflowerBlue;
            this.panelTrackersTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTrackersTitle.Controls.Add(this.labelTrackers);
            this.panelTrackersTitle.Controls.Add(this.pictureBoxTrackersLoading);
            this.panelTrackersTitle.Controls.Add(this.comboBoxTrackersExport);
            this.panelTrackersTitle.Controls.Add(this.buttonMailingTrackersExport);
            this.panelTrackersTitle.Location = new System.Drawing.Point(21, 80);
            this.panelTrackersTitle.Margin = new System.Windows.Forms.Padding(4);
            this.panelTrackersTitle.Name = "panelTrackersTitle";
            this.panelTrackersTitle.Size = new System.Drawing.Size(1002, 32);
            this.panelTrackersTitle.TabIndex = 1;
            // 
            // pictureBoxTrackersLoading
            // 
            this.pictureBoxTrackersLoading.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureBoxTrackersLoading.Image = global::ultimate.mailer.Properties.Resources.load;
            this.pictureBoxTrackersLoading.Location = new System.Drawing.Point(684, 0);
            this.pictureBoxTrackersLoading.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxTrackersLoading.Name = "pictureBoxTrackersLoading";
            this.pictureBoxTrackersLoading.Size = new System.Drawing.Size(80, 30);
            this.pictureBoxTrackersLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxTrackersLoading.TabIndex = 48;
            this.pictureBoxTrackersLoading.TabStop = false;
            this.pictureBoxTrackersLoading.Visible = false;
            // 
            // comboBoxTrackersExport
            // 
            this.comboBoxTrackersExport.Dock = System.Windows.Forms.DockStyle.Right;
            this.comboBoxTrackersExport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTrackersExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxTrackersExport.FormattingEnabled = true;
            this.comboBoxTrackersExport.ItemHeight = 20;
            this.comboBoxTrackersExport.Items.AddRange(new object[] {
            "All",
            "Remaining",
            "Performed",
            "Sent",
            "Error",
            "Remaining (Emails)",
            "Performed (Emails)",
            "Sent (Emails)",
            "Error (Emails)"});
            this.comboBoxTrackersExport.Location = new System.Drawing.Point(764, 0);
            this.comboBoxTrackersExport.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxTrackersExport.Name = "comboBoxTrackersExport";
            this.comboBoxTrackersExport.Size = new System.Drawing.Size(160, 28);
            this.comboBoxTrackersExport.TabIndex = 49;
            // 
            // buttonMailingTrackersExport
            // 
            this.buttonMailingTrackersExport.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.buttonMailingTrackersExport.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonMailingTrackersExport.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonMailingTrackersExport.FlatAppearance.BorderSize = 0;
            this.buttonMailingTrackersExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMailingTrackersExport.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMailingTrackersExport.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonMailingTrackersExport.Location = new System.Drawing.Point(924, 0);
            this.buttonMailingTrackersExport.Margin = new System.Windows.Forms.Padding(4);
            this.buttonMailingTrackersExport.Name = "buttonMailingTrackersExport";
            this.buttonMailingTrackersExport.Size = new System.Drawing.Size(76, 30);
            this.buttonMailingTrackersExport.TabIndex = 1;
            this.buttonMailingTrackersExport.Text = "Export";
            this.buttonMailingTrackersExport.UseVisualStyleBackColor = false;
            this.buttonMailingTrackersExport.Click += new System.EventHandler(this.ButtonMailingTrackersExport_Click);
            // 
            // labelLogs
            // 
            this.labelLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLogs.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLogs.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelLogs.Location = new System.Drawing.Point(0, 0);
            this.labelLogs.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelLogs.Name = "labelLogs";
            this.labelLogs.Padding = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.labelLogs.Size = new System.Drawing.Size(1000, 30);
            this.labelLogs.TabIndex = 0;
            this.labelLogs.Text = "» Logs";
            this.labelLogs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelLogsTitle
            // 
            this.panelLogsTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLogsTitle.BackColor = System.Drawing.Color.CornflowerBlue;
            this.panelLogsTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLogsTitle.Controls.Add(this.labelLogs);
            this.panelLogsTitle.Location = new System.Drawing.Point(21, 320);
            this.panelLogsTitle.Margin = new System.Windows.Forms.Padding(4);
            this.panelLogsTitle.Name = "panelLogsTitle";
            this.panelLogsTitle.Size = new System.Drawing.Size(1002, 32);
            this.panelLogsTitle.TabIndex = 3;
            // 
            // panelMailingLogs
            // 
            this.panelMailingLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMailingLogs.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelMailingLogs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMailingLogs.Controls.Add(this.richTextBoxLogs);
            this.panelMailingLogs.Location = new System.Drawing.Point(21, 351);
            this.panelMailingLogs.Margin = new System.Windows.Forms.Padding(4);
            this.panelMailingLogs.Name = "panelMailingLogs";
            this.panelMailingLogs.Size = new System.Drawing.Size(1002, 321);
            this.panelMailingLogs.TabIndex = 4;
            // 
            // richTextBoxLogs
            // 
            this.richTextBoxLogs.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.richTextBoxLogs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxLogs.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxLogs.HideSelection = false;
            this.richTextBoxLogs.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxLogs.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBoxLogs.Name = "richTextBoxLogs";
            this.richTextBoxLogs.ReadOnly = true;
            this.richTextBoxLogs.Size = new System.Drawing.Size(1000, 319);
            this.richTextBoxLogs.TabIndex = 0;
            this.richTextBoxLogs.Text = "";
            this.richTextBoxLogs.TextChanged += new System.EventHandler(this.RichTextBoxLogs_TextChanged);
            // 
            // FormMailing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 690);
            this.Controls.Add(this.panelMailingLogs);
            this.Controls.Add(this.panelLogsTitle);
            this.Controls.Add(this.panelTrackersTitle);
            this.Controls.Add(this.tableLayoutPanelTrackers);
            this.Controls.Add(this.panelMailingMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(594, 542);
            this.Name = "FormMailing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ultimate Mailer V3";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMailing_FormClosing);
            this.Load += new System.EventHandler(this.FormMailing_Load);
            this.Shown += new System.EventHandler(this.FormMailing_Shown);
            this.panelMailingMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMailingMainTitle)).EndInit();
            this.tableLayoutPanelTrackers.ResumeLayout(false);
            this.tableLayoutPanelTrackers.PerformLayout();
            this.panelTrackersTitle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTrackersLoading)).EndInit();
            this.panelLogsTitle.ResumeLayout(false);
            this.panelMailingLogs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelMailingMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTrackers;
        private CircularProgressBar.CircularProgressBar circularProgressBarTrackersError;
        private System.Windows.Forms.Label labelTrackersErrorValue;
        private System.Windows.Forms.Label labelTrackersSentValue;
        private System.Windows.Forms.Label labelTrackersPerformedValue;
        private System.Windows.Forms.Label labelMailingGeneralSent;
        private CircularProgressBar.CircularProgressBar circularProgressBarTrackersSent;
        private System.Windows.Forms.Label labelMailingGeneralPerformed;
        private CircularProgressBar.CircularProgressBar circularProgressBarTrackersPerformed;
        private System.Windows.Forms.Label labelMailingGeneralError;
        private System.Windows.Forms.Label labelTrackers;
        private System.Windows.Forms.Panel panelTrackersTitle;
        private System.Windows.Forms.PictureBox pictureBoxMailingMainTitle;
        private System.Windows.Forms.Label labelLogs;
        private System.Windows.Forms.Panel panelLogsTitle;
        private System.Windows.Forms.Button buttonMailingTrackersExport;
        private System.Windows.Forms.Panel panelMailingLogs;
        private System.Windows.Forms.RichTextBox richTextBoxLogs;
        private System.Windows.Forms.PictureBox pictureBoxTrackersLoading;
        private System.Windows.Forms.ComboBox comboBoxTrackersExport;
        private System.Windows.Forms.Button buttonMaillingStart;
        private System.Windows.Forms.Panel panelMainSeparator4;
        private System.Windows.Forms.Button buttonMaillingStop;
    }
}