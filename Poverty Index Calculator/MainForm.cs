using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using PovertyIndex.DomainModel;
using FileHelpers;
using System.Diagnostics;

namespace PovertyIndexCalculator
{
    public partial class MainForm : XtraForm
    {
        private PanelData panelData;
        private List<PovertyIndexResult> indexes;

        public MainForm()
        {
            InitializeComponent();
            panelData = new PanelData();
            indexes = new List<PovertyIndexResult>();
            
            string startingDir = 
                Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            openFileDialog.InitialDirectory = startingDir;
            saveFileDialog.InitialDirectory = startingDir;
        }

        private void LoadPanel(string filename)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                BeforeLoadingPanelData();
                PanelData tmp = new PanelData();
                tmp.LoadFromFile(openFileDialog.FileName);
                panelData = tmp;
                OnLoadPanelData();
            }
            catch (BusinessException exc)
            {
                XtraMessageBox.Show(
                    exc.Message,
                    "Error Loading File",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BeforeLoadingPanelData()
        {
            ClearControls();
        }

        private void ClearControls()
        {
            gridControlPanelData.DataSource = null;
            gridControlPeople.DataSource = null;            
            gridControlPovertyPersistanceRatios.DataSource = null;
            gridControlPanelErrors.DataSource = null;
            gridControlPovertIndexes.DataSource = null;
            indexes.Clear();
        }

        private void OnLoadPanelData()
        {
            gridControlPanelData.DataSource = panelData.Observations;
            gridControlPeople.DataSource = panelData.People;            
            gridControlPovertyPersistanceRatios.DataSource = panelData.PovertyPersistenceRatios;
            gridControlPanelErrors.DataSource = panelData.Errors;

            if (!panelData.IsValid)
            {
                dockPanelErrors.Visibility = DockVisibility.Visible;

                XtraMessageBox.Show(
                    "The panel is not valid. Please fix errors in file and load it again",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else
            {
                dockPanelErrors.Visibility = DockVisibility.Hidden;
                LoadPovertyIndexes();
            }
        }

        private void LoadPovertyIndexes()
        {
            if (panelData.IsValid)
            {
                double[] alphaValues = { 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };
                foreach (var alpha in alphaValues)
                {
                    indexes.AddRange(panelData.CalculatePovertyIndex(alpha));
                }                
                gridControlPovertIndexes.DataSource =
                    indexes;
            }
        }

        private void biExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        private void biLoadFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                LoadPanel(openFileDialog.FileName);
            }
        }

        private void biViewPanelErrors_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dockPanelErrors.Visibility = DockVisibility.Visible;
        }

        private void biSavePovertyPersistenceRatios_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            saveFileDialog.FileName = "PPProbs.txt";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var writer = new FileHelperEngine<PovertyPersistenceRatio>();
                writer.WriteFile(saveFileDialog.FileName, panelData.PovertyPersistenceRatios);
            }
        }

        private void biSavePovertyIndexes_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            saveFileDialog.FileName = "PIndices.txt";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var writer = new FileHelperEngine<PovertyIndexResult>();
                writer.WriteFile(saveFileDialog.FileName, indexes);
            }
        }

        private void menuExport_Popup(object sender, EventArgs e)
        {
            biSavePovertyIndexes.Enabled = panelData.IsValid && indexes.Count > 0;
            biSavePovertyPersistenceRatios.Enabled = panelData.IsValid && panelData.PovertyPersistenceRatios.Count > 0;
        }

        private void biHelpAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (AboutBox aboutBox = new AboutBox())
            {
                aboutBox.ShowDialog(this);
            }
        }

        private void biHelpReadme_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dirname = Path.GetDirectoryName(Application.ExecutablePath);
            var readme = Path.Combine(dirname, @"Resources\readme.pdf");

            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = readme;

            process.Start();
        }
    }
}