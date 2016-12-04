using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ThermalMate.Class;

namespace ThermalMate
{
    public partial class Form1 : Form
    {
        private readonly XmlHelper _xmlHelper;

        public Form1()
        {
            InitializeComponent();

            Text = string.Format("{0}  V{1}   【Code by hangch】",
                Application.ProductName,
                Application.ProductVersion);
            
            Common.ReleaseResource("ThermalMate.Resource.ThermalMate.xml", "ThermalMate.xml");
            Common.ReleaseResource("ThermalMate.Resource.UEwasp.dll", "UEwasp.dll");

            _xmlHelper = new XmlHelper(@"ThermalMate.xml");

            LoadConfig();
        }

        public void LoadConfig()
        {
            chkTopMost.Checked = TopMost = Convert.ToBoolean(_xmlHelper.GetOnlyInnerText("//Config/TopMost"));

            var mediums = _xmlHelper.GetElementNames("//Velocity/*").ToList();
            var velocities = _xmlHelper.GetInnerTexts("//Velocity/*").ToList();
            for (var i = 0; i < mediums.Count; i++)
            {
                if (mediums[i].Contains("分隔线"))
                {
                    lstVelocity.Items.Add(new ListViewItem(new[] { "////////////////////", "////////" }));
                    continue;
                }
                lstVelocity.Items.Add(new ListViewItem(new[] { mediums[i], velocities[i] }));
            }

            // 隔行换色
            for (var i = 0; i < lstVelocity.Items.Count; i++)
            {
                if (i % 2 == 0)
                {
                    lstVelocity.Items[i].BackColor =Color.BurlyWood;
                }
            }
            
            // 读取项目名
            cbxProject.DataSource = _xmlHelper.GetAttributeValues("//Project/@Name").ToList();
            cbxProject.Text = _xmlHelper.GetOnlyInnerText("//Config/Project");
            cbxSpecification.Text = _xmlHelper.GetOnlyInnerText("//Config/Specification");
            cbxStandardName.Text = _xmlHelper.GetOnlyInnerText("//Config/StandardName");
        }

