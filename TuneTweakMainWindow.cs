using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TuneTweak.Actors;
using TuneTweak.Interfaces;

namespace TuneTweak
{
    public partial class TuneTweakMainWindow : Form, IUIUpdater
    {
        private List<string> selectedFiles = new List<string>();
        private Dictionary<string, Dictionary<string, string>> fileMetadata = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, Image> fileAlbumArt = new Dictionary<string, Image>();

        private string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
        private string ffprobePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffprobe.exe");

        private IActorRef coordinatorActor = null!;

        private readonly Image defaultAlbumArt = null!;

        public TuneTweakMainWindow()
        {
            InitializeComponent();
        }

        public void SetCoordinator(IActorRef coordinator)
        {
            coordinatorActor = coordinator;
        }

        public void EnableControls(bool enable)
        {
            BeginInvoke(new Action(() =>
            {
                btnConvert.Enabled = enable && selectedFiles.Count > 0;
                btnSelectFiles.Enabled = enable;
                btnSelectFolder.Enabled = enable;
                // updateMetadataButton.Enabled = enable && lvFilesToConvert.SelectedItems.Count > 0;
            }));
        }
        public void SelectFileInListView(string filePath)
        {
            BeginInvoke(new Action(() =>
            {
                foreach (DataGridViewRow row in dgvFilesToConvert.Rows)
                {
                    var fileCellValue = row.Cells["File"].Value;
                    if (fileCellValue is string file && !string.IsNullOrEmpty(file))
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }));
        }

        public void UpdateFileList(string filePath, Dictionary<string, string> metadata, Image albumArt)
        {
            BeginInvoke(new Action(() =>
            {
                if (!selectedFiles.Contains(filePath, StringComparer.OrdinalIgnoreCase))
                {
                    selectedFiles.Add(filePath);
                    fileMetadata[filePath] = metadata;
                    fileAlbumArt[filePath] = albumArt ?? defaultAlbumArt;
                    dgvFilesToConvert.Rows.Add(Path.GetFileName(filePath), metadata.GetValueOrDefault("title", ""), metadata.GetValueOrDefault("artist", ""), metadata.GetValueOrDefault("album", ""), albumArt ?? defaultAlbumArt);
                    lblStatus.Text = $"{selectedFiles.Count} file(s) selected.";
                    btnConvert.Enabled = selectedFiles.Count > 0;
                }
            }));
        }

        public void UpdateMetadata(string filePath, string title, string artist)
        {
            //BeginInvoke(new Action(() =>
            //{
            //    fileMetadata[filePath]["title"] = title;
            //    fileMetadata[filePath]["artist"] = artist;
            //    foreach (ListViewItem item in lvFilesToConvert.Items)
            //    {
            //        if (item.Text == Path.GetFileName(filePath))
            //        {
            //            item.SubItems[1].Text = title;
            //            item.SubItems[2].Text = artist;
            //            lblStatus.Text = $"Metadata updated for {Path.GetFileName(filePath)}";
            //            break;
            //        }
            //    }
            //}));
        }

        public void UpdateProgress(string filePath, int progress)
        {
            BeginInvoke(new Action(() =>
            {
                pbConversion.Value = Math.Min(progress, 100);
                lblStatus.Text = $"Processing: {Path.GetFileName(filePath)} ({progress}%)";
            }));
        }

        public void UpdateStatus(string message)
        {
            BeginInvoke(new Action(() => lblStatus.Text = message));
        }

        private void dgvFilesToConvert_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFilesToConvert.SelectedRows.Count > 0)
            {
                var fileCellValue = dgvFilesToConvert.SelectedRows[0].Cells["File"].Value;
                if (fileCellValue is string file && !string.IsNullOrEmpty(file))
                {
                    string? fullPath = selectedFiles.Find(f => Path.GetFileName(f) == file);
                    if (fullPath != null && fileMetadata.TryGetValue(fullPath, out var metadata))
                    {
                        txtTitle.Text = metadata.GetValueOrDefault("title", "");
                        txtArtist.Text = metadata.GetValueOrDefault("artist", "");
                        //updateMetadataButton.Enabled = true;
                        //playButton.Enabled = true;
                        coordinatorActor.Tell(new SelectFileInListViewMessage(fullPath));
                    }
                    else
                    {
                        txtTitle.Text = "";
                        txtArtist.Text = "";
                        //updateMetadataButton.Enabled = false;
                        //playButton.Enabled = false;
                    }
                }
                else
                {
                    txtTitle.Text = "";
                    txtArtist.Text = "";
                    //updateMetadataButton.Enabled = false;
                    //playButton.Enabled = false;
                }
            }
            else
            {
                txtTitle.Text = "";
                txtArtist.Text = "";
                //updateMetadataButton.Enabled = false;
                //playButton.Enabled = false;
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("Please select at least one audio file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(ffmpegPath) || !File.Exists(ffprobePath))
            {
                MessageBox.Show("FFmpeg or FFprobe not found in the 'bin' folder. Please ensure they are included.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cboConversionFormat.SelectedItem == null)
            {
                MessageBox.Show("Please select a conversion format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string outputFormat = cboConversionFormat.SelectedItem.ToString() ?? string.Empty;
            coordinatorActor.Tell(new ConvertFilesMessage(outputFormat));
        }

        private void TuneTweakMainWindow_Load(object sender, EventArgs e)
        {
            dgvFilesToConvert.Columns.Add(new DataGridViewTextBoxColumn { Name = "File", HeaderText = "File", FillWeight = 350 });
            dgvFilesToConvert.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title", HeaderText = "Title", FillWeight = 200 });
            dgvFilesToConvert.Columns.Add(new DataGridViewTextBoxColumn { Name = "Artist", HeaderText = "Artist", FillWeight = 150 });
            dgvFilesToConvert.Columns.Add(new DataGridViewTextBoxColumn { Name = "Album", HeaderText = "Album", FillWeight = 150 });
            var imageColumn = new DataGridViewImageColumn
            {
                Name = "AlbumArt",
                HeaderText = "Album Art",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                FillWeight = 100
            };
            dgvFilesToConvert.Columns.Add(imageColumn);

            cboConversionFormat.Items.AddRange(new string[] { "mp3", "wav", "aac", "ogg", "flac" });
            cboConversionFormat.SelectedIndex = 0;
        }

        private async void btnSelectFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Audio Files|*.mp3;*.wav;*.aac;*.flac;*.ogg|All Files|*.*"
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    await coordinatorActor.Ask(new SelectFilesMessage(openFileDialog.FileNames));
                }
            }
        }
    }
}
