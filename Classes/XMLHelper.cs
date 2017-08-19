using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace ThermalMate.Classes
{
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