        private void tabControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    switch (tabControl.SelectedIndex)
                    {
                        case 0:
                            Page_Diameter();
                            e.Handled = true;
                            break;
                        case 1:
                            Page_Steam();
                            e.Handled = true;
                            break;
                        case 2:
                            // TODO
                            break;
                        case 3:
                            Page_Misc();
                            e.Handled = true;
                            break;
                    }
                    break;
                case Keys.Escape:
                    if (ActiveControl is TextBox)
                    {
                        ((TextBox)ActiveControl).Clear();
                    }
                    e.Handled = true;
                    break;
            }
        }

        private void Page_Diameter()
        {
            if (rioDiameter.Checked)
            {
                var volumeFlow = Convert.ToDouble(txtVolumeFlow.Text);
                var diameter = Convert.ToDouble(txtDiameter.Text);
                var velocity = Diameter2Velocity(volumeFlow, diameter);
                txtVelocity.Text = Math.Round(velocity, 1).ToString();
            }
            else if (rioVelocity.Checked)
            {
                var volumeFlow = Convert.ToDouble(txtVolumeFlow.Text);
                var velocity = Convert.ToDouble(txtVelocity.Text);
                var diameter = Velocity2Diameter(volumeFlow, velocity);
                txtDiameter.Text = diameter.ToString();
            }
        }

        private void Page_Steam()
        {
            // 清空编辑框
            Common.ClearGroupTextControl(grpSaturatedSteam);
            Common.ClearGroupTextControl(grpSuperheatedSteam);
            Common.ClearGroupTextControl(grpSaturatedWater);

            double massFlow, pressure, temperature, retValue = 0.0;
            var range = 0;
            double.TryParse(txtMassFlow.Text, out massFlow);
            double.TryParse(txtPressure.Text, out pressure);
            double.TryParse(txtTemperature.Text, out temperature);

            // 使用IFC97标准
            Steam.SETSTD_WASP(97);

            if (chkPressure.Checked && chkTemperature.Checked)
            {// 过热汽
                Steam.PT2V(pressure, temperature, ref retValue, ref range);
                var volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow1.Text = Math.Round(volumeFlow, 1).ToString();
                txtDensity1.Text = Math.Round(1 / retValue, 3).ToString();

                Steam.PT2H(pressure, temperature, ref retValue, ref range);
                txtEnthalpy1.Text = Math.Round(retValue, 2).ToString();

                Steam.PT2ETA(pressure, temperature, ref retValue, ref range);
                txtViscosity1.Text = Math.Round(retValue * 1000, 3).ToString();

                Steam.PT2KS(pressure, temperature, ref retValue, ref range);
                txtIsoIndex1.Text = Math.Round(retValue, 3).ToString();

            }
            else if (chkPressure.Checked && !chkTemperature.Checked)
            {
                Steam.P2T(pressure, ref retValue, ref range);
                txtTemperature.Text = Math.Round(retValue, 1).ToString();

                // 饱和汽
                Steam.P2VG(pressure, ref retValue, ref range);
                var volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow2.Text = Math.Round(volumeFlow, 1).ToString();
                txtDensity2.Text = Math.Round(1 / retValue, 3).ToString();

                Steam.P2HG(pressure, ref retValue, ref range);
                txtEnthalpy2.Text = Math.Round(retValue, 2).ToString();

                Steam.P2ETAG(pressure, ref retValue, ref range);
                txtViscosity2.Text = Math.Round(retValue * 1000, 3).ToString();

                Steam.P2KSG(pressure, ref retValue, ref range);
                txtIsoIndex2.Text = Math.Round(retValue, 3).ToString();

                // 饱和水
                Steam.P2VL(pressure, ref retValue, ref range);
                volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow3.Text = Math.Round(volumeFlow, 1).ToString();
                txtDensity3.Text = Math.Round(1 / retValue, 3).ToString();

                Steam.P2HL(pressure, ref retValue, ref range);
                txtEnthalpy3.Text = Math.Round(retValue, 2).ToString();

                Steam.P2ETAL(pressure, ref retValue, ref range);
                txtViscosity3.Text = Math.Round(retValue * 1000, 3).ToString();

                Steam.P2KSL(pressure, ref retValue, ref range);
                txtIsoIndex3.Text = Math.Round(retValue, 3).ToString();
            }
            else if (!chkPressure.Checked && chkTemperature.Checked)
            {
                Steam.T2P(temperature, ref retValue, ref range);
                txtPressure.Text = Math.Round(retValue, 3).ToString();

                // 饱和汽
                Steam.T2VG(temperature, ref retValue, ref range);
                var volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow2.Text = Math.Round(volumeFlow, 1).ToString();
                txtDensity2.Text = Math.Round(1 / retValue, 3).ToString();

                Steam.T2HG(temperature, ref retValue, ref range);
                txtEnthalpy2.Text = Math.Round(retValue, 2).ToString();

                Steam.T2ETAG(temperature, ref retValue, ref range);
                txtViscosity2.Text = Math.Round(retValue * 1000, 3).ToString();

                Steam.T2KSG(temperature, ref retValue, ref range);
                txtIsoIndex2.Text = Math.Round(retValue, 3).ToString();

                // 饱和水
                Steam.T2VL(temperature, ref retValue, ref range);
                volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow3.Text = Math.Round(volumeFlow, 1).ToString();
                txtDensity3.Text = Math.Round(1 / retValue, 3).ToString();

                Steam.T2HL(temperature, ref retValue, ref range);
                txtEnthalpy3.Text = Math.Round(retValue, 2).ToString();

                Steam.T2ETAL(temperature, ref retValue, ref range);
                txtViscosity3.Text = Math.Round(retValue * 1000, 3).ToString();

                Steam.T2KSL(temperature, ref retValue, ref range);
                txtIsoIndex3.Text = Math.Round(retValue, 3).ToString();
            }

            // 区间判断
            switch (range)
            {
                case 1:
                    txtState.Text = @"过冷区";
                    break;
                case 2:
                    txtState.Text = @"过热区";
                    break;
                case 4:
                    txtState.Text = @"饱和区";
                    break;
            }
        }

        private void Page_Misc()
        {
            // 管道等级查询
            if (cmbNominalDiameter.Focused || cbxProject.Focused || cbxSpecification.Focused || cbxStandardName.Focused)   
            {
                Common.ClearGroupTextControl(grpPipe);

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
                    txtSCH.Text = thk;

                    // 材质
                    xPath = string.Format("//Project[@Name='{0}']/{1}/*[text()='{2}']/@MATERIAL",
                        cbxProject.Text,
                        cbxSpecification.Text,
                        dn);
                    txtMaterial.Text = _xmlHelper.GetOnlyAttributeValue(xPath);

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

                return;
            }

            // 流量折算
            double op, ot, of, sp, st, sf;
            double.TryParse(txtOperatingPressure.Text, out op);
            double.TryParse(txtOperatingTemperature.Text, out ot);
            double.TryParse(txtOperatingFlow.Text, out of);
            double.TryParse(txtStandardPressure.Text, out sp);
            double.TryParse(txtStandardTemperature.Text, out st);
            double.TryParse(txtStandardFlow.Text, out sf);

            if (rioStandardCondition.Checked)
            {
                var flow = FlowConversion(sp, st, op, ot, sf);
                flow = Math.Round(flow, 1);
                txtOperatingFlow.Text = flow.ToString();
            }
            else if (rioOperatingCondition.Checked)
            {
                var flow = FlowConversion(op, ot, sp, st, of);
                flow = Math.Round(flow, 1);
                txtStandardFlow.Text = flow.ToString();
            }
        }

        private void cbxBoltSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtHoleSize.Text = string.Empty;
            var xpath = string.Format("//BlotHole/{0}[@TYPE='{1}']", cbxBoltSpec.Text, cbxEquipmentType.Text);
            txtHoleSize.Text = _xmlHelper.GetOnlyInnerText(xpath);
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

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {

            _xmlHelper.SetInnerText("//Config/TopMost", chkTopMost.Checked.ToString());
        }

        #region 界面代码
        private void rioStandardCondition_CheckedChanged(object sender, EventArgs e)
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

        private void rioDiameter_CheckedChanged(object sender, EventArgs e)
        {
            if (rioVelocity.Checked)
            {
                txtVelocity.ReadOnly = false;
                txtDiameter.ReadOnly = true;
                txtVelocity.Clear();
                txtDiameter.Clear();
                txtVelocity.Focus();
            }
            else if (rioDiameter.Checked)
            {
                txtVelocity.ReadOnly = true;
                txtDiameter.ReadOnly = false;
                txtVelocity.Clear();
                txtDiameter.Clear();
                txtDiameter.Focus();
            }
        }

        private void chkPressure_CheckedChanged(object sender, EventArgs e)
        {
            txtPressure.ReadOnly = !txtPressure.ReadOnly;
        }

        private void chkTemperature_CheckedChanged(object sender, EventArgs e)
        {
            txtTemperature.ReadOnly = !txtTemperature.ReadOnly;
        }

        private void chkTopMost_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = chkTopMost.Checked;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl.SelectedTab.TabIndex)
            {
                case 0:
                    txtVolumeFlow.Focus();
                    break;
                case 1:
                    txtOperatingPressure.Focus();
                    break;
                case 2:
                    txtPressure.Focus();
                    break;
            }
        }

        private void sendtoDiameter_DoubleClick(object sender, EventArgs e)
        {
            var txt = (TextBox)sender;
            if (null == txt) return;
            txtDiameter.Clear();
            txtVelocity.Clear();
            rioDiameter.Checked = true;
            txtDiameter.Text = txt.Text;
            tabControl.SelectedIndex = 0;
        }

        private void sendtoVolumeFlow_DoubleClick(object sender, EventArgs e)
        {
            var txt = (TextBox)sender;
            if (null == txt) return;
            txtVolumeFlow.Text = txt.Text;
            tabControl.SelectedIndex = 0;
            txtDiameter.Clear();
            txtVelocity.Clear();
            if (rioDiameter.Checked)
            {
                txtDiameter.Focus();
            }
            else if (rioVelocity.Checked)
            {
                txtVelocity.Focus();
            }
        }

        #endregion

        #region 计算代码
        /// <summary>
        /// 流量折算
        /// </summary>
        public static double FlowConversion(double p1, double t1, double p2, double t2, double f1)
        {
            t1 += 273.15;
            t2 += 273.15;
            var f2 = f1 * (t2 / t1) * (p1 / p2);
            return f2;
        }

        /// <summary>
        /// 内径2流速
        /// </summary>
        public static double Diameter2Velocity(double flow, double diameter)
        {
            var velocity = flow / 3600 / (0.785 * diameter * diameter / 1000000);
            return Math.Round(velocity, 1);
        }

        /// <summary>
        /// 流速2内径
        /// </summary>
        public static int Velocity2Diameter(double flow, double velocity)
        {
            var diameter = 18.81 * Math.Sqrt(flow / velocity);
            return Convert.ToInt32(diameter);
        }


        #endregion

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

    }
}
