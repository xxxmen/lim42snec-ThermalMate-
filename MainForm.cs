using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ThermalMate
{
    public partial class MainForm : Form
    {
        private readonly XmlHelper _xmlHelper;

        public MainForm()
        {
            InitializeComponent();

            Utils.ReleaseResource("ThermalMate.Resource.ThermalMate.xml", "ThermalMate.xml");
            Utils.ReleaseResource("ThermalMate.Resource.UEwasp.dll", "UEwasp.dll");

            _xmlHelper = new XmlHelper(@"ThermalMate.xml");

            LoadConfig();
        }

        private void LoadConfig()
        {
            chkTopMost.Checked = TopMost = Convert.ToBoolean(_xmlHelper.GetOnlyInnerText("//Config/TopMost"));

            var mediums = _xmlHelper.GetElementNames("//Velocity/*").ToList();
            var velocities = _xmlHelper.GetInnerTexts("//Velocity/*").ToList();
            for (var i = 0; i < mediums.Count; i++)
            {
                lstVelocity.Items.Add(new ListViewItem(new[] { mediums[i], velocities[i] }));
            }

            // 读取项目名
            cbxProject.DataSource = _xmlHelper.GetAttributeValues("//Project/@Name").ToList();
            cbxProject.Text = _xmlHelper.GetOnlyInnerText("//Config/Project");
            cbxSpecification.Text = _xmlHelper.GetOnlyInnerText("//Config/Specification");
            cbxStandardName.Text = _xmlHelper.GetOnlyInnerText("//Config/StandardName");
        }

        #region 界面
        private static void ResetControls(Control container)
        {
            foreach (Control c in container.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Clear();
                    ((TextBox)c).BackColor = SystemColors.Window;
                }
                else if (c is ComboBox)
                {
                    ((ComboBox)c).SelectedIndex = -1;
                }
                ResetControls(c);
            }
        }

        private void passDiameter_DoubleClick(object sender, EventArgs e)
        {
            var txt = (TextBox)sender;
            if (null == txt) return;
            txtInnerDiameter.Clear();
            txtFlowVelocity.Clear();
            rioDiameter.Checked = true;
            txtInnerDiameter.Text = txt.Text;
            tabMain.SelectedIndex = 0;
        }

        private void passFlowRate_DoubleClick(object sender, EventArgs e)
        {
            var txt = (TextBox)sender;
            if (null == txt) return;
            txtFlowRate.Text = txt.Text;
            tabMain.SelectedIndex = 0;
            txtInnerDiameter.Clear();
            txtFlowVelocity.Clear();
            if (rioDiameter.Checked)
            {
                txtInnerDiameter.Focus();
            }
            else if (rioVelocity.Checked)
            {
                txtFlowVelocity.Focus();
            }
        }

        private void rioPipeDiameter_CheckedChanged(object sender, EventArgs e)
        {
            if (rioVelocity.Checked)
            {
                txtFlowVelocity.ReadOnly = false;
                txtInnerDiameter.ReadOnly = true;
                txtFlowVelocity.Clear();
                txtInnerDiameter.Clear();
                txtFlowVelocity.Focus();
            }
            else if (rioDiameter.Checked)
            {
                txtFlowVelocity.ReadOnly = true;
                txtInnerDiameter.ReadOnly = false;
                txtFlowVelocity.Clear();
                txtInnerDiameter.Clear();
                txtInnerDiameter.Focus();
            }
        }

        private void cbxGroundMass_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 重置控件背景色
            txtMoisture1.BackColor = SystemColors.Window;
            txtMoisture2.BackColor = SystemColors.Window;
            txtAsh1.BackColor = SystemColors.Window;
            txtAsh2.BackColor = SystemColors.Window;
            txtVolatile1.BackColor = SystemColors.Window;
            txtVolatile2.BackColor = SystemColors.Window;
            txtCarbon1.BackColor = SystemColors.Window;
            txtCarbon2.BackColor = SystemColors.Window;

            var groundmassA = cbxGroundMass1.Text;
            var groundmassB = cbxGroundMass2.Text;
            if (@"收到基(ar)" == groundmassA && @"空气干燥基(ad)" == groundmassB)
            {// 收到基->
                txtMoisture1.BackColor = Color.Pink;
                txtMoisture2.BackColor = Color.Pink;
            }
            else if (@"收到基(ar)" == groundmassA && @"干燥基(d)" == groundmassB)
            {
                txtMoisture1.BackColor = Color.Pink;
            }
            else if (@"收到基(ar)" == groundmassA && @"干燥无灰基(daf)" == groundmassB)
            {
                txtMoisture1.BackColor = Color.Pink;
                txtAsh1.BackColor = Color.Pink;
            }
            else if (@"空气干燥基(ad)" == groundmassA && @"收到基(ar)" == groundmassB)
            {// 空气干燥基->
                txtMoisture1.BackColor = Color.Pink;
                txtMoisture2.BackColor = Color.Pink;
            }
            else if (@"空气干燥基(ad)" == groundmassA && @"干燥基(d)" == groundmassB)
            {
                txtMoisture1.BackColor = Color.Pink;
            }
            else if (@"空气干燥基(ad)" == groundmassA && @"干燥无灰基(daf)" == groundmassB)
            {
                txtMoisture1.BackColor = Color.Pink;
                txtAsh1.BackColor = Color.Pink;
            }
            else if (@"干燥基(d)" == groundmassA && @"收到基(ar)" == groundmassB)
            {// 干燥基->
                txtMoisture2.BackColor = Color.Pink;
            }
            else if (@"干燥基(d)" == groundmassA && @"空气干燥基(ad)" == groundmassB)
            {
                txtMoisture2.BackColor = Color.Pink;
            }
            else if (@"干燥基(d)" == groundmassA && @"干燥无灰基(daf)" == groundmassB)
            {
                txtAsh1.BackColor = Color.Pink;
            }
            else if (@"干燥无灰基(daf)" == groundmassA && @"收到基(ar)" == groundmassB)
            {// 干燥无灰基->
                txtMoisture2.BackColor = Color.Pink;
                txtAsh2.BackColor = Color.Pink;
            }
            else if (@"干燥无灰基(daf)" == groundmassA && @"空气干燥基(ad)" == groundmassB)
            {
                txtMoisture2.BackColor = Color.Pink;
                txtAsh2.BackColor = Color.Pink;
            }
            else if (@"干燥无灰基(daf)" == groundmassA && @"干燥基(d)" == groundmassB)
            {
                txtAsh2.BackColor = Color.Pink;
            }
        }

        private void rioCondition_CheckedChanged(object sender, EventArgs e)
        {
            if (rioStandardCondition.Checked)
            {
                txtStandardFlow.ReadOnly = false;
                txtOperatingFlow.ReadOnly = true;
                txtStandardFlow.Clear();
                txtOperatingFlow.Clear();
                txtStandardFlow.Focus();
            }
            else if (rioOperatingCondition.Checked)
            {
                txtStandardFlow.ReadOnly = true;
                txtOperatingFlow.ReadOnly = false;
                txtStandardFlow.Clear();
                txtOperatingFlow.Clear();
                txtOperatingFlow.Focus();
            }
        }

        private void chkTopMost_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = chkTopMost.Checked;
        }

        private void btnBrowseCurrentDirectory_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Environment.CurrentDirectory);
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            Process.Start("notepad++", "ThermalMate.xml");
        }

        private void btnOpenCalculator_Click(object sender, EventArgs e)
        {
            Process.Start("calc");
        }

        private void cbxProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 读取项目包含的管道等级
            cbxSpecification.Items.Clear();
            cbxSpecification.ResetText();
            var xPath = string.Format("//Project[@Name='{0}']/*", cbxProject.Text);
            var specs = _xmlHelper.GetElementNames(xPath);
            specs.ToList().ForEach(x => cbxSpecification.Items.Add(x));
        }

        private void ForMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                switch (tabMain.SelectedTab.Text)
                {
                    case @"管径计算":
                        CalculatePipeDiameter(null, null);
                        CalculateDuctSection(null, null);
                        CalculateEquivalentInnerDiameter(null, null);
                        break;
                    case @"管道特性":
                        QueryPipeSpecification(null, null);
                        CalculatePipeCharacteristic(null, null);
                        break;
                    case @"汽水性质":
                        QuerySteamProperty(null, null);
                        break;
                    case @"煤炭相关":
                        ConvertCoalGroundMass(null, null);
                        DistinguishCoalRank(null, null);
                        break;
                    case @"杂项功能":
                        ConvertFlowRate(null, null);
                        QueryBoltHole(null, null);
                        break;
                }
            }
            else if (e.KeyCode == Keys.D && ModifierKeys == Keys.Alt)
            {
                ResetControls(tabMain.SelectedTab);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape && ActiveControl is TextBox)
            {
                ((TextBox)ActiveControl).Clear();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.C && ModifierKeys == Keys.Alt)
            {
                Process.Start("calc");
            }
        }
        #endregion

        #region 业务逻辑
        private void CalculatePipeDiameter(object sender, EventArgs e)
        {
            double flowRate, innerDiameter, flowVelocity;
            double.TryParse(txtFlowRate.Text, out flowRate);
            double.TryParse(txtInnerDiameter.Text, out innerDiameter);
            double.TryParse(txtFlowVelocity.Text, out flowVelocity);

            if (rioDiameter.Checked)
            {
                flowVelocity = flowRate / 3600 / (0.785 * innerDiameter * innerDiameter / 1000000);
                txtFlowVelocity.Text = Math.Round(flowVelocity, 1) + string.Empty;
            }
            else if (rioVelocity.Checked)
            {
                innerDiameter = 18.81 * Math.Sqrt(flowRate / flowVelocity);
                txtInnerDiameter.Text = Math.Round(innerDiameter, 1) + string.Empty;
            }

        }

        private void CalculateEquivalentInnerDiameter(object sender, EventArgs e)
        {
            double innerDiameter1, innerDiameter2, innerDiameter3, equivalentInnerDiameter;
            double.TryParse(txtInnerDiameter1.Text, out innerDiameter1);
            double.TryParse(txtInnerDiameter2.Text, out innerDiameter2);
            double.TryParse(txtInnerDiameter3.Text, out innerDiameter3);
            double.TryParse(txtEquivalentInnerDiameter.Text, out equivalentInnerDiameter);

            equivalentInnerDiameter =
                Math.Sqrt(innerDiameter1 * innerDiameter1 + innerDiameter2 * innerDiameter2 + innerDiameter3 * innerDiameter3);
            equivalentInnerDiameter = Math.Round(equivalentInnerDiameter, 1);
            txtEquivalentInnerDiameter.Text = equivalentInnerDiameter + string.Empty;
        }

        private void CalculateDuctSection(object sender, EventArgs e)
        {
            double gasAmount, sectionHeight, sectionWidth;
            double.TryParse(txtGasAmount.Text, out gasAmount);
            double.TryParse(txtSectionHeight.Text, out sectionHeight);
            double.TryParse(txtSectionWidth.Text, out sectionWidth);

            var sectionArea = sectionHeight * sectionWidth;
            var gasVelocity = gasAmount / 3600 / sectionArea;
            txtGasVelocity.Text = Math.Round(gasVelocity, 1) + string.Empty;
        }

        private void CalculatePipeCharacteristic(object sender, EventArgs e)
        {
            if (txtOutDiameter.Text == string.Empty)
            {
                return;
            }

            
            double outDiameter, insulationThickness, pipeThickness, 
                insulationDensity, materialDensity,designTemperature;
            double.TryParse(txtInsulationThickness.Text, out insulationThickness);
            double.TryParse(txtPipeThickness.Text, out pipeThickness);
            double.TryParse(txtOutDiameter.Text, out outDiameter);
            double.TryParse(txtInsulationDensity.Text, out insulationDensity);
            double.TryParse(txtMaterialDensity.Text, out materialDensity);
            double.TryParse(txtDesignTemperature.Text, out designTemperature);
            // 尺寸均已米为单位
            outDiameter /= 1000; insulationThickness /= 1000; pipeThickness /= 1000;

            var jackerArea = 3.14 * (outDiameter + insulationThickness * 2);
            var paintArea = 3.14 * outDiameter;
            var insulationVolume = 3.14 / 4 * (Math.Pow(outDiameter + insulationThickness * 2, 2) - Math.Pow(outDiameter, 2));
            var factor = 0.02466;
            if (cbxPipeMaterial.Text.Contains("304") || cbxPipeMaterial.Text.Contains("316"))
            {
                factor *= 1.015;
            }
            var pipeWeight = factor * 1000000 * pipeThickness * (outDiameter - pipeThickness);
            var materialWeight = materialDensity * 3.14 / 4 * Math.Pow(outDiameter - pipeThickness * 2, 2);
            var waterWeight = 1000 * 3.14 / 4 * Math.Pow(outDiameter - pipeThickness * 2, 2);
            var insulationWeight = insulationVolume * insulationDensity;
            var testingLoad = pipeWeight + waterWeight + insulationWeight;
            var operatingLoad = pipeWeight + insulationWeight + materialWeight;
            var innerDiameter = outDiameter - 2*pipeThickness;
           
            // 截面惯性矩
            var moment = 3.14 / 64 * (Math.Pow(outDiameter*1000, 4) - Math.Pow(innerDiameter*1000, 4));
            // 弹性模量
            double modulus = 0;
            if (cbxPipeMaterial.Text.Contains("20"))
            {
                modulus = Utils.LineInterpolationModulus(Utils.Modulus20, designTemperature) * 1000;
            }
            else if (cbxPipeMaterial.Text.Contains("12Cr"))
            {
                modulus = Utils.LineInterpolationModulus(Utils.Modulus12Cr, designTemperature) * 1000;
            }
            else if (cbxPipeMaterial.Text.Contains("15Cr"))
            {
                modulus = Utils.LineInterpolationModulus(Utils.Modulus15Cr, designTemperature) * 1000;
            }
            else if (cbxPipeMaterial.Text.Contains("235"))
            {
                modulus = Utils.LineInterpolationModulus(Utils.Modulus235, designTemperature) * 1000;
            }
            else if (cbxPipeMaterial.Text.Contains("06Cr"))
            {
                modulus = Utils.LineInterpolationModulus(Utils.Modulus06Cr, designTemperature) * 1000;
            }

            // 管道单位荷载
            var averageLoad = testingLoad*9.8;
            if (!chkWaterTest.Checked)
            {
                averageLoad = operatingLoad*9.8;
            }

            // 计算跨度
            factor = 0.039;
            if (rioOutSite.Checked)
            {
                factor = 0.048; // 装置外管道
            }
            else if (cbxPipeMaterial.Text.Contains("12Cr") || cbxPipeMaterial.Text.Contains("15Cr"))
            {
                factor = 0.02093;   // 电力管道执行DLT5054
            }

            var span = factor * Math.Pow(moment * modulus / averageLoad, 0.25);

            txtHorizontalSpan.Text = Math.Round(span, 1) + string.Empty;
            txtJacketArea.Text = Math.Round(jackerArea, 3) + string.Empty;
            txtPaintArea.Text = Math.Round(paintArea, 3) + string.Empty;
            txtInsulationVolume.Text = Math.Round(insulationVolume, 3) + string.Empty;
            txtTestingLoad.Text = Math.Round(testingLoad, 3) + string.Empty;
            txtOperatingLoad.Text = Math.Round(operatingLoad, 3) + string.Empty;

            if (txtDesignTemperature.Text == string.Empty || modulus < 0)
            {
                txtHorizontalSpan.Clear();
                txtHorizontalSpan.Clear();
            }

            if (materialDensity <= 1)
            {
                txtOperatingLoad.Clear();
            }
        }

        private void QueryPipeSpecification(object sender, EventArgs e)
        {
            txtDo.Clear();
            txtSCH.Clear();
            txtThk.Clear();
            txtDi.Clear();
            txtMat.Clear();

            var dn = cmbNominalDiameter.Text.Replace("(", "").Replace(")", "");
            var standardName = cbxStandardName.Text;

            try
            {
                // 获取外径
                var xPath = string.Format("//Standard[@Name='{0}']/Pipe[@DN='{1}']/@DO", standardName, dn);
                var Do = _xmlHelper.GetOnlyAttributeValue(xPath);
                var dDo = double.Parse(Do);
                txtDo.Text = Do;

                // 获取壁厚
                xPath = string.Format("//Project[@Name='{0}']/{1}/*[text()='{2}']/@THK",
                    cbxProject.Text,
                    cbxSpecification.Text,
                    dn);
                var thk = _xmlHelper.GetOnlyAttributeValue(xPath);
                if (string.Empty == thk)
                {
                    MessageBox.Show(@"当前等级下不存在此管径", @"警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                txtSCH.Text = thk;

                // 材质
                xPath = string.Format("//Project[@Name='{0}']/{1}/*[text()='{2}']/@MATERIAL",
                    cbxProject.Text,
                    cbxSpecification.Text,
                    dn);
                txtMat.Text = _xmlHelper.GetOnlyAttributeValue(xPath);

                if (thk.Contains("SCH"))
                {
                    var sch = thk.Replace("SCH", string.Empty);

                    // 壁厚
                    xPath = string.Format("//Standard[@Name='{0}']/Pipe[@DN='{1}']/*[text()='{2}']/@THK", standardName, dn, sch);
                    thk = _xmlHelper.GetOnlyAttributeValue(xPath);

                    // 单重
                    //xPath = string.Format("//Standard[@Name='{0}']/Pipe[@DN='{1}']/*[text()='{2}']/@PW", standardName, dn, sch);
                    //var pw = _xmlHelper.GetOnlyAttributeValue(xPath);
                }
                var dThk = double.Parse(thk);
                txtThk.Text = thk;

                // 计算内径
                if ("SHT3405-2012" == cbxStandardName.Text.Trim())
                {
                    var nDi = Convert.ToInt32(dDo - 2 * dThk);
                    txtDi.Text = nDi + "";
                }
                else
                {
                    var dDi = Convert.ToDouble(dDo - 2 * dThk);
                    txtDi.Text = dDi + "";
                }

                Clipboard.Clear();
                Clipboard.SetData(DataFormats.Text, Do + "×" + thk);
            }
            catch (Exception ex)
            {
                // 清空编辑框
                foreach (var ctl in groupBox2.Controls.OfType<TextBox>())
                {
                    (ctl).Clear();
                }
                //MessageBox.Show("查询失败", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.Message);
            }

        }

        private void QuerySteamProperty(object sender, EventArgs e)
        {
            // 清空编辑框
            txtDensity1.Clear();
            txtEnthalpy1.Clear();
            txtViscosity1.Clear();
            txtIsoIndex1.Clear();
            txtVolumeFlow1.Clear();
            txtTotalEnthalpy1.Clear();
            txtDensity2.Clear();
            txtEnthalpy2.Clear();
            txtViscosity2.Clear();
            txtIsoIndex2.Clear();
            txtVolumeFlow2.Clear();
            txtTotalEnthalpy2.Clear();
            txtDensity3.Clear();
            txtEnthalpy3.Clear();
            txtViscosity3.Clear();
            txtIsoIndex3.Clear();
            txtVolumeFlow3.Clear();
            txtTotalEnthalpy3.Clear();

            double massFlow, pressure, temperature, retValue = 0.0;
            var range = 0;
            double.TryParse(txtMassFlow.Text, out massFlow);
            double.TryParse(txtPressure.Text, out pressure);
            double.TryParse(txtTemperature.Text, out temperature);

            // 使用IFC97标准
            Steam.SETSTD_WASP(97);

            if (string.Empty != txtPressure.Text && string.Empty != txtTemperature.Text &&
                !txtPressure.Text.Contains("（") && !txtTemperature.Text.Contains("（"))
            {// 过热汽
                Steam.PT2V(pressure, temperature, ref retValue, ref range);
                var volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow1.Text = Math.Round(volumeFlow, 1) + string.Empty;
                txtDensity1.Text = Math.Round(1 / retValue, 3) + string.Empty;

                Steam.PT2H(pressure, temperature, ref retValue, ref range);
                txtEnthalpy1.Text = Math.Round(retValue, 2) + string.Empty;
                var totalEnthalpy1 = Math.Round(retValue, 2) * massFlow * 1000;
                txtTotalEnthalpy1.Text = totalEnthalpy1 + string.Empty;

                Steam.PT2ETA(pressure, temperature, ref retValue, ref range);
                txtViscosity1.Text = Math.Round(retValue * 1000, 3) + string.Empty;

                Steam.PT2KS(pressure, temperature, ref retValue, ref range);
                txtIsoIndex1.Text = Math.Round(retValue, 3) + string.Empty;

            }
            else if (string.Empty != txtPressure.Text && string.Empty == txtTemperature.Text)
            {
                // 沸点
                Steam.P2T(pressure, ref retValue, ref range);
                txtTemperature.Text = string.Format("（{0}）", Math.Round(retValue, 1));

                // 饱和汽
                Steam.P2VG(pressure, ref retValue, ref range);
                var volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow2.Text = Math.Round(volumeFlow, 1) + string.Empty;
                txtDensity2.Text = Math.Round(1 / retValue, 3) + string.Empty;

                Steam.P2HG(pressure, ref retValue, ref range);
                txtEnthalpy2.Text = Math.Round(retValue, 2) + string.Empty;
                var totalEnthalpy2 = Math.Round(retValue, 2) * massFlow * 1000;
                txtTotalEnthalpy2.Text = totalEnthalpy2 + string.Empty;

                Steam.P2ETAG(pressure, ref retValue, ref range);
                txtViscosity2.Text = Math.Round(retValue * 1000, 3) + string.Empty;

                Steam.P2KSG(pressure, ref retValue, ref range);
                txtIsoIndex2.Text = Math.Round(retValue, 3) + string.Empty;

                // 饱和水
                Steam.P2VL(pressure, ref retValue, ref range);
                volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow3.Text = Math.Round(volumeFlow, 1) + string.Empty;
                txtDensity3.Text = Math.Round(1 / retValue, 3) + string.Empty;

                Steam.P2HL(pressure, ref retValue, ref range);
                txtEnthalpy3.Text = Math.Round(retValue, 2) + string.Empty;
                var totalEnthalpy3 = Math.Round(retValue, 2) * massFlow * 1000;
                txtTotalEnthalpy3.Text = totalEnthalpy3 + string.Empty;

                Steam.P2ETAL(pressure, ref retValue, ref range);
                txtViscosity3.Text = Math.Round(retValue * 1000, 3) + string.Empty;

                Steam.P2KSL(pressure, ref retValue, ref range);
                txtIsoIndex3.Text = Math.Round(retValue, 3) + string.Empty;
            }
            else if (string.Empty == txtPressure.Text && string.Empty != txtTemperature.Text)
            {
                Steam.T2P(temperature, ref retValue, ref range);
                txtPressure.Text = string.Format("（{0}）", Math.Round(retValue, 3));

                // 饱和汽
                Steam.T2VG(temperature, ref retValue, ref range);
                var volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow2.Text = Math.Round(volumeFlow, 1) + string.Empty;
                txtDensity2.Text = Math.Round(1 / retValue, 3) + string.Empty;

                Steam.T2HG(temperature, ref retValue, ref range);
                txtEnthalpy2.Text = Math.Round(retValue, 2) + string.Empty;

                Steam.T2ETAG(temperature, ref retValue, ref range);
                txtViscosity2.Text = Math.Round(retValue * 1000, 3) + string.Empty;

                Steam.T2KSG(temperature, ref retValue, ref range);
                txtIsoIndex2.Text = Math.Round(retValue, 3) + string.Empty;

                // 饱和水
                Steam.T2VL(temperature, ref retValue, ref range);
                volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow3.Text = Math.Round(volumeFlow, 1) + string.Empty;
                txtDensity3.Text = Math.Round(1 / retValue, 3) + string.Empty;

                Steam.T2HL(temperature, ref retValue, ref range);
                txtEnthalpy3.Text = Math.Round(retValue, 2) + string.Empty;

                Steam.T2ETAL(temperature, ref retValue, ref range);
                txtViscosity3.Text = Math.Round(retValue * 1000, 3) + string.Empty;

                Steam.T2KSL(temperature, ref retValue, ref range);
                txtIsoIndex3.Text = Math.Round(retValue, 3) + string.Empty;
            }
        }

        private void ConvertCoalGroundMass(object sender, EventArgs e)
        {
            double moistureA, ashA, volatileA, carbonA;
            double.TryParse(txtMoisture1.Text, out moistureA);
            double.TryParse(txtAsh1.Text, out ashA);
            double.TryParse(txtVolatile1.Text, out volatileA);
            double.TryParse(txtCarbon1.Text, out carbonA);

            double moistureB, ashB, volatileB, carbonB;
            double.TryParse(txtMoisture2.Text, out moistureB);
            double.TryParse(txtAsh2.Text, out ashB);
            double.TryParse(txtVolatile2.Text, out volatileB);
            double.TryParse(txtCarbon2.Text, out carbonB);

            var groundmassA = cbxGroundMass1.Text;
            var groundmassB = cbxGroundMass2.Text;

            double ratio;
            if (@"收到基(ar)" == groundmassA && @"空气干燥基(ad)" == groundmassB)
            {// 收到基->
                ratio = (100 - moistureB) / (100 - moistureA);
                ashB = ashA * ratio;
                volatileB = volatileA * ratio;
                carbonB = carbonA * ratio;
            }
            else if (@"收到基(ar)" == groundmassA && @"干燥基(d)" == groundmassB)
            {
                ratio = 100 / (100 - moistureA);
                moistureB = 0;
                ashB = ashA * ratio;
                volatileB = volatileA * ratio;
                carbonB = carbonA * ratio;
            }
            else if (@"收到基(ar)" == groundmassA && @"干燥无灰基(daf)" == groundmassB)
            {
                ratio = 100 / (100 - moistureA - ashA);
                moistureB = 0;
                ashB = 0;
                volatileB = volatileA * ratio;
                carbonB = carbonA * ratio;
            }
            else if (@"空气干燥基(ad)" == groundmassA && @"收到基(ar)" == groundmassB)
            {// 空气干燥基->
                ratio = (100 - moistureB) / (100 - moistureA);
                ashB = ashA * ratio;
                volatileB = volatileA * ratio;
                carbonB = carbonA * ratio;
            }
            else if (@"空气干燥基(ad)" == groundmassA && @"干燥基(d)" == groundmassB)
            {
                ratio = 100 / (100 - moistureA);
                moistureB = 0;
                ashB = ashA * ratio;
                volatileB = volatileA * ratio;
                carbonB = carbonA * ratio;
            }
            else if (@"空气干燥基(ad)" == groundmassA && @"干燥无灰基(daf)" == groundmassB)
            {
                ratio = 100 / (100 - moistureA - ashA);
                moistureB = 0;
                ashB = 0;
                volatileB = volatileA * ratio;
                carbonB = carbonA * ratio;

            }
            else if (@"干燥基(d)" == groundmassA && @"收到基(ar)" == groundmassB)
            {// 干燥基->
                ratio = (100 - moistureB) / 100;
                ashB = ashA * ratio;
                volatileB = volatileA * ratio;
                carbonB = carbonA * ratio;
            }
            else if (@"干燥基(d)" == groundmassA && @"空气干燥基(ad)" == groundmassB)
            {
                ratio = (100 - moistureB) / 100;
                ashB = ashA * ratio;
                volatileB = volatileA * ratio;
                carbonB = carbonA * ratio;
            }
            else if (@"干燥基(d)" == groundmassA && @"干燥无灰基(daf)" == groundmassB)
            {
                ratio = 100 / (100 - ashA);
                moistureB = 0;
                ashB = 0;
                volatileB = volatileA * ratio;
                carbonB = carbonA * ratio;

            }
            else if (@"干燥无灰基(daf)" == groundmassA && @"收到基(ar)" == groundmassB)
            {// 干燥无灰基->
                ratio = (100 - moistureB - ashB) / 100;
                volatileB = volatileA * ratio;
                carbonB = carbonA * ratio;
            }
            else if (@"干燥无灰基(daf)" == groundmassA && @"空气干燥基(ad)" == groundmassB)
            {
                ratio = (100 - moistureB - ashB) / 100;
                volatileB = volatileA * ratio;
                carbonB = carbonA * ratio;
            }
            else if (@"干燥无灰基(daf)" == groundmassA && @"干燥基(d)" == groundmassB)
            {
                ratio = (100 - ashB) / 100;
                moistureB = 0;
                volatileB = volatileA * ratio;
                carbonB = carbonA * ratio;
            }

            txtMoisture2.Text = moistureB.ToString("F2");
            txtAsh2.Text = ashB.ToString("F2");
            txtVolatile2.Text = volatileB.ToString("F2");
            txtCarbon2.Text = carbonB.ToString("F2");


        }

        private void DistinguishCoalRank(object sender, EventArgs e)
        {

            double vdaf, pm;
            double.TryParse(txtVdaf.Text, out vdaf);
            double.TryParse(txtPm.Text, out pm);


            if (vdaf > 0 && vdaf <= 10)
            {
                txtCoalRank.Text = @"无烟煤";
            }
            else if (vdaf > 10 && vdaf <= 37)
            {
                txtCoalRank.Text = @"烟煤";
            }
            else if (vdaf > 37 && pm <= 50 && pm > 0)
            {
                txtCoalRank.Text = @"褐煤";
            }
            else if (vdaf > 37 && pm > 50)
            {
                txtCoalRank.Text = @"烟煤";
            }
            else
            {
                txtCoalRank.Clear();
            }
        }

        private void QueryBoltHole(object sender, EventArgs e)
        {
            txtHoleSize.Text = string.Empty;
            var xpath = string.Format("//BlotHole/{0}[@TYPE='{1}']", cbxBoltSpec.Text, cbxEquipmentType.Text);
            txtHoleSize.Text = _xmlHelper.GetOnlyInnerText(xpath);
        }

        private void ConvertFlowRate(object sender, EventArgs e)
        {
            double op, ot, of, sp, st, sf;
            double.TryParse(txtOperatingPressure.Text, out op);
            double.TryParse(txtOperatingTemperature.Text, out ot);
            double.TryParse(txtOperatingFlow.Text, out of);
            double.TryParse(txtStandardPressure.Text, out sp);
            double.TryParse(txtStandardTemperature.Text, out st);
            double.TryParse(txtStandardFlow.Text, out sf);

            if (rioStandardCondition.Checked)
            {
                var flow = ConvertFlowRate(sp, st, op, ot, sf);
                flow = Math.Round(flow, 1);
                txtOperatingFlow.Text = flow + string.Empty;
            }
            else if (rioOperatingCondition.Checked)
            {
                var flow = ConvertFlowRate(op, ot, sp, st, of);
                flow = Math.Round(flow, 1);
                txtStandardFlow.Text = flow + string.Empty;
            }
        }

        private static double ConvertFlowRate(double p1, double t1, double p2, double t2, double f1)
        {
            t1 += 273.15;
            t2 += 273.15;
            var f2 = f1 * (t2 / t1) * (p1 / p2);
            return f2;
        }
        #endregion
    }
}