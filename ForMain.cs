using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ThermalMate.Class;

namespace ThermalMate
{
    public partial class ForMain : Form
    {
        private readonly XmlHelper _xmlHelper;

        public ForMain()
        {
            InitializeComponent();

            Common.ReleaseResource("ThermalMate.Resource.ThermalMate.xml", "ThermalMate.xml");
            Common.ReleaseResource("ThermalMate.Resource.UEwasp.dll", "UEwasp.dll");

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

            cbxSpecification.SelectedIndexChanged += cmbNominalDiameter_SelectedIndexChanged;
        }

        #region 界面
        private void resetControls(Control container)
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
                resetControls(c);
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
            txtMoisture1.BackColor = SystemColors.Control;
            txtMoisture2.BackColor = SystemColors.Control;
            txtAsh1.BackColor = SystemColors.Control;
            txtAsh2.BackColor = SystemColors.Control;
            txtVolatile1.BackColor = SystemColors.Control;
            txtVolatile2.BackColor = SystemColors.Control;
            txtCarbon1.BackColor = SystemColors.Control;
            txtCarbon2.BackColor = SystemColors.Control;

            var groundmassA = cbxGroundMass1.Text;
            var groundmassB = cbxGroundMass2.Text;
            if (@"收到基(ar)" == groundmassA && @"空气干燥基(ad)" == groundmassB)
            {// 收到基->
                txtMoisture1.BackColor = Color.Coral;
                txtMoisture2.BackColor = Color.Coral;
            }
            else if (@"收到基(ar)" == groundmassA && @"干燥基(d)" == groundmassB)
            {
                txtMoisture1.BackColor = Color.Coral;
            }
            else if (@"收到基(ar)" == groundmassA && @"干燥无灰基(daf)" == groundmassB)
            {
                txtMoisture1.BackColor = Color.Coral;
                txtAsh1.BackColor = Color.Coral;
            }
            else if (@"空气干燥基(ad)" == groundmassA && @"收到基(ar)" == groundmassB)
            {// 空气干燥基->
                txtMoisture1.BackColor = Color.Coral;
                txtMoisture2.BackColor = Color.Coral;
            }
            else if (@"空气干燥基(ad)" == groundmassA && @"干燥基(d)" == groundmassB)
            {
                txtMoisture1.BackColor = Color.Coral;
            }
            else if (@"空气干燥基(ad)" == groundmassA && @"干燥无灰基(daf)" == groundmassB)
            {
                txtMoisture1.BackColor = Color.Coral;
                txtAsh1.BackColor = Color.Coral;
            }
            else if (@"干燥基(d)" == groundmassA && @"收到基(ar)" == groundmassB)
            {// 干燥基->
                txtMoisture2.BackColor = Color.Coral;
            }
            else if (@"干燥基(d)" == groundmassA && @"空气干燥基(ad)" == groundmassB)
            {
                txtMoisture2.BackColor = Color.Coral;
            }
            else if (@"干燥基(d)" == groundmassA && @"干燥无灰基(daf)" == groundmassB)
            {
                txtAsh1.BackColor = Color.Coral;
            }
            else if (@"干燥无灰基(daf)" == groundmassA && @"收到基(ar)" == groundmassB)
            {// 干燥无灰基->
                txtMoisture2.BackColor = Color.Coral;
                txtAsh2.BackColor = Color.Coral;
            }
            else if (@"干燥无灰基(daf)" == groundmassA && @"空气干燥基(ad)" == groundmassB)
            {
                txtMoisture2.BackColor = Color.Coral;
                txtAsh2.BackColor = Color.Coral;
            }
            else if (@"干燥无灰基(daf)" == groundmassA && @"干燥基(d)" == groundmassB)
            {
                txtAsh2.BackColor = Color.Coral;
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

        private void ForMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                switch (tabMain.SelectedTab.Text)
                {
                    case @"管径计算":
                        PipeDiameter();
                        break;
                    case @"汽水性质":
                        SteamProperty();
                        break;
                    case @"管道特性":
                        PipeCharacteristic();
                        break;
                    case @"煤炭相关":
                        Coal();
                        break;
                    case @"杂项功能":
                        Misc();
                        break;
                }
            }
            else if (e.KeyCode == Keys.D && ModifierKeys == Keys.Alt)
            {
                resetControls(tabMain.SelectedTab);
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

        private void PipeDiameter()
        {
            double flowRate, innerDiameter, flowVelocity;
            double.TryParse(txtFlowRate.Text, out flowRate);
            double.TryParse(txtInnerDiameter.Text, out innerDiameter);
            double.TryParse(txtFlowVelocity.Text, out flowVelocity);

            double gasAmount, sectionHeight, sectionWidth;
            double.TryParse(txtGasAmount.Text, out gasAmount);
            double.TryParse(txtSectionHeight.Text, out sectionHeight);
            double.TryParse(txtSectionWidth.Text, out sectionWidth);

            
                if (rioDiameter.Checked)
                {
                    flowVelocity = flowRate / 3600 / (0.785 * innerDiameter * innerDiameter / 1000000);
                    txtFlowVelocity.Text = Math.Round(flowVelocity, 1) + string.Empty;
                }
                else if (rioVelocity.Checked)
                {
                    innerDiameter = 18.81 * Math.Sqrt(flowRate / flowVelocity);
                    txtInnerDiameter.Text = Math.Round(innerDiameter, 1)+ string.Empty;
                }
           

            // 风管
            var sectionArea = sectionHeight * sectionWidth;
            var gasVelocity = gasAmount / 3600 / sectionArea;
            txtGasVelocity.Text = Math.Round(gasVelocity, 1) + string.Empty;
            

           
        }

        private void SteamProperty()
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

        private void cmbNominalDiameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtDo.Clear();
            txtSCH.Clear();
            txtThk.Clear();
            txtDi.Clear();
            txtPw.Clear();
            txtMat.Clear();

            var dn = cmbNominalDiameter.Text.Replace("(", "").Replace(")", "");
            var standardName = cbxStandardName.Text;

            try
            {
                // 获取外径
                var xPath = string.Format("//Standard[@Name='{0}']/Pipe[@DN='{1}']/@DO", standardName, dn);
                var Do = _xmlHelper.GetOnlyAttributeValue(xPath);
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
                    xPath = string.Format("//Standard[@Name='{0}']/Pipe[@DN='{1}']/*[text()='{2}']/@PW", standardName, dn, sch);
                    var pw = _xmlHelper.GetOnlyAttributeValue(xPath);
                    txtPw.Text = pw;
                }
                txtThk.Text = thk;

                // 计算内径
                if ("SHT3405-2012" == cbxStandardName.Text.Trim())
                {
                    var nDo = double.Parse(Do);
                    var dThk = double.Parse(thk);
                    var nDi = Convert.ToInt32(nDo - 2 * dThk);
                    txtDi.Text = nDi + "";
                }
                else
                {
                    var dDo = double.Parse(Do);
                    var dThk = double.Parse(thk);
                    var dDi = Convert.ToDouble(dDo - 2 * dThk);
                    txtDi.Text = dDi + "";
                }

                Common.SetClipboard(Do + "×" + thk);
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

            cmbNominalDiameter.Focus();
            cmbNominalDiameter.SelectAll();
        }

