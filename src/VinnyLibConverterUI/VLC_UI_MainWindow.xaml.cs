using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace VinnyLibConverterUI
{
    /// <summary>
    /// Interaction logic for VLC_UI_MainWindow.xaml
    /// </summary>
    public partial class VLC_UI_MainWindow : Window
    {
        public VLC_UI_MainWindow(bool isForImport)
        {
            pIsImport = isForImport;
            InitializeComponent();
            //init affine schema
            this.TextBox_AffineSchema.Text = "X' = αX + βY + λ;\nY' = γX + δY + μ";
            Utils.LoadVinnyLibConverterCommon();
            VinnyParametets = new VinnyLibConverterCommon.ImportExportParameters();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            if (pIsImport)
            {
                this.CheckBoxTryCombineSameMeshGeometries.IsEnabled = false;
            }
        }

        private void SetUiFromConfig(VinnyLibConverterCommon.ImportExportParameters VinnyParametets)
        {
            this.TextBoxPath.Text = VinnyParametets.Path;

            this.TextBoxToken.Text = VinnyParametets.Token;
            this.TextBoxLogin.Text = VinnyParametets.Login;
            this.TextBoxPassword.Text = VinnyParametets.Password;

            this.CheckBoxCheckGeometryDubles.IsChecked = VinnyParametets.CheckGeometryDubles;
            this.TextBoxGeometryDublesAccuracy.Text = VinnyParametets.VertexAccuracy.ToString();
            this.CheckBoxCheckMaterialsDubles.IsChecked = VinnyParametets.CheckMaterialsDubles;
            this.CheckBoxCheckParameterDefsDubles.IsChecked = VinnyParametets.CheckMaterialsDubles;
            this.CheckBoxReprojectOnlyPosition.IsChecked = VinnyParametets.ReprojectOnlyPosition;
            this.CheckBoxInverseXYCoordinates.IsChecked = VinnyParametets.InverseXYCoordinates;
            this.CheckBoxTryCombineSameMeshGeometries.IsChecked = VinnyParametets.TryCombineSameMeshGeometries;

            foreach (var tr in VinnyParametets.TransformationInfo)
            {
                this.ListBoxTransformationInfo.Items.Add(tr.ToString().Replace(Environment.NewLine, ""));
            }

            this.VinnyParametets = VinnyParametets;
        }

        private void SaveUiToConfig()
        {
            if (this.TextBoxPath.Text == "")
            {
                //throw new Exception("VinnyRenga. Путь к файлу не указан!");
            }
            this.VinnyParametets.Path = this.TextBoxPath.Text;

            this.VinnyParametets.Token = this.TextBoxToken.Text;
            this.VinnyParametets.Login = this.TextBoxLogin.Text;
            this.VinnyParametets.Password = this.TextBoxPassword.Text;

            this.VinnyParametets.CheckGeometryDubles = this.CheckBoxCheckGeometryDubles.IsChecked ?? false;
            this.VinnyParametets.VertexAccuracy = int.Parse(this.TextBoxGeometryDublesAccuracy.Text);
            this.VinnyParametets.CheckMaterialsDubles = this.CheckBoxCheckMaterialsDubles.IsChecked ?? false;
            this.VinnyParametets.CheckParameterDefsDubles = this.CheckBoxCheckParameterDefsDubles.IsChecked ?? false;
            this.VinnyParametets.ReprojectOnlyPosition = this.CheckBoxReprojectOnlyPosition.IsChecked ?? false;
            this.VinnyParametets.InverseXYCoordinates = this.CheckBoxInverseXYCoordinates.IsChecked ?? false;
            this.VinnyParametets.TryCombineSameMeshGeometries = this.CheckBoxTryCombineSameMeshGeometries.IsChecked ?? false;

            this.VinnyParametets.ModelType = VinnyLibConverterCommon.CommomUtils.GetCdeVariantFromExtension(System.IO.Path.GetExtension(this.VinnyParametets.Path));
        }

        private VinnyLibConverterCommon.Transformation.TransformationGeodetic CreateGeodeticFromData()
        {
            VinnyLibConverterCommon.Transformation.TransformationGeodetic geodeticTr =
                new VinnyLibConverterCommon.Transformation.TransformationGeodetic();

            geodeticTr.SourceCs = this.TextBoxGeodetic_StartCsDef.Text;
            geodeticTr.SourceCsType = VinnyLibConverterCommon.Transformation.CsDefVariant.NameOrCode;
            if (this.RadioButton_StartCS_WKT.IsChecked == true) geodeticTr.SourceCsType = VinnyLibConverterCommon.Transformation.CsDefVariant.WKT;

            geodeticTr.DestCs = this.TextBoxGeodetic_DestCsDef.Text;
            geodeticTr.DestCsType = VinnyLibConverterCommon.Transformation.CsDefVariant.NameOrCode;
            if (this.RadioButton_DestCS_WKT.IsChecked == true) geodeticTr.DestCsType = VinnyLibConverterCommon.Transformation.CsDefVariant.WKT;

            return geodeticTr;
        }

        private VinnyLibConverterCommon.Transformation.TransformationMatrix4x4 CreateMatrix4x4FromData()
        {
            VinnyLibConverterCommon.Transformation.TransformationMatrix4x4 matrix = VinnyLibConverterCommon.Transformation.TransformationMatrix4x4.CreateEmptyTransformationMatrix();
            //translation
            matrix.SetPosition(
                double.Parse(this.TextBox_TranslationX.Text),
                double.Parse(this.TextBox_TranslationY.Text),
                double.Parse(this.TextBox_TranslationZ.Text));
            //rotation
            double rotX = double.Parse(this.TextBox_RotAxisAnglesX.Text);
            double rotY = double.Parse(this.TextBox_RotAxisAnglesY.Text);
            double rotZ = double.Parse(this.TextBox_RotAxisAnglesZ.Text);

            double rotQx = double.Parse(this.TextBox_RotQuaternionX.Text);
            double rotQy = double.Parse(this.TextBox_RotQuaternionY.Text);
            double rotQz = double.Parse(this.TextBox_RotQuaternionZ.Text);
            double rotQw = double.Parse(this.TextBox_RotQuaternionW.Text);

            if (this.RadioButton_IsDegree.IsChecked == true)
            {
                rotX = VinnyLibConverterCommon.CommomUtils.DegreeToRadians(rotX);
                rotY = VinnyLibConverterCommon.CommomUtils.DegreeToRadians(rotY);
                rotZ = VinnyLibConverterCommon.CommomUtils.DegreeToRadians(rotZ);

                rotQx = VinnyLibConverterCommon.CommomUtils.DegreeToRadians(rotQx);
                rotQy = VinnyLibConverterCommon.CommomUtils.DegreeToRadians(rotQy);
                rotQz = VinnyLibConverterCommon.CommomUtils.DegreeToRadians(rotQz);
                rotQw = VinnyLibConverterCommon.CommomUtils.DegreeToRadians(rotQw);
            }

            if (this.RadioButton_AsDegrees.IsChecked == true)
            {
                matrix.SetRotationFromAngles(rotX, rotY, rotZ);
            }
            else if (this.RadioButton_AsQuaternion.IsChecked == true)
            {
                matrix.SetRotationFromQuaternion(new VinnyLibConverterCommon.Transformation.QuaternionInfo(rotQx, rotQy, rotQz, rotQw));
            }

            //scale
            matrix.SetScale(
                (double)double.Parse(this.TextBox_ScaleX.Text),
                (double)double.Parse(this.TextBox_ScaleY.Text),
                (double)double.Parse(this.TextBox_ScaleY.Text));

            return matrix;
        }

        #region Handlers
        private void ButtonSetPath_Click(object sender, RoutedEventArgs e)
        {
            var frmtsInfo = VinnyLibConverterCommon.CommomUtils.GetCurrentFormats();

            FileDialog fileDialog;
            if (this.pIsImport)
            {
                fileDialog = new OpenFileDialog() { Multiselect = false };
                fileDialog.Title = "Выборка файла для импорта";
            }
            else 
            {
                fileDialog = new SaveFileDialog();
                fileDialog.Title = "Сохранить проект";
            }
            

            List<string> filters = new List<string>();
            List<string> exts = new List<string>();

            foreach (VinnyLibConverterCommon.DataExchangeFormatInfo frmtInfo in frmtsInfo)
            {
                if (pIsImport && !frmtInfo.IsReadable) continue;
                if (!pIsImport && !frmtInfo.IsWritable) continue;

                var current_exts = frmtInfo.Extensions.Select(a => a = "*." + a)
                        .Concat(frmtInfo.Extensions.Select(a => a = "*." + a.ToUpper()));
                string extensions = string.Join(";", current_exts);
                exts = exts.Concat(current_exts).ToList();
                filters.Add($"{frmtInfo.Caption} ({extensions}) | {extensions} ");
            }

            exts = exts.Distinct().ToList();
            

            filters = filters.Distinct().ToList();
            filters.Sort();

            List<string> filters2 = new List<string>();
            if (pIsImport) filters2.Add($"Все файлы | {string.Join(";", exts)}");
            filters2 = filters2.Concat(filters).ToList();

            fileDialog.Filter = string.Join("|", filters2);

            if (fileDialog.ShowDialog() == true)
            {
                this.TextBoxPath.Text = fileDialog.FileName;
            }

        }

        private void ButtonDeleteTransformation_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxTransformationInfo.SelectedIndex <= 0) return;
            if (ListBoxTransformationInfo.SelectedIndex > this.VinnyParametets.TransformationInfo.Count) return;

            List<VinnyLibConverterCommon.Transformation.ICoordinatesTransformation> existedTr = new List<VinnyLibConverterCommon.Transformation.ICoordinatesTransformation>();
            this.VinnyParametets.TransformationInfo = new List<VinnyLibConverterCommon.Transformation.ICoordinatesTransformation>();

            for(int i = 0; i < existedTr.Count; i++)
            {
                if (i != ListBoxTransformationInfo.SelectedIndex) this.VinnyParametets.TransformationInfo.Add(existedTr[i]);
            }

            this.ListBoxTransformationInfo.Items.Clear();
            foreach (var tr in this.VinnyParametets.TransformationInfo)
            {
                this.ListBoxTransformationInfo.Items.Add(tr.ToString());
            }
        }

        private void ButtonSaveTransformation_Click(object sender, RoutedEventArgs e)
        {
            if (TabControlTransformationTypes.SelectedItem == null) return;
            if (TabControlTransformationTypes.SelectedItem as TabItem == null) return;

            string tabName = ((TabItem)TabControlTransformationTypes.SelectedItem).Header.ToString();
            if (tabName == "Matrix4x4")
            {
                var matrix = CreateMatrix4x4FromData();

                this.VinnyParametets.TransformationInfo.Add(matrix);
                this.ListBoxTransformationInfo.Items.Add(matrix.Matrix.ToString().Replace(Environment.NewLine, " "));
            }
            else if (tabName == "Affine")
            {
                VinnyLibConverterCommon.Transformation.TransformationAffine transformAffine =
                    new VinnyLibConverterCommon.Transformation.TransformationAffine(
                        double.Parse(this.TextBox_ScaleX.Text),
                        double.Parse(this.TextBox_ScaleY.Text),
                        double.Parse(this.TextBoxAffineT_RotationX.Text),
                        double.Parse(this.TextBoxAffineT_RotationY.Text),
                        double.Parse(this.TextBoxAffineT_TranslationX.Text),
                        double.Parse(this.TextBoxAffineT_TranslationY.Text)
                    );
                this.VinnyParametets.TransformationInfo.Add(transformAffine);
                this.ListBoxTransformationInfo.Items.Add(transformAffine.ToString());

            }
            else if (tabName == "Geodetic")
            {
                var geod = CreateGeodeticFromData();
                string sourceCS;
                string destCS;
                if (geod.InitData(out sourceCS, out destCS))
                {
                    this.VinnyParametets.TransformationInfo.Add(geod);
                    this.ListBoxTransformationInfo.Items.Add(geod.ToString());
                }
            }
        }

        private void ButtonLoadSettings_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выбор конфигурационного файла VinnyLibConverter";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Конфиграционный файл (*.XML, *.xml) | *.XML;*.xml";

            if (openFileDialog.ShowDialog() == true && File.Exists(openFileDialog.FileName))
            {
                this.SetUiFromConfig(VinnyLibConverterCommon.ImportExportParameters.LoadFromFile(openFileDialog.FileName));
            }
        }

        private void ButtonSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            SaveUiToConfig();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Сохранение файла конфигурации VinnyLibConverter";
            saveFileDialog.Filter = "Конфиграционный файл (*.XML, *.xml) | *.XML;*.xml";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                this.VinnyParametets.Save(saveFileDialog.FileName);
            }
        }

        private void ButtonShowMatrix4x4_Click(object sender, RoutedEventArgs e)
        {
            VinnyLibConverterCommon.Transformation.TransformationMatrix4x4 matrix = CreateMatrix4x4FromData();
            this.TextBoxResultMatrix.Text = matrix.Matrix.ToString();
        }

        private void ButtonCheckGeodeticData_Click(object sender, RoutedEventArgs e)
        {
            var geod = CreateGeodeticFromData();
            string sourceCS;
            string destCS;
            if (!geod.InitData(out sourceCS, out destCS))
            {
                this.TextBoxGeodetic_StartCsDef.Text = sourceCS;
                this.TextBoxGeodetic_DestCsDef.Text = destCS;

                this.VinnyParametets.TransformationInfo.Add(geod);
                this.ListBoxTransformationInfo.Items.Add(geod.ToString());
            }
            else
            {
                this.TextBoxGeodetic_StartCsDef.Text = sourceCS;
                this.TextBoxGeodetic_DestCsDef.Text = destCS;
            }
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.SaveUiToConfig();
            this.Close();
        }

        

        #endregion

        public VinnyLibConverterCommon.ImportExportParameters VinnyParametets {  get; set; }
        private bool pIsImport;
    }
}
