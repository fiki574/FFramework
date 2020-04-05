/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2019/2020 Bruno Fištrek

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace FFramework_Core.XML
{
	public class XMLConfigFile : ConfigElement
	{
		public XMLConfigFile() : base(null)
		{
		}

		private static bool IsBadXMLElementName(string name)
		{
			if (name == null)
			{
				return false;
			}
			if (name.IndexOf("\\") != -1)
			{
				return true;
			}
			if (name.IndexOf("/") != -1)
			{
				return true;
			}
			if (name.IndexOf("<") != -1)
			{
				return true;
			}
			if (name.IndexOf(">") != -1)
			{
				return true;
			}
			return false;
		}

		private static void SaveElement(XmlWriter writer, string name, ConfigElement element)
		{
			bool flag = IsBadXMLElementName(name);
			if (element.HasChildren)
			{
				if (name == null)
				{
					name = "root";
				}
				if (flag)
				{
					writer.WriteStartElement("param");
					writer.WriteAttributeString("name", name);
				}
				else
				{
					writer.WriteStartElement(name);
				}
				foreach (KeyValuePair<string, ConfigElement> child in element.Children)
				{
					SaveElement(writer, child.Key, child.Value);
				}
				writer.WriteEndElement();
			}
			else if (name != null)
			{
				if (flag)
				{
					writer.WriteStartElement("param");
					writer.WriteAttributeString("name", name);
					writer.WriteString(element.GetString());
					writer.WriteEndElement();
				}
				else
				{
					writer.WriteElementString(name, element.GetString());
				}
			}
		}

		public void Save(FileInfo configFile)
		{
			if (configFile == null)
			{
				throw new ArgumentNullException("configFile");
			}
			if (configFile.Exists)
			{
				configFile.Delete();
			}
            XmlTextWriter xmlTextWriter = new XmlTextWriter(configFile.FullName, Encoding.ASCII)
            {
                Formatting = Formatting.Indented
            };
            XmlTextWriter xmlTextWriter2 = xmlTextWriter;
			try
			{
				xmlTextWriter2.WriteStartDocument();
				SaveElement(xmlTextWriter2, null, this);
				xmlTextWriter2.WriteEndDocument();
			}
			finally
			{
				xmlTextWriter2.Close();
			}
		}

		public static XMLConfigFile ParseXMLFile(FileInfo configFile)
		{
			if (configFile == null)
			{
				throw new ArgumentNullException("configFile");
			}
			XMLConfigFile xMLConfigFile = new XMLConfigFile();
			if (!configFile.Exists)
			{
				return xMLConfigFile;
			}
			try
			{
				ConfigElement configElement = xMLConfigFile;
				using (XmlTextReader xmlTextReader = new XmlTextReader(configFile.OpenRead()))
				{
					while (xmlTextReader.Read())
					{
						if (xmlTextReader.NodeType == XmlNodeType.Element)
						{
							if (!(xmlTextReader.Name == "root"))
							{
								if (xmlTextReader.Name == "param")
								{
									string attribute = xmlTextReader.GetAttribute("name");
									if (attribute != null && attribute != "root")
									{
										ConfigElement configElement3 = configElement[attribute] = new ConfigElement(configElement);
										configElement = configElement3;
									}
								}
								else
								{
									ConfigElement configElement4 = new ConfigElement(configElement);
									configElement[xmlTextReader.Name] = configElement4;
									configElement = configElement4;
								}
							}
						}
						else if (xmlTextReader.NodeType == XmlNodeType.Text)
						{
							configElement.Set(xmlTextReader.Value);
						}
						else if (xmlTextReader.NodeType == XmlNodeType.EndElement && xmlTextReader.Name != "root")
						{
							configElement = configElement.Parent;
						}
					}
					return xMLConfigFile;
				}
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex.ToString());
				return null;
			}
		}
	}
}