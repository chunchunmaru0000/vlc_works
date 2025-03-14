﻿using System.IO;
using System;

namespace vlc_works
{
	partial class FaceForm
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
            this.userDataListView = new System.Windows.Forms.ListView();
            this.connectBut = new System.Windows.Forms.Button();
            this.disconnectBut = new System.Windows.Forms.Button();
            this.clearList = new System.Windows.Forms.Button();
            this.textPort = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ipPortBox = new System.Windows.Forms.TextBox();
            this.passwordTextLabel = new System.Windows.Forms.Label();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.ipAddressTextLabel = new System.Windows.Forms.Label();
            this.ipAdressBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.camBox = new System.Windows.Forms.ComboBox();
            this.webCamTextLabel = new System.Windows.Forms.Label();
            this.camPictureBox = new System.Windows.Forms.PictureBox();
            this.saveCamBut = new System.Windows.Forms.Button();
            this.takenPhotoPictureBox = new System.Windows.Forms.PictureBox();
            this.takePhotoBut = new System.Windows.Forms.Button();
            this.aiPictureBox = new System.Windows.Forms.PictureBox();
            this.idLabel = new System.Windows.Forms.Label();
            this.testWriteButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.openEditDbFormBut = new System.Windows.Forms.Button();
            this.newIdBut = new System.Windows.Forms.Button();
            this.photoSelectedLabel = new System.Windows.Forms.Label();
            this.lastIdLabel = new System.Windows.Forms.Label();
            this.photoTextLabel = new System.Windows.Forms.Label();
            this.machineLabel = new System.Windows.Forms.Label();
            this.idBox = new System.Windows.Forms.TextBox();
            this.machineIdBox = new System.Windows.Forms.TextBox();
            this.idLabeltext = new System.Windows.Forms.Label();
            this.saveAiBut = new System.Windows.Forms.Button();
            this.photoToSetTextLabel = new System.Windows.Forms.Label();
            this.recognizedPersonTextLabel = new System.Windows.Forms.Label();
            this.upCamBut = new System.Windows.Forms.Button();
            this.downCamBut = new System.Windows.Forms.Button();
            this.faceControlProgramTextLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.camPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.takenPhotoPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aiPictureBox)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // userDataListView
            // 
            this.userDataListView.HideSelection = false;
            this.userDataListView.Location = new System.Drawing.Point(0, 736);
            this.userDataListView.Name = "userDataListView";
            this.userDataListView.Size = new System.Drawing.Size(448, 224);
            this.userDataListView.TabIndex = 0;
            this.userDataListView.UseCompatibleStateImageBehavior = false;
            this.userDataListView.View = System.Windows.Forms.View.Details;
            // 
            // connectBut
            // 
            this.connectBut.BackColor = System.Drawing.Color.LightCoral;
            this.connectBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connectBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.connectBut.Location = new System.Drawing.Point(0, 128);
            this.connectBut.Name = "connectBut";
            this.connectBut.Size = new System.Drawing.Size(352, 32);
            this.connectBut.TabIndex = 1;
            this.connectBut.Text = "Подключиться";
            this.connectBut.UseVisualStyleBackColor = false;
            this.connectBut.Click += new System.EventHandler(this.Connect_Click);
            // 
            // disconnectBut
            // 
            this.disconnectBut.BackColor = System.Drawing.Color.LightCoral;
            this.disconnectBut.Enabled = false;
            this.disconnectBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disconnectBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.disconnectBut.Location = new System.Drawing.Point(0, 160);
            this.disconnectBut.Name = "disconnectBut";
            this.disconnectBut.Size = new System.Drawing.Size(352, 32);
            this.disconnectBut.TabIndex = 2;
            this.disconnectBut.Text = "Отключиться";
            this.disconnectBut.UseVisualStyleBackColor = false;
            this.disconnectBut.Click += new System.EventHandler(this.Disconnect_Click);
            // 
            // clearList
            // 
            this.clearList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearList.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.clearList.Location = new System.Drawing.Point(0, 192);
            this.clearList.Name = "clearList";
            this.clearList.Size = new System.Drawing.Size(352, 32);
            this.clearList.TabIndex = 3;
            this.clearList.Text = "Очистить список";
            this.clearList.UseVisualStyleBackColor = true;
            this.clearList.Click += new System.EventHandler(this.clearList_Click);
            // 
            // textPort
            // 
            this.textPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textPort.Location = new System.Drawing.Point(128, 32);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(224, 31);
            this.textPort.TabIndex = 4;
            this.textPort.Text = "7005";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ipPortBox);
            this.panel1.Controls.Add(this.passwordTextLabel);
            this.panel1.Controls.Add(this.passwordBox);
            this.panel1.Controls.Add(this.ipAddressTextLabel);
            this.panel1.Controls.Add(this.ipAdressBox);
            this.panel1.Controls.Add(this.portLabel);
            this.panel1.Controls.Add(this.disconnectBut);
            this.panel1.Controls.Add(this.connectBut);
            this.panel1.Controls.Add(this.clearList);
            this.panel1.Controls.Add(this.camBox);
            this.panel1.Controls.Add(this.webCamTextLabel);
            this.panel1.Controls.Add(this.textPort);
            this.panel1.Location = new System.Drawing.Point(448, 736);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(352, 224);
            this.panel1.TabIndex = 6;
            // 
            // ipPortBox
            // 
            this.ipPortBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ipPortBox.Location = new System.Drawing.Point(288, 64);
            this.ipPortBox.Name = "ipPortBox";
            this.ipPortBox.Size = new System.Drawing.Size(64, 31);
            this.ipPortBox.TabIndex = 20;
            this.ipPortBox.Text = "5005";
            // 
            // passwordTextLabel
            // 
            this.passwordTextLabel.AutoSize = true;
            this.passwordTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.passwordTextLabel.Location = new System.Drawing.Point(0, 96);
            this.passwordTextLabel.Name = "passwordTextLabel";
            this.passwordTextLabel.Size = new System.Drawing.Size(108, 31);
            this.passwordTextLabel.TabIndex = 19;
            this.passwordTextLabel.Text = "Пароль";
            // 
            // passwordBox
            // 
            this.passwordBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.passwordBox.Location = new System.Drawing.Point(128, 96);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.Size = new System.Drawing.Size(224, 31);
            this.passwordBox.TabIndex = 18;
            this.passwordBox.Text = "0";
            // 
            // ipAddressTextLabel
            // 
            this.ipAddressTextLabel.AutoSize = true;
            this.ipAddressTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ipAddressTextLabel.Location = new System.Drawing.Point(0, 64);
            this.ipAddressTextLabel.Name = "ipAddressTextLabel";
            this.ipAddressTextLabel.Size = new System.Drawing.Size(40, 31);
            this.ipAddressTextLabel.TabIndex = 17;
            this.ipAddressTextLabel.Text = "IP";
            // 
            // ipAdressBox
            // 
            this.ipAdressBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ipAdressBox.Location = new System.Drawing.Point(128, 64);
            this.ipAdressBox.Name = "ipAdressBox";
            this.ipAdressBox.Size = new System.Drawing.Size(160, 31);
            this.ipAdressBox.TabIndex = 16;
            this.ipAdressBox.Text = "192.168.1.224";
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.portLabel.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.portLabel.Location = new System.Drawing.Point(0, 32);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(76, 31);
            this.portLabel.TabIndex = 5;
            this.portLabel.Text = "Порт";
            // 
            // camBox
            // 
            this.camBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.camBox.FormattingEnabled = true;
            this.camBox.Location = new System.Drawing.Point(127, -1);
            this.camBox.Name = "camBox";
            this.camBox.Size = new System.Drawing.Size(224, 33);
            this.camBox.TabIndex = 9;
            this.camBox.DropDown += new System.EventHandler(this.camBox_DropDown);
            this.camBox.SelectedIndexChanged += new System.EventHandler(this.camBox_SelectedIndexChanged);
            // 
            // webCamTextLabel
            // 
            this.webCamTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.webCamTextLabel.Location = new System.Drawing.Point(-1, -1);
            this.webCamTextLabel.Name = "webCamTextLabel";
            this.webCamTextLabel.Size = new System.Drawing.Size(128, 32);
            this.webCamTextLabel.TabIndex = 10;
            this.webCamTextLabel.Text = "Камера";
            this.webCamTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // camPictureBox
            // 
            this.camPictureBox.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.camPictureBox.Location = new System.Drawing.Point(0, 32);
            this.camPictureBox.Name = "camPictureBox";
            this.camPictureBox.Size = new System.Drawing.Size(384, 576);
            this.camPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.camPictureBox.TabIndex = 8;
            this.camPictureBox.TabStop = false;
            // 
            // saveCamBut
            // 
            this.saveCamBut.BackColor = System.Drawing.Color.PaleTurquoise;
            this.saveCamBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveCamBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveCamBut.Location = new System.Drawing.Point(512, 640);
            this.saveCamBut.Name = "saveCamBut";
            this.saveCamBut.Size = new System.Drawing.Size(192, 64);
            this.saveCamBut.TabIndex = 11;
            this.saveCamBut.Text = "Сохранить изображение";
            this.saveCamBut.UseVisualStyleBackColor = false;
            this.saveCamBut.Click += new System.EventHandler(this.saveCamBut_Click);
            // 
            // takenPhotoPictureBox
            // 
            this.takenPhotoPictureBox.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.takenPhotoPictureBox.Location = new System.Drawing.Point(416, 32);
            this.takenPhotoPictureBox.Name = "takenPhotoPictureBox";
            this.takenPhotoPictureBox.Size = new System.Drawing.Size(384, 576);
            this.takenPhotoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.takenPhotoPictureBox.TabIndex = 12;
            this.takenPhotoPictureBox.TabStop = false;
            // 
            // takePhotoBut
            // 
            this.takePhotoBut.BackColor = System.Drawing.Color.SpringGreen;
            this.takePhotoBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.takePhotoBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.takePhotoBut.Location = new System.Drawing.Point(96, 608);
            this.takePhotoBut.Name = "takePhotoBut";
            this.takePhotoBut.Size = new System.Drawing.Size(192, 64);
            this.takePhotoBut.TabIndex = 13;
            this.takePhotoBut.Text = "Сделать фото";
            this.takePhotoBut.UseVisualStyleBackColor = false;
            this.takePhotoBut.Click += new System.EventHandler(this.takePhotoBut_Click);
            // 
            // aiPictureBox
            // 
            this.aiPictureBox.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.aiPictureBox.Location = new System.Drawing.Point(832, 32);
            this.aiPictureBox.Name = "aiPictureBox";
            this.aiPictureBox.Size = new System.Drawing.Size(384, 576);
            this.aiPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.aiPictureBox.TabIndex = 14;
            this.aiPictureBox.TabStop = false;
            // 
            // idLabel
            // 
            this.idLabel.AutoSize = true;
            this.idLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.idLabel.Location = new System.Drawing.Point(0, 0);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(250, 31);
            this.idLabel.TabIndex = 15;
            this.idLabel.Text = "ПОСЛЕДНИЙ ID:  ";
            // 
            // testWriteButton
            // 
            this.testWriteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.testWriteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.testWriteButton.Location = new System.Drawing.Point(0, 96);
            this.testWriteButton.Name = "testWriteButton";
            this.testWriteButton.Size = new System.Drawing.Size(319, 65);
            this.testWriteButton.TabIndex = 17;
            this.testWriteButton.Text = "Записать игрока";
            this.testWriteButton.UseVisualStyleBackColor = true;
            this.testWriteButton.Click += new System.EventHandler(this.WriteButton_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.openEditDbFormBut);
            this.panel2.Controls.Add(this.newIdBut);
            this.panel2.Controls.Add(this.photoSelectedLabel);
            this.panel2.Controls.Add(this.lastIdLabel);
            this.panel2.Controls.Add(this.photoTextLabel);
            this.panel2.Controls.Add(this.testWriteButton);
            this.panel2.Controls.Add(this.machineLabel);
            this.panel2.Controls.Add(this.idBox);
            this.panel2.Controls.Add(this.machineIdBox);
            this.panel2.Controls.Add(this.idLabel);
            this.panel2.Controls.Add(this.idLabeltext);
            this.panel2.Location = new System.Drawing.Point(800, 736);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(320, 224);
            this.panel2.TabIndex = 18;
            // 
            // openEditDbFormBut
            // 
            this.openEditDbFormBut.BackColor = System.Drawing.Color.Khaki;
            this.openEditDbFormBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openEditDbFormBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.openEditDbFormBut.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.openEditDbFormBut.Location = new System.Drawing.Point(0, 192);
            this.openEditDbFormBut.Name = "openEditDbFormBut";
            this.openEditDbFormBut.Size = new System.Drawing.Size(320, 32);
            this.openEditDbFormBut.TabIndex = 26;
            this.openEditDbFormBut.Text = "Редактирование";
            this.openEditDbFormBut.UseVisualStyleBackColor = false;
            this.openEditDbFormBut.Click += new System.EventHandler(this.openEditDbFormBut_Click);
            // 
            // newIdBut
            // 
            this.newIdBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.newIdBut.Location = new System.Drawing.Point(64, 64);
            this.newIdBut.Name = "newIdBut";
            this.newIdBut.Size = new System.Drawing.Size(96, 32);
            this.newIdBut.TabIndex = 25;
            this.newIdBut.Text = "новый";
            this.newIdBut.UseVisualStyleBackColor = true;
            this.newIdBut.Click += new System.EventHandler(this.newIdBut_Click);
            // 
            // photoSelectedLabel
            // 
            this.photoSelectedLabel.AutoSize = true;
            this.photoSelectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.photoSelectedLabel.Location = new System.Drawing.Point(160, 32);
            this.photoSelectedLabel.Name = "photoSelectedLabel";
            this.photoSelectedLabel.Size = new System.Drawing.Size(69, 31);
            this.photoSelectedLabel.TabIndex = 24;
            this.photoSelectedLabel.Text = "НЕТ";
            // 
            // lastIdLabel
            // 
            this.lastIdLabel.AutoSize = true;
            this.lastIdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lastIdLabel.Location = new System.Drawing.Point(223, -1);
            this.lastIdLabel.Name = "lastIdLabel";
            this.lastIdLabel.Size = new System.Drawing.Size(29, 31);
            this.lastIdLabel.TabIndex = 23;
            this.lastIdLabel.Text = "_";
            // 
            // photoTextLabel
            // 
            this.photoTextLabel.AutoSize = true;
            this.photoTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.photoTextLabel.Location = new System.Drawing.Point(0, 32);
            this.photoTextLabel.Name = "photoTextLabel";
            this.photoTextLabel.Size = new System.Drawing.Size(79, 31);
            this.photoTextLabel.TabIndex = 23;
            this.photoTextLabel.Text = "Фото";
            // 
            // machineLabel
            // 
            this.machineLabel.AutoSize = true;
            this.machineLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.machineLabel.Location = new System.Drawing.Point(0, 160);
            this.machineLabel.Name = "machineLabel";
            this.machineLabel.Size = new System.Drawing.Size(157, 31);
            this.machineLabel.TabIndex = 21;
            this.machineLabel.Text = "Устройство";
            // 
            // idBox
            // 
            this.idBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.idBox.Location = new System.Drawing.Point(160, 64);
            this.idBox.Name = "idBox";
            this.idBox.Size = new System.Drawing.Size(160, 31);
            this.idBox.TabIndex = 1;
            // 
            // machineIdBox
            // 
            this.machineIdBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.machineIdBox.Location = new System.Drawing.Point(160, 160);
            this.machineIdBox.Name = "machineIdBox";
            this.machineIdBox.Size = new System.Drawing.Size(160, 31);
            this.machineIdBox.TabIndex = 22;
            this.machineIdBox.Text = "1";
            // 
            // idLabeltext
            // 
            this.idLabeltext.AutoSize = true;
            this.idLabeltext.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.idLabeltext.Location = new System.Drawing.Point(0, 64);
            this.idLabeltext.Name = "idLabeltext";
            this.idLabeltext.Size = new System.Drawing.Size(42, 31);
            this.idLabeltext.TabIndex = 0;
            this.idLabeltext.Text = "ID";
            // 
            // saveAiBut
            // 
            this.saveAiBut.BackColor = System.Drawing.Color.LightCyan;
            this.saveAiBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveAiBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveAiBut.Location = new System.Drawing.Point(928, 640);
            this.saveAiBut.Name = "saveAiBut";
            this.saveAiBut.Size = new System.Drawing.Size(192, 64);
            this.saveAiBut.TabIndex = 19;
            this.saveAiBut.Text = "Сохранить AI изображение";
            this.saveAiBut.UseVisualStyleBackColor = false;
            this.saveAiBut.Click += new System.EventHandler(this.saveAiBut_Click);
            // 
            // photoToSetTextLabel
            // 
            this.photoToSetTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.photoToSetTextLabel.Location = new System.Drawing.Point(416, 608);
            this.photoToSetTextLabel.Name = "photoToSetTextLabel";
            this.photoToSetTextLabel.Size = new System.Drawing.Size(384, 32);
            this.photoToSetTextLabel.TabIndex = 20;
            this.photoToSetTextLabel.Text = "Фото на запись";
            this.photoToSetTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // recognizedPersonTextLabel
            // 
            this.recognizedPersonTextLabel.BackColor = System.Drawing.Color.LightCoral;
            this.recognizedPersonTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.recognizedPersonTextLabel.Location = new System.Drawing.Point(832, 608);
            this.recognizedPersonTextLabel.Name = "recognizedPersonTextLabel";
            this.recognizedPersonTextLabel.Size = new System.Drawing.Size(384, 32);
            this.recognizedPersonTextLabel.TabIndex = 21;
            this.recognizedPersonTextLabel.Text = "Не опознан";
            this.recognizedPersonTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // upCamBut
            // 
            this.upCamBut.BackColor = System.Drawing.Color.SkyBlue;
            this.upCamBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.upCamBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.upCamBut.Location = new System.Drawing.Point(0, 672);
            this.upCamBut.Name = "upCamBut";
            this.upCamBut.Size = new System.Drawing.Size(192, 64);
            this.upCamBut.TabIndex = 24;
            this.upCamBut.Text = "Камеру ВВЕРХ";
            this.upCamBut.UseVisualStyleBackColor = false;
            this.upCamBut.Click += new System.EventHandler(this.upCamBut_Click);
            // 
            // downCamBut
            // 
            this.downCamBut.BackColor = System.Drawing.Color.LightCoral;
            this.downCamBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downCamBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.downCamBut.Location = new System.Drawing.Point(192, 672);
            this.downCamBut.Name = "downCamBut";
            this.downCamBut.Size = new System.Drawing.Size(192, 64);
            this.downCamBut.TabIndex = 25;
            this.downCamBut.Text = "Камеру ВНИЗ";
            this.downCamBut.UseVisualStyleBackColor = false;
            this.downCamBut.Click += new System.EventHandler(this.downCamBut_Click);
            // 
            // faceControlProgramTextLabel
            // 
            this.faceControlProgramTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.faceControlProgramTextLabel.Location = new System.Drawing.Point(0, 0);
            this.faceControlProgramTextLabel.Name = "faceControlProgramTextLabel";
            this.faceControlProgramTextLabel.Size = new System.Drawing.Size(1216, 32);
            this.faceControlProgramTextLabel.TabIndex = 26;
            this.faceControlProgramTextLabel.Text = "ПРОГРАММА ФЭЙС КОНТРОЛЯ";
            this.faceControlProgramTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1216, 961);
            this.Controls.Add(this.faceControlProgramTextLabel);
            this.Controls.Add(this.downCamBut);
            this.Controls.Add(this.upCamBut);
            this.Controls.Add(this.recognizedPersonTextLabel);
            this.Controls.Add(this.photoToSetTextLabel);
            this.Controls.Add(this.saveAiBut);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.aiPictureBox);
            this.Controls.Add(this.takePhotoBut);
            this.Controls.Add(this.takenPhotoPictureBox);
            this.Controls.Add(this.saveCamBut);
            this.Controls.Add(this.camPictureBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.userDataListView);
            this.Name = "FaceForm";
            this.Text = "Программа фэйс контроля";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.camPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.takenPhotoPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aiPictureBox)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView userDataListView;
		private System.Windows.Forms.Button connectBut;
		private System.Windows.Forms.Button disconnectBut;
		private System.Windows.Forms.Button clearList;
        public System.Windows.Forms.TextBox textPort;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.PictureBox camPictureBox;
        public System.Windows.Forms.ComboBox camBox;
        private System.Windows.Forms.Label webCamTextLabel;
        private System.Windows.Forms.Button saveCamBut;
        private System.Windows.Forms.PictureBox takenPhotoPictureBox;
        private System.Windows.Forms.Button takePhotoBut;
        private System.Windows.Forms.PictureBox aiPictureBox;
        private System.Windows.Forms.Label idLabel;
        public System.Windows.Forms.TextBox ipAdressBox;
        private System.Windows.Forms.Label ipAddressTextLabel;
        private System.Windows.Forms.Label passwordTextLabel;
        public System.Windows.Forms.TextBox passwordBox;
        public System.Windows.Forms.TextBox ipPortBox;
        private System.Windows.Forms.Button testWriteButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox idBox;
        private System.Windows.Forms.Label idLabeltext;
        public System.Windows.Forms.TextBox machineIdBox;
        private System.Windows.Forms.Label machineLabel;
        private System.Windows.Forms.Button saveAiBut;
        private System.Windows.Forms.Label photoSelectedLabel;
        private System.Windows.Forms.Label photoTextLabel;
        private System.Windows.Forms.Label photoToSetTextLabel;
        private System.Windows.Forms.Label recognizedPersonTextLabel;
        private System.Windows.Forms.Label lastIdLabel;
        private System.Windows.Forms.Button newIdBut;
        private System.Windows.Forms.Button upCamBut;
        private System.Windows.Forms.Button downCamBut;
        private System.Windows.Forms.Button openEditDbFormBut;
        private System.Windows.Forms.Label faceControlProgramTextLabel;
    }
}

