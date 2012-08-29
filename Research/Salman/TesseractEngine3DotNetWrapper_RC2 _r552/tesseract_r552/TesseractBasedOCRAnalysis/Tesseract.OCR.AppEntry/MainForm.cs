﻿/**
Copyright 2011, Cong Nguyen

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

using tesseract;
using IPoVn.Engine.IPMath;
using IPoVn.Engine.IPCommon;
using System.IO;
using Tesseract.OCR.AppEntry.UI;

namespace Tesseract.OCR.AppEntry
{
    public enum eOcrEngineMode : int
    {
        TESSERACT_ONLY = 0,
        CUBE_ONLY = 1,
        TESSERACT_CUBE_COMBINED = 2,
        DEFAULT = 3
    }

    public partial class MainForm : Form
    {
        #region Member fields
        private new const string ProductName = "Tesseract-based OCR";
        private const string FileName_GeneralSettings = "general_settings.settings";
        private const string FileName_RecentFiles = "recent_files.settings";

        private string _fileName = string.Empty;
        private IImage _image = null;
        
        private TesseractProcessor _ocrProcessor = null;
        private string _tessData = "";
        private string _lang = "eng";
        private int _ocrEngineMode = (int)eOcrEngineMode.DEFAULT;
        #endregion Member fields

        #region Properties
        private string FileName
        {
            get { return _fileName; }

            set
            {
                if (_fileName != value)
                {
                    _fileName = value;

                    this.Text = string.Format("{0} - [{1}]", ProductName, _fileName);
                }
            }
        }
        #endregion Properties

        #region Constructors and destructors
        public MainForm()
        {
            InitializeComponent();

            Initialize();
        }

        private void End()
        {
            if (_ocrProcessor != null)
            {
                _ocrProcessor.End();
                _ocrProcessor = null;
            }
        }
        #endregion Constructors and destructors

        #region Initializations
        private void Initialize()
        {
            InitializeMenuItems();

            InitializeToolbarItems();
            
            imageViewer.Initialize(new OCRRenderingData(), new OCRAnalysisRender(imageViewer));
        }

        private void InitializeMenuItems()
        {
            foreach (ToolStripItem item in mainMenu.Items)
            {
                if (item is ToolStripMenuItem)
                {
                    RegisterMenuItemClickEvent(item as ToolStripMenuItem);
                }
            }

            detectBlocksToolStripMenuItem.Visible = false;
        }

        private void InitializeToolbarItems()
        {
            toolStripButton1.Click += new EventHandler(toolbarButtonClicked);
            toolStripButton2.Click += new EventHandler(toolbarButtonClicked);
            toolStripButton3.Click += new EventHandler(toolbarButtonClicked);


            toolStripComboBoxLanguage.SelectedIndex = 0;
            toolStripComboBoxLanguage.Enabled = false;

        }        

        private void RegisterMenuItemClickEvent(ToolStripMenuItem item)
        {
            if (item.DropDownItems == null || item.DropDownItems.Count == 0)
            {
                item.Click += new EventHandler(mainToolStripMenuItem_Click);
                return;
            }

            foreach (ToolStripItem subItem in item.DropDownItems)
            {
                if (subItem is ToolStripMenuItem)
                    RegisterMenuItemClickEvent(subItem as ToolStripMenuItem);
            }
        }
        #endregion Initializations

        #region Override methods
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.LoadGeneralSettings();

            _ocrProcessor = new TesseractProcessor();
            _ocrProcessor.DoMonitor = true;

            bool status = _ocrProcessor.Init(_tessData, _lang, _ocrEngineMode);
            Console.WriteLine(string.Format("[DEBUG] Init status: {0}", status));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            this.SaveGeneralSettings();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            this.CorrectImageViewerLocation(pnImageViewer);
        }
        #endregion Override methods

        #region Events
        private void mainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string mnItemName = (sender as ToolStripItem).Name.Trim().ToLower();
                // File
                if (mnItemName == openToolStripMenuItem.Name.Trim().ToLower())
                {
                    DoCommandOpen();
                }
                else if (mnItemName == oCRToolStripMenuItem.Name.Trim().ToLower())
                {
                    DoCommandOCR();
                }
                else if (mnItemName == optionsToolStripMenuItem.Name.Trim().ToLower())
                {
                    DoCommandShowOptions();
                }
                else if (mnItemName == detectBlocksToolStripMenuItem.Name.Trim().ToLower())
                {
                    DoCommandDetectBlocks();
                }
            }
            catch (System.Exception exp)
            {
                Console.WriteLine(exp.Message);
                Console.WriteLine(exp.StackTrace);

                string str = string.Format(
                    "Failed to process, due to:\nMessage: {0}\nStackTrace: {1}",
                    exp.Message, exp.StackTrace);

                MessageBox.Show(this, str, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
            }
        }

        void toolbarButtonClicked(object sender, EventArgs e)
        {
            if (imageViewer.RenderingData != null &&
                    imageViewer.RenderingData is OCRRenderingData)
            {
                (imageViewer.RenderingData as OCRRenderingData).UpdateFlags(
                    toolStripButton1.Checked, toolStripButton2.Checked, toolStripButton3.Checked);
                imageViewer.Invalidate(true);
            }
        }
        #endregion Events

        #region Commands

        #region File Commands
        private void DoCommandOpen()
        {
            OpenFile();
        }

        private void OpenFile()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Choose an image file to open";
                dlg.RestoreDirectory = true;
                dlg.Filter = Common.DefaultExtFilters;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    OpenFile(dlg.FileName);
                }
            }
        }

        private void OpenFile(string fileName)
        {
            ClearRenderingData();

            IImage rgbImage = new RGBImage();

            string sCommand = SupportedImageActions.Load;
            object[] inputs = new object[] { fileName };
            object[] outputs = null;

            rgbImage.DoCommand(sCommand, inputs, ref outputs);
            
            Image image = ToImage(rgbImage);

            // update currnt grey image
            FileName = fileName;
            _image = rgbImage;

            // update image for image viewer
            UpdateImageViewer(image);

            List<Word> wordList = null;
            this.UpdateImageViewer(wordList);
        }
        #endregion File Commands


        #region Analysis Commands
        private void DoCommandOCR()
        {
            if (_image == null)
                return;

            this.DoOCR(_image);
        }

        private void DoOCR(IImage image)
        {
            ClearRenderingData();
            
            string variable = "tessedit_pageseg_mode";
            int storedOSD = 0;
            _ocrProcessor.GetIntVariable(variable, ref storedOSD);
            try
            {
                // Fully automatic page segmentation
                int fully_psm_auto = 3;
                _ocrProcessor.SetVariable(variable, fully_psm_auto.ToString());

                



                ///// DEMO ONLY
                using (Image bitmap = ToImage(image))
                {
                    ///// DEMO ONLY
                    _ocrProcessor.Clear();
                    _ocrProcessor.ClearAdaptiveClassifier();
                    BlockList blocks = _ocrProcessor.DetectBlocks(bitmap);
                    this.UpdateImageViewer(blocks);


                    ///// DEMO ONLY
                    _ocrProcessor.Clear();
                    _ocrProcessor.ClearAdaptiveClassifier();
                    string result = _ocrProcessor.Apply(bitmap);
                    List<Word> detectedWords = _ocrProcessor.RetriveResultDetail();
                    this.UpdateImageViewer(detectedWords);

                    if (!string.IsNullOrEmpty(result))
                        MessageBox.Show(result);
                }
            }
            catch
            {
            }
            finally
            {
                _ocrProcessor.SetVariable(variable, storedOSD.ToString());
            }


            //_ocrProcessor.Clear();
            //_ocrProcessor.ClearAdaptiveClassifier();

            //using (Image bitmap = ToImage(image))
            //{
            //    //string result = _ocrProcessor.Apply(_fileName);
            //    string result = _ocrProcessor.Apply(bitmap);

            //    List<Word> detectedWords = _ocrProcessor.RetriveResultDetail();
            //    this.UpdateImageViewer(detectedWords);

            //    if (!string.IsNullOrEmpty(result))
            //        MessageBox.Show(result);
            //}
        }

        private void DoCommandDetectBlocks()
        {
            if (_image == null)
                return;

            ClearRenderingData();

            DetectBlocks(_image);
        }

        private void DetectBlocks(IImage image)
        {
            string variable = "tessedit_pageseg_mode";
            int storedOSD = 0;
            _ocrProcessor.GetIntVariable(variable, ref storedOSD);
            try
            {
                // Fully automatic page segmentation
                int fully_psm_auto = 3;
                _ocrProcessor.SetVariable(variable, fully_psm_auto.ToString());

                _ocrProcessor.Clear();
                _ocrProcessor.ClearAdaptiveClassifier();

                using (Image bitmap = ToImage(image))
                {
                    BlockList blocks = _ocrProcessor.DetectBlocks(bitmap);
                    this.UpdateImageViewer(blocks);
                }
            }
            catch
            {
            }
            finally
            {
                _ocrProcessor.SetVariable(variable, storedOSD.ToString());
            }
        }
        #endregion Analysis Commands

        private void DoCommandShowOptions()
        {
            DlgOptions dlg = new DlgOptions();
            dlg.DataPath = _tessData;
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _tessData = dlg.DataPath;
                _lang = dlg.Language;
                _ocrEngineMode = (int)dlg.OcrEngineMode;

                bool status = _ocrProcessor.Init(_tessData, _lang, _ocrEngineMode);
                Console.WriteLine(string.Format("[DEBUG] Init status: {0}", status));

                string msg = string.Format("{0} to initialize Tesseract Engine {1}.",
                    (status ? "Succeed" : "Failed"), _ocrProcessor.GetTesseractEngineVersion());
                MessageBox.Show(msg);
            }
        }
        #endregion Commands

        #region Methods
        private Image ToImage(IImage image)
        {
            string sCommand = SupportedImageActions.ToImage;
            object[] inputs = null;
            Image bitmap = null;
            object[] outputs = new object[] { bitmap };

            image.DoCommand(sCommand, inputs, ref outputs);

            return (outputs[0] as Image);
        }
        #endregion Methods

        #region Helpers
        private void UpdateImageViewer(Image image)
        {
            imageViewer.Image = image;
            imageViewer.Visible = true;
            CorrectImageViewerLocation(pnImageViewer as Control);
        }

        private void UpdateImageViewer(List<Word> detectedWords)
        {
            if (imageViewer.RenderingData != null &&
                    imageViewer.RenderingData is OCRRenderingData)
            {
                //(imageViewer.RenderingData as OCRRenderingData).ShowDetectedCharacters = false;
                (imageViewer.RenderingData as OCRRenderingData).WordList = detectedWords;
                imageViewer.Invalidate(true);
            }
        }

        private void UpdateImageViewer(BlockList blocks)
        {
            if (imageViewer.RenderingData != null &&
                    imageViewer.RenderingData is OCRRenderingData)
            {
                (imageViewer.RenderingData as OCRRenderingData).Blocks = blocks;
                imageViewer.Invalidate(true);
            }
        }

        private void CorrectImageViewerLocation(Control ctrl)
        {
            if (!imageViewer.Visible) return;

            int x = 0;
            int y = 0;

            if (imageViewer.Width < ctrl.Width)
                x = (ctrl.Width - imageViewer.Width) / 2;
            if (imageViewer.Height < ctrl.Height)
                y = (ctrl.Height - imageViewer.Height) / 2;

            if (imageViewer.Left != x || imageViewer.Top != y)
            {
                imageViewer.Location = new Point(x, y);
                ctrl.Invalidate(true);
            }
        }

        private void ClearRenderingData()
        {
            if (imageViewer.RenderingData != null &&
                    imageViewer.RenderingData is OCRRenderingData)
            {
                (imageViewer.RenderingData as OCRRenderingData).ClearData();
                imageViewer.Invalidate(true);
            }
        }

        private string GetDefaultSettingFolder()
        {
            string settingFolder = string.Format("{0}Settings", Application.StartupPath);

            try
            {

                if (!Directory.Exists(settingFolder))
                    Directory.CreateDirectory(settingFolder);
            }
            catch
            {
                settingFolder = string.Empty;
            }

            return settingFolder;
        }

        private void LoadGeneralSettings()
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, FileName_GeneralSettings);

                using (FileStream fs = new FileStream(
                    filePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        // load version
                        string version = reader.ReadString();

                        //// load general settings
                        _tessData = reader.ReadString();
                        _lang = reader.ReadString();
                        _ocrEngineMode = reader.ReadInt32();
                    }
                }
            }
            catch
            {
                // nothing
            }
        }

        private const string _version = "1.0";
        private void SaveGeneralSettings()
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, FileName_GeneralSettings);

                using (FileStream fs = new FileStream(
                    filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (BinaryWriter writer = new BinaryWriter(fs))
                    {
                        // save version
                        writer.Write(_version);

                        //// save general settings
                        writer.Write(_tessData);
                        writer.Write(_lang);
                        writer.Write(_ocrEngineMode);
                    }
                }
            }
            catch
            {
                // nothing
            }
        }
        #endregion Helpers
    }
}