        private void PipeCharacteristic()
        {

        }

        private void Coal()
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

            // 煤炭分类
            double vdaf;
            double.TryParse(txtVdaf.Text, out vdaf);
            if (vdaf > 0 && vdaf <= 10)
            {
                lblCoalRank.Text = @"无烟煤";
            }
            else if (vdaf > 10 && vdaf <= 37)
            {
                lblCoalRank.Text = @"烟煤";
            }
            else if (vdaf > 37)
            {
                lblCoalRank.Text = @"烟煤/褐煤";
            }
        }

        private void Misc()
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
                var flow = ConvertFlow(sp, st, op, ot, sf);
                flow = Math.Round(flow, 1);
                txtOperatingFlow.Text = flow + string.Empty;
            }
            else if (rioOperatingCondition.Checked)
            {
                var flow = ConvertFlow(op, ot, sp, st, of);
                flow = Math.Round(flow, 1);
                txtStandardFlow.Text = flow + string.Empty;
            }
        }

        private static double ConvertFlow(double p1, double t1, double p2, double t2, double f1)
        {
            t1 += 273.15;
            t2 += 273.15;
            var f2 = f1 * (t2 / t1) * (p1 / p2);
            return f2;
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

        private void cbxBoltSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtHoleSize.Text = string.Empty;
            var xpath = string.Format("//BlotHole/{0}[@TYPE='{1}']", cbxBoltSpec.Text, cbxEquipmentType.Text);
            txtHoleSize.Text = _xmlHelper.GetOnlyInnerText(xpath);
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

        #endregion
    }
}
