﻿
namespace Launcher
{
    partial class MainWindow
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
            this.actionGroup = new System.Windows.Forms.GroupBox();
            this.uploadLog = new System.Windows.Forms.Button();
            this.startNoUpdate = new System.Windows.Forms.Button();
            this.updateAndStart = new System.Windows.Forms.Button();
            this.outputLog = new System.Windows.Forms.RichTextBox();
            this.actionGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // actionGroup
            // 
            this.actionGroup.Controls.Add(this.uploadLog);
            this.actionGroup.Controls.Add(this.startNoUpdate);
            this.actionGroup.Controls.Add(this.updateAndStart);
            this.actionGroup.Controls.Add(this.outputLog);
            this.actionGroup.Location = new System.Drawing.Point(12, 11);
            this.actionGroup.Name = "actionGroup";
            this.actionGroup.Size = new System.Drawing.Size(690, 438);
            this.actionGroup.TabIndex = 0;
            this.actionGroup.TabStop = false;
            this.actionGroup.Text = "Actions";
            // 
            // uploadLog
            // 
            this.uploadLog.Location = new System.Drawing.Point(6, 384);
            this.uploadLog.Name = "uploadLog";
            this.uploadLog.Size = new System.Drawing.Size(335, 48);
            this.uploadLog.TabIndex = 2;
            this.uploadLog.Text = "Upload output_log.txt*\r\n\r\n* Game must be ran at least once!";
            this.uploadLog.UseVisualStyleBackColor = true;
            this.uploadLog.Click += new System.EventHandler(this.uploadLog_Click);
            // 
            // startNoUpdate
            // 
            this.startNoUpdate.Location = new System.Drawing.Point(347, 19);
            this.startNoUpdate.Name = "startNoUpdate";
            this.startNoUpdate.Size = new System.Drawing.Size(335, 32);
            this.startNoUpdate.TabIndex = 1;
            this.startNoUpdate.Text = "Just Play (No Downloading/Updating)";
            this.startNoUpdate.UseVisualStyleBackColor = true;
            this.startNoUpdate.Click += new System.EventHandler(this.startNoUpdate_Click);
            // 
            // updateAndStart
            // 
            this.updateAndStart.Location = new System.Drawing.Point(6, 19);
            this.updateAndStart.Name = "updateAndStart";
            this.updateAndStart.Size = new System.Drawing.Size(335, 32);
            this.updateAndStart.TabIndex = 0;
            this.updateAndStart.Text = "Download/Update and Play";
            this.updateAndStart.UseVisualStyleBackColor = true;
            this.updateAndStart.Click += new System.EventHandler(this.updateAndStart_Click);
            // 
            // outputLog
            // 
            this.outputLog.Location = new System.Drawing.Point(6, 57);
            this.outputLog.Name = "outputLog";
            this.outputLog.ReadOnly = true;
            this.outputLog.Size = new System.Drawing.Size(677, 321);
            this.outputLog.TabIndex = 3;
            this.outputLog.Text = "";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 461);
            this.Controls.Add(this.actionGroup);
            this.DoubleBuffered = true;
            this.Name = "MainWindow";
            this.Text = "Guardian Install Manager";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.actionGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox actionGroup;
        private System.Windows.Forms.RichTextBox outputLog;
        private System.Windows.Forms.Button startNoUpdate;
        private System.Windows.Forms.Button updateAndStart;
        private System.Windows.Forms.Button uploadLog;
    }
}

