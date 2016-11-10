using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

namespace ThermalMate
{
    public partial class Form1
    {
        private XmlHelper _xmlHelper;

        private static void Copy2Clipboard(string context)
        {
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, context);
        }
        private void ReleaseResource(string filePath, string resourceName)
        {
            // 获取当前正在执行的程序集
            var assembly = Assembly.GetExecutingAssembly();
            //foreach (var file in assembly.GetManifestResourceNames())
            // 辅助方法：获取资源在程序集内部的名称
            // MessageBox.Show(file);
            var stream = assembly.GetManifestResourceStream(resourceName);
            // 将流读取到二进制数组中
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            // 将二进制写入文件
            if (File.Exists(filePath)) return;
            var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
            fs.Write(buffer, 0, buffer.Length);
            fs.Flush();
            fs.Close();
        }
        

        // UEwasp导出函数声明
        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void SETSTD_WASP(int stdid);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2T(double p, ref double t, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2VL(double p, ref double v, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2VG(double p, ref double v, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2HL(double p, ref double h, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2HG(double p, ref double h, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2KSG(double p, ref double ks, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2KSL(double p, ref double ks, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2ETAL(double p, ref double h, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void P2ETAG(double p, ref double h, ref int range);
       
        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2P(double t, ref double p, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2VL(double p, ref double v, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2VG(double p, ref double v, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2HL(double p, ref double h, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2HG(double p, ref double h, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2KSG(double p, ref double ks, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2KSL(double p, ref double ks, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2ETAL(double p, ref double h, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void T2ETAG(double p, ref double h, ref int range);
       
        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void PT2V(double p, double t, ref double v, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void PT2H(double p, double T, ref double h, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void PT2ETA(double p, double T, ref double cp, ref int range);

        [DllImport("UEwasp.dll", CallingConvention = CallingConvention.StdCall)]
        public extern static void PT2KS(double p, double T, ref double ks, ref int range);
    }

    internal class XmlHelper
    {
        private string _xmlFilePath;

        public string XmlFilePath
        {
            set
            {
                if (File.Exists(value))
                {
                    _xmlFilePath = value;
                }
                else
                {
                    MessageBox.Show(value + "不存在", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
        }

        public XmlHelper()
        {

        }

        public XmlHelper(string xmlFilePath)
        {
            XmlFilePath = xmlFilePath;
        }

        private XmlNodeList GetNodes(string xPath)
        {
            var xmlDoc = CreateXmlDocument();
            // 根根xPath返回节点对象列表
            return xmlDoc != null ? xmlDoc.SelectNodes(xPath) : null;
        }

        /// <summary>
        /// 获取属性名称
        /// </summary>
        /// <param name="xPath">指定xpath</param>
        /// <returns></returns>
        public IEnumerable<string> GetAttributeValues(string xPath)
        {
            return from XmlAttribute attribute in GetNodes(xPath) select attribute.Value;
        }

        public string GetOnlyAttributeValue(string xPath)
        {
            var value = string.Empty;
            var nodes = GetNodes(xPath);
            if (nodes.Count > 0)
            {
                var attribute = nodes.Item(0) as XmlAttribute;
                value = attribute.Value;
            }

            return value;
        }

        /// <summary>
        /// 获取元素文本(多个元素)
        /// </summary>
        /// <param name="xPath">指定xpath</param>
        /// <returns></returns>
        public IEnumerable<string> GetInnerTexts(string xPath)
        {
            return from XmlElement element in GetNodes(xPath) select element.InnerText;
        }

        public string GetOnlyInnerText(string xPath)
        {
            var text = string.Empty;
            var nodes = GetNodes(xPath);
            if (nodes.Count > 0)
            {
                text = nodes.Item(0).InnerText;
            }

            return text;
        }

        public void SetInnerText(string xPath, string value)
        {
            var xmlDoc = CreateXmlDocument();
            // 根根xPath返回节点对象列表
            var nodes = xmlDoc.SelectNodes(xPath);
            if (nodes.Count > 0)
            {
                nodes.Item(0).InnerText = value;
            }
            xmlDoc.Save(_xmlFilePath);
        }

        /// <summary>
        /// 获取元素名称
        /// </summary>
        /// <param name="xPath">指定xpath</param>
        /// <returns></returns>
        public IEnumerable<string> GetElementNames(string xPath)
        {
            return from XmlElement element in GetNodes(xPath) select element.Name;
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="xPath">指定xpath</param>
        public void RemoveNode(string xPath)
        {
            var xmlDoc = CreateXmlDocument();
            var node = xmlDoc.SelectSingleNode(xPath);
            if (node != null && node.ParentNode != null) node.ParentNode.RemoveChild(node);
            xmlDoc.Save(_xmlFilePath);
        }

        /// <summary>
        /// 新增元素
        /// </summary>
        /// <param name="parentXPath">父节点xpath</param>
        /// <param name="elementName">元素名称</param>
        /// <param name="innerText">元素文本</param>
        public void AddElement(string parentXPath, string elementName, string innerText)
        {
            var xmlDoc = CreateXmlDocument();
            var node = xmlDoc.SelectSingleNode(parentXPath);
            var subElement = xmlDoc.CreateElement(elementName);
            subElement.InnerXml = innerText;
            if (node != null) node.AppendChild(subElement);
            xmlDoc.Save(_xmlFilePath);
        }

        /// <summary>
        /// 新增元素（带属性）
        /// </summary>
        /// <param name="parentXPath">父节点xpath</param>
        /// <param name="elementName">元素名称</param>
        /// <param name="innerText">元素文本</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="attributeValue">属性值</param>
        public void AddElement(string parentXPath, string elementName, string innerText, string attributeName, string attributeValue)
        {
            var xmlDoc = CreateXmlDocument();
            var node = xmlDoc.SelectSingleNode(parentXPath);
            var subElement = xmlDoc.CreateElement(elementName);
            subElement.InnerXml = innerText;
            var xmlAttribute = xmlDoc.CreateAttribute(attributeName);
            xmlAttribute.Value = attributeValue;
            subElement.Attributes.Append(xmlAttribute);
            if (node != null) node.AppendChild(subElement);
            xmlDoc.Save(_xmlFilePath);
        }

        private XmlDocument CreateXmlDocument()
        {
            XmlDocument xmlDoc = null;
            try
            {
                // 生成XmlDocument对象
                xmlDoc = new XmlDocument();
                // 指定xml文本
                xmlDoc.Load(_xmlFilePath);
            }
            catch (XmlException e)
            {
                MessageBox.Show(e.Message);
            }
            // 根根xPath返回节点对象列表
            return xmlDoc;
        }
    }
}