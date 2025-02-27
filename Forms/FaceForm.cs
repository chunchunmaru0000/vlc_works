using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using AxFPCLOCK_SVRLib;
using AxFP_CLOCKLib;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace vlc_works
{
	public partial class FaceForm : Form
	{
        #region VAR

        private AccountingForm accountingForm;
        public EditDbForm editDbForm;

		private AxFPCLOCK_Svr axFPCLOCK_Svr { get; set; }
        private AxFP_CLOCK axFP_CLOCK { get; set; }
		private int machineNumber = 1;
		private long lastCode { get; set; } = -1;
        private const string webCamPhotosDirectory = "web_cam_photos";
        private const string aiCamPhotosDirectory = "ai_cam_photos";

        #endregion VAR

        public FaceForm(AccountingForm accountingForm)
		{
			InitializeComponent();
            this.accountingForm = accountingForm;

			InitaxFPCLOCK_Svr();
			InitListView();
        }

		private void print(object obj)
		{
			string str = obj == null ? "" : obj.ToString();

            Console.WriteLine(str);

            //const string testFileName = "test.txt";
            //File.AppendAllText(testFileName, str, encoding: System.Text.Encoding.UTF8);
        }

		#region WEB_CAM

		private FilterInfoCollection videoDevices { get; set; }
		private VideoCaptureDevice videoCaptureDevice { get; set; }

        private void camBox_DropDown(object sender, EventArgs e)
        {
			videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			camBox.Items.Clear();

			for (int i = 0; i < videoDevices.Count; i++)
				camBox.Items.Add($"{i} {videoDevices[i].Name}");
        }

        private void camBox_SelectedIndexChanged(object sender, EventArgs e)
        {
			string camName = string.Join(" ", camBox.SelectedItem.ToString().Split(' ').Skip(1));
            for (int i = 0; i < videoDevices.Count; i++)
			{
				if (videoDevices[i].Name == camName)
				{
					videoCaptureDevice?.Stop();

                    videoCaptureDevice = new VideoCaptureDevice(videoDevices[0].MonikerString);
                    videoCaptureDevice.NewFrame += WebCamNewFrame;
                    videoCaptureDevice.Start();
                }
			}
        }

		private void WebCamNewFrame(object sender, NewFrameEventArgs e)
		{
			camPictureBox.Image?.Dispose();
			camPictureBox.Image = (Bitmap)e.Frame.Clone();
		}

        private void takePhotoBut_Click(object sender, EventArgs e)
        {
            if (camPictureBox.Image == null)
                return;

			takenPhotoPictureBox.Image = (Bitmap)camPictureBox.Image.Clone();
            photoSelectedLabel.Text = "ВЫБРАНО";
        }

        private void saveCamBut_Click(object sender, EventArgs e)
        {
			if (takenPhotoPictureBox.Image == null)
				return;

            SavePhoto(webCamPhotosDirectory, takenPhotoPictureBox);
            Directory.CreateDirectory(webCamPhotosDirectory);
            string imageName = $"{webCamPhotosDirectory}\\img_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.jpg";
            takenPhotoPictureBox.Image.Save(imageName, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private string SavePhoto(string directory, PictureBox pictureBox, string prefix = "", long code = -1)
        {
            code = code < 0 ? lastCode : code;
            string imageName = $"{directory}\\{prefix}img_{code}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.jpg";

            Directory.CreateDirectory(directory);
            pictureBox.Image.Save(imageName, System.Drawing.Imaging.ImageFormat.Jpeg);

            return imageName;
        }

        private void saveAiBut_Click(object sender, EventArgs e)
        {
            if (aiPictureBox.Image == null)
                return;

            SavePhoto(aiCamPhotosDirectory, aiPictureBox);
        }

        #endregion WEB_CAM

        #region BEGIN_INITS
        private void InitaxFPCLOCK_Svr()
		{
            string stateStr = @"
        AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4w
        LCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAACFTeXN0
        ZW0uV2luZG93cy5Gb3Jtcy5BeEhvc3QrU3RhdGUBAAAABERhdGEHAgIAAAAJAwAAAA8DAAAAJQAAAAIB
        AAAAAQAAAAAAAAAAAAAAABAAAAAAAAEAVgoAADgEAAAAAAAACw==
";
            byte[] serializedData = Convert.FromBase64String(stateStr);


			using (MemoryStream ms = new MemoryStream(serializedData))
			axFPCLOCK_Svr = new AxFPCLOCK_Svr() 
			{
				Name = "axFPCLOCK_Svr", 
                OcxState = new AxHost.State(ms, 1, false, null),
				Location = new System.Drawing.Point(0, -100)
            };
			axFPCLOCK_Svr.OnReceiveGLogData += axFPCLOCK_Svr1_OnReceiveGLogData;


            using (MemoryStream ms = new MemoryStream(serializedData))
			axFP_CLOCK = new AxFP_CLOCK() 
			{ 
				Name = "axFP_CLOCK",
				OcxState = new AxHost.State(ms, 1, false, null),
                Location = new System.Drawing.Point(0, -100)
            };


            Controls.Add(axFPCLOCK_Svr);
			Controls.Add(axFP_CLOCK);
		}

		private void InitListView()
		{
			userDataListView.Columns.Add(" ", 40, HorizontalAlignment.Left);          //一步添加
			userDataListView.Columns.Add("EnrollNo", 100, HorizontalAlignment.Left);
			userDataListView.Columns.Add("VerifyMode", 100, HorizontalAlignment.Left);
			userDataListView.Columns.Add("InOut", 60, HorizontalAlignment.Left);
			userDataListView.Columns.Add("DateTime", 140, HorizontalAlignment.Left);
			userDataListView.Columns.Add("IP", 130, HorizontalAlignment.Left);
			userDataListView.Columns.Add("Port", 60, HorizontalAlignment.Left);
			userDataListView.Columns.Add("DevID", 60, HorizontalAlignment.Left);
			userDataListView.Columns.Add("SerialNo", 60, HorizontalAlignment.Left);
		}
		#endregion

		#region BUTS
		private void Connect_Click(object sender, EventArgs e)
		{
			if (int.TryParse(textPort.Text, out int port))
			{
				if (OpenAxFP_CLOCK())
				{
					connectBut.Enabled = false;
					disconnectBut.Enabled = true;
                    connectBut.BackColor = Color.LightGreen;
                    disconnectBut.BackColor = Color.LightGreen;
                    axFPCLOCK_Svr.OpenNetwork(port);
				}
				else
                    MessageBox.Show("НЕ ПОДКЛЮЧИЛОСЬ");
            }
			else
				MessageBox.Show("НЕВРНОЕ ЧИСЛО ДЛЯ ОТКРЫВАЕМОГО ПОРТА");
		}

        private bool OpenAxFP_CLOCK()
        {
			machineNumber = int.Parse(machineIdBox.Text);
            int nPort = Convert.ToInt32(ipPortBox.Text);
            int nPassword = Convert.ToInt32(passwordBox.Text);
            string strIP = ipAdressBox.Text;

            axFP_CLOCK.OpenCommPort(machineNumber);
            if (!axFP_CLOCK.SetIPAddress(ref strIP, nPort, nPassword))
                return false;
			return axFP_CLOCK.OpenCommPort(machineNumber);
            /*
			switch (nConnecttype)
			{
				case (int)CURDEVICETYPE.DEVICE_NET:
					#################################
							currently used
					#################################
					break;
				case (int)CURDEVICETYPE.DEVICE_COM:
					axFP_CLOCK.CommPort = cmbComPort.SelectedIndex + 1;
					axFP_CLOCK.Baudrate = 38400;
					break;
				case (int)CURDEVICETYPE.DEVICE_USB:
					axFP_CLOCK.IsUSB = true;
					break;
				case (int)CURDEVICETYPE.DEVICE_P2S:
					int nPort = Convert.ToInt32(P2SPort.Text);
					int nTimeOut = Convert.ToInt32(P2STimeOut.Text);
					axFP_CLOCK.SetServerPortandtick(nPort, nTimeOut);
					break;
			}
             */
        }

        private void Disconnect_Click(object sender, EventArgs e)
		{
			if (int.TryParse(textPort.Text, out int port))
			{
				axFPCLOCK_Svr.CloseNetwork(port);
                axFP_CLOCK.CloseCommPort();
                connectBut.Enabled = true;
				disconnectBut.Enabled = false;
                connectBut.BackColor = Color.LightCoral;
                disconnectBut.BackColor = Color.LightCoral;
            }
			else
				MessageBox.Show("НЕВРНОЕ ЧИСЛО ДЛЯ ЗАКРЫВАЕМОГО ПОРТА");
		}

		private void clearList_Click(object sender, EventArgs e)
		{

			nIndex = 0;
			userDataListView.Items.Clear();
		}
        #endregion

        #region AX_FP_CLOCK
        public T PerformOperation<T>(Func<T> operation, bool close = true, bool open = true)
        {
            if (axFP_CLOCK == null)
                return default;
            /*
			 * When the device is not available for attendance 
			 * it is in a busy state and the user cannot perform attendance operations, and vice versa.
			 * Before performing any operation, the device should be set to the non-attendance state (bFlag=0), 
			 */
			if (open)
				Invoke(new Action(() => axFP_CLOCK.EnableDevice(machineNumber, 0)));

            // do operation
            T result = operation();

            /*
			 * and at the same time, after performing the corresponding operation, 
			 * the device should be set to the attendance state (bFlag=1).
			 */
			if (close)
                Invoke(new Action(() => axFP_CLOCK.EnableDevice(machineNumber, 1)));

			return result;
        }
        #endregion

        #region AX_SRV(SURVEILANCE?)
        private int nIndex { get; set; } = 0;
        private readonly Dictionary<int, string> actions = new Dictionary<int, string>()
        {
            { 0, "Closed" },   { 1, "Opened" },
            { 2, "HandOpen" }, { 3, "ProcOpen" },
            { 4, "ProcClose" }, { 5, "IllegalOpen" },
            { 6, "IlleagalRemove" }, { 7, "Alarm" },
            { 8, "--" }
        };

        public string FormString(int nVerify, int nEnrollNum) =>
			nEnrollNum == 0 ? actions[nVerify % 8] : nVerify.ToString();

		public string FormStringlong(int nVerify, long nEnrollNum) =>
			nEnrollNum == 0 ? actions[nVerify % 8] : nVerify.ToString();

		private void axFPCLOCK_Svr1_OnReceiveGLogData(object sender, _DFPCLOCK_SvrEvents_OnReceiveGLogDataEvent e)
		{
			string strKey = Convert.ToString(nIndex + 1);
			string str = e.anSEnrollNumber.ToString("D8");
			long dwCardNum1 = 0;
			double aTemperature;

			/*
			print(string.Join("\n", new object[] {
				$"DEVICE PORT      [{e.anDevicePort}]",
				$"anSEnrollNumber  [{e.anSEnrollNumber}]",
				$"astrDeviceIP     [{e.astrDeviceIP}]",
				$"vnDeviceID       [{e.vnDeviceID}]",
				$"anSN             [{e.anSN}]",
			}.Select(o => o.ToString())) + '\n');
			 */

			if (e.anInOutMode != 0)
			{
                axFPCLOCK_Svr.SendResultandTime(e.linkindex, e.vnDeviceID, e.anSEnrollNumber, 1);
                return;
			}

			if (e.anSEnrollNumber > 0)
			{
				int imagelen = 0;
				int[] imagebuff = new int[200 * 1024];
				bool bRet;
				IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(imagebuff.Length);
				bRet = axFPCLOCK_Svr.GetLogImageCS(e.linkindex, ref imagelen, ptrIndexFacePhoto);
				if (bRet && imagelen > 0)
				{
					byte[] mbytCurEnrollData = new byte[imagelen];
					Marshal.Copy(ptrIndexFacePhoto, mbytCurEnrollData, 0, imagelen);

                    string imageName = e.anSEnrollNumber.ToString() + "_" + e.anLogDate.ToString("yy_MM_dd_HH_mm_ss") + ".png";
					try
					{
						byte[] imageBytes = new byte[imagelen];
						mbytCurEnrollData.CopyTo(imageBytes, 0);

						aiPictureBox.Image = new Bitmap(new MemoryStream(imageBytes)).Clone() as Bitmap;
					} 
					catch { }

					//File.WriteAllBytes(imageName, mbytCurEnrollData);
				}
				Marshal.FreeHGlobal(ptrIndexFacePhoto);
			}

			//数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
			userDataListView.BeginUpdate();
            #region LIST_ADD_DATA
            //this.listView1.Focus();
            ListViewItem lvi = new ListViewItem();
			lvi.Text = strKey;

			if (e.anSEnrollNumber < 0)
			{
                dwCardNum1 = e.anSEnrollNumber + 4294967296;
				str = dwCardNum1.ToString();
				lvi.SubItems.Add(str);
			}
			else
				lvi.SubItems.Add(str);

			if (e.anSEnrollNumber > 0 && e.anSEnrollNumber != 99999999)
			{
                int enrollId = e.anSEnrollNumber;

                lastCode = enrollId;
                lastIdLabel.Text = lastCode.ToString();

                recognizedPersonTextLabel.Text = "Опознан";
                recognizedPersonTextLabel.BackColor = Color.LightGreen;

                accountingForm.SetUserId(enrollId);
                accountingForm.Invoke(new Action(() =>
                accountingForm.requestDbUserDataBut_Click(null, EventArgs.Empty)));
            } 
            else //if (e.anSEnrollNumber < 0)
            {
                recognizedPersonTextLabel.Text = "Не опознан";
                recognizedPersonTextLabel.BackColor = Color.LightCoral;
            }

            if (e.anVerifyMode > 40)
            {
                aTemperature = e.anVerifyMode;
                aTemperature = (250 + aTemperature) / 10;
                str = aTemperature.ToString("#0.0");
            }
            else
                str = FormString(e.anVerifyMode, e.anSEnrollNumber);

			if (e.anSEnrollNumber < 0)
			{
				dwCardNum1 = e.anSEnrollNumber + 4294967296;
				str = FormStringlong(e.anVerifyMode, dwCardNum1);
				lvi.SubItems.Add(str);
			}
			else
				lvi.SubItems.Add(str);

			if (e.anInOutMode == 1)
				str = "OUT";
			else if (0 == e.anInOutMode)
				str = "IN";
			else
				str = "--";

			lvi.SubItems.Add(str);

			str = Convert.ToString(e.anLogDate.ToString("yyyy/MM/dd HH:mm:ss"));
			lvi.SubItems.Add(str);

			lvi.SubItems.Add(e.astrDeviceIP);

			str = Convert.ToString(e.anDevicePort);
			lvi.SubItems.Add(str);

			str = Convert.ToString(e.vnDeviceID);
			lvi.SubItems.Add(str);

			str = Convert.ToString(e.anSN);
			lvi.SubItems.Add(str);

			userDataListView.Items.Add(lvi);
            #endregion LIST_ADD_DATA
            userDataListView.Update();

			userDataListView.EnsureVisible(nIndex);
			userDataListView.EndUpdate();  //结束数据处理，UI界面一次性绘制。

			int nResult = 1;

			axFPCLOCK_Svr.SendResultandTime(e.linkindex, e.vnDeviceID, e.anSEnrollNumber, nResult);

			nIndex++;
			if (nIndex > 1000)
			{
				nIndex = 0;
				userDataListView.Items.Clear();
			}

		}

        #endregion

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
			/*
			try
			{
				axFP_CLOCK?.CloseCommPort();
				axFPCLOCK_Svr?.CloseNetwork(int.Parse(textPort.Text)); // System.AccessViolationException
			}
			catch { }
            videoCaptureDevice?.Stop();
			Application.Exit();
			Environment.Exit(0);
			 */
        }

        #region WRITE_PLAYER_AREA

        private void testButton_Click(object sender, EventArgs e)
        {
			machineNumber = int.Parse(machineIdBox.Text);
            /*
             * ReadAllUserID()
			 * This function reads all the fingerprint data from the terminal into memory, 
			 * and then you must call GetAllUserID() to retrieve the fingerprint data 
			 * from memory one by one.
			 */
            PerformOperation(() => axFP_CLOCK.ReadAllUserID(machineNumber));

            int dwEnrollNumber = 0;
            int dwEMachineNumber = 0;
            int dwBackupNumber = 0;
            int dwUserPrivilege = 0;
            int dwAttendenceEnable = 0;

			int times = 0;

			while (times++ < 5000)
            {
				bool res = PerformOperation<bool>(() => axFP_CLOCK
					.GetAllUserID(
						machineNumber,
						ref dwEnrollNumber,
						ref dwEMachineNumber,
						ref dwBackupNumber,
						ref dwUserPrivilege,
						ref dwAttendenceEnable
						));

				if (!res) 
					break;

				//print(userID.ToString().Replace('\n', ' ').Replace("\r\n", " ") + '\n');

				if (dwBackupNumber == (int)BackupNum.AIFace)
				{
					int[] indexDataFacePhoto = new int[400800]; // BackupNum.AIFace...
																// 400800 = 835 * 480 so it's like 480p?
					IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(indexDataFacePhoto.Length);
					int dwPhotoSize = 0;

					PerformOperation(() => axFP_CLOCK
						.GetEnrollPhotoCS(
							machineNumber,
							dwEnrollNumber,
							ref dwPhotoSize,
                            ptrIndexFacePhoto
                            ));

					Directory.CreateDirectory("photos");
					string photoName = $"photos\\{dwEnrollNumber}_{DateTimeOffset.Now.ToUnixTimeSeconds()}.jpg";

					byte[] photoBytes = new byte[dwPhotoSize];
					Marshal.Copy(ptrIndexFacePhoto, photoBytes, 0, dwPhotoSize);
					//File.WriteAllBytes(photoName, photoBytes);

                    //print($"PHOTO |> {photoName}\n");
                }
            }

            if (times < 2)
                print("ничего\n");

            print("Успешно прочитано\n");
        }

        private void WriteButton_Click(object sender, EventArgs e)
        {
			if (takenPhotoPictureBox.Image == null)
			{
                MessageBox.Show("ФОТО НЕ ВЫБРАНО");
				print("PHOTO IS NOT SELECTED AND TRYED TO WRITE USER");
				return;
			}

            int enrollId;
            if (int.TryParse(idBox.Text, out int id))
                enrollId = id;
            else
            {
                MessageBox.Show("НЕВЕРНЫЙ ФОРМАТ ID");
                return;
            }
			machineNumber = int.Parse(machineIdBox.Text);

            string photoPath = 
                SavePhoto(
                    webCamPhotosDirectory, 
                    takenPhotoPictureBox, 
                    prefix: "tmp_",
                    code: enrollId
                    );

			byte[] photoBytes = File.ReadAllBytes(photoPath);
            IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(photoBytes.Length);
            Marshal.Copy(photoBytes, 0, ptrIndexFacePhoto, photoBytes.Length);

            bool setEnrollPhotoCSResult =
				PerformOperation(() => axFP_CLOCK
				.SetEnrollPhotoCS(
                    machineNumber,
					enrollId,
                    photoBytes.Length,
					ptrIndexFacePhoto
					));

            print(string.Join("\n", new string[] {
				$"[BEGIN SET USER]",
                $"USER ENROLL ID:      [{enrollId}]",
                $"MACHINE ENROLL ID:   [{machineNumber}]",
                $"PHOTO PATH:          [{photoPath}]",
				"",
				$"SET ENROLL PHOTO:    [{setEnrollPhotoCSResult}]",
				$"[END SET USER]",
            }) + '\n');

            if (!setEnrollPhotoCSResult)
                MessageBox.Show("ФОТО НЕ БЫЛО ДОБАВЛЕНО");
            else
            {
                accountingForm.SetUserId(enrollId);
                accountingForm.Invoke(new Action(() => 
                accountingForm.requestDbUserDataBut_Click(null, EventArgs.Empty)));
                // does RefreshDbForm(); in requestDbUserDataBut_Click
            }
        }

        private void newIdBut_Click(object sender, EventArgs e)
        {
            long id = Db.AutoincrementCounter(Db.PlayersTableName) + 1; // AUTOINCREMENT + 1
            idBox.Text = id.ToString();
        }
        #endregion

        #region REALY
        private void upCamBut_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                RelayChecker.Transmit(1, true); // camera UP on
                Thread.Sleep(1000);
                RelayChecker.Transmit(1, false); // camera UP off
            }).Start();
        }

        private void downCamBut_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                RelayChecker.Transmit(2, true); // camera DOWN on
                Thread.Sleep(1000);
                RelayChecker.Transmit(2, false); // camera DOWN off
            }).Start();
        }
        #endregion RELAY

        private void openEditDbFormBut_Click(object sender, EventArgs e)
        {
            if (editDbForm != null && !editDbForm.IsDisposed)
                return;

            editDbForm = new EditDbForm(this, axFP_CLOCK, machineNumber);
            editDbForm.Show();
            editDbForm.Location = new System.Drawing.Point(2000, 100);
        }
    }
}
