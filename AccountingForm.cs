using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace vlc_works
{
	public partial class AccountingForm : Form
	{
		ClientForm clientForm { get; set; }
		// some
		long Now() => DateTimeOffset.Now.ToUnixTimeSeconds();
		DateTimeOffset SecToTime(long unixSeconds) => DateTimeOffset.FromUnixTimeSeconds(unixSeconds);

		public AccountingForm(ClientForm clientForm)
		{
			InitializeComponent();

			this.clientForm = clientForm;
			Db.BeginSQL();
			StartTables();
		}

		private void StartTables()
		{
			throw new NotImplementedException();
		}

		private void AccountingForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Db.EndSQL();
			Environment.Exit(0);
		}
	}
}
