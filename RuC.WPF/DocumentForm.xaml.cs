using System;
using System.Windows.Controls;
using ScintillaNET.WPF;
using System.IO;
using System.ComponentModel;
using ScintillaNET;
using System.Globalization;
using System.Windows;
using System.Diagnostics;
using ScintillaNET_FindReplaceDialog;
using Xceed.Wpf.AvalonDock.Layout;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace RuC.WPF
{
	/// <summary>
	/// Interaction logic for DocumentForm.xaml
	/// </summary>
	public partial class DocumentForm : LayoutDocument
	{
		public FindReplace FindReplace { get; set; }

		public DocumentForm()
		{
			InitializeComponent();
			this.Title = "";
			this.Closing += new EventHandler<CancelEventArgs>(DocumentForm_Closing);
			Scintilla.MouseDown += Scintilla_MouseDown;
			Scintilla.SavePointLeft += Scintilla_SavePointLeft;
			Scintilla.UpdateUI += Scintilla_SavePointLeft;
		}

		private void Scintilla_SavePointLeft(object sender, EventArgs e)
		{
			AddOrRemoveAsteric();
		}

		private void Scintilla_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			switch (e.Button)
			{
				default:
					break;
			}
		}

		public ScintillaWPF Scintilla
		{
			get { return scintilla; }
		}

		private string _filePath;

		public string FilePath
		{
			get { return _filePath; }
			set { _filePath = value; }
		}

		private void AddOrRemoveAsteric()
		{
			if (scintilla.Modified)
			{
				if (!Title.EndsWith("*", StringComparison.InvariantCulture))
					Title += "*";
			}
			else
			{
				if (Title.EndsWith("*", StringComparison.InvariantCulture))
					Title = Title.Substring(0, Title.Length - 1);
			}
		}

		private async void DocumentForm_Closing(object sender, CancelEventArgs e)
		{
			if (Scintilla.Modified)
			{
				if (this.Scintilla.Visibility == Visibility.Collapsed)
				{
					return;
				}

				// Prompt if not saved
				string message = String.Format(CultureInfo.CurrentCulture, Application.Current.Resources["m_DocumentForm"].ToString(), Title.TrimEnd(' ', '*'), Environment.NewLine, Environment.NewLine);

				Task<MessageDialogResult> dc = (Application.Current.MainWindow as MetroWindow).ShowMessageAsync(
					Program.Title,
					message,
					MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
					new MetroDialogSettings()
						{
							DefaultButtonFocus = MessageDialogResult.FirstAuxiliary,
							AffirmativeButtonText = Application.Current.Resources["m_DocumentYes"].ToString(),
							NegativeButtonText = Application.Current.Resources["m_DocumentNo"].ToString(),
							FirstAuxiliaryButtonText = Application.Current.Resources["m_DocumentCancel"].ToString()
						});

				e.Cancel = true;
				this.Scintilla.Visibility = Visibility.Collapsed;
				MessageDialogResult dr = await dc;
				if (dr == MessageDialogResult.FirstAuxiliary)
				{
					// Stop closing
					this.Scintilla.Visibility = Visibility.Visible;
					return;
				}

				e.Cancel = false;
				if (dr == MessageDialogResult.Affirmative)
				{
					// Try to save before closing
					e.Cancel = !Save();
				}
				this.Close();
			}

			// Close as normal
		}

		public bool ExportAsHtml()
		{
			// TODO - Implement ExportAsHtml feature

			//SaveFileDialog dialog = new SaveFileDialog();
			//string fileName = (Title.EndsWith(" *") ? Title.Substring(0, Title.Length - 2) : Title);
			//dialog.Filter = "HTML Files (*.html;*.htm)|*.html;*.htm|All Files (*.*)|*.*";
			//dialog.FileName = fileName + ".html";
			//bool? res = dialog.ShowDialog();
			//if (res != null && (bool)res)
			//{
			//	scintilla.Lexing.Colorize(); // Make sure the document is current
			//	using (StreamWriter sw = new StreamWriter(dialog.FileName))
			//		scintilla.ExportHtml(sw, fileName, false);

			//	return true;
			//}

			return false;
		}

		public bool Save()
		{
			if (String.IsNullOrEmpty(_filePath))
				return SaveAs();

			return Save(_filePath);
		}

		public bool Save(string filePath)
		{
			using (FileStream fs = File.Create(filePath))
			{
				using (BinaryWriter bw = new BinaryWriter(fs))
					bw.Write(scintilla.Text.ToCharArray(), 0, scintilla.Text.Length - 1); // Omit trailing NULL
			}
			this.Title = Path.GetFileName(filePath);

			scintilla.SetSavePoint();
			return true;
		}

		public bool SaveAs()
		{
			bool? res = saveFileDialog.ShowDialog();
			if (res != null && (bool)res)
			{
				_filePath = saveFileDialog.FileName;
				return Save(_filePath);
			}

			return false;
		}
	}
}