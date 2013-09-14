using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using CommandLine;

namespace BlenderExporterFix
{
	/// <summary>
	/// This program is written to correct a bug in the blender exporter.
	/// Only the selected NLA strip will be exported properly, while the
	/// rest of the animations will have a fixed position and rotation.
	/// 
	/// To ensure that all animation works, only export one animation at
	/// a time, and use this tool to merge all animations together.
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			Options options = new Options();
			if (Parser.Default.ParseArguments(args, options))
			{
				foreach (string fileName in options.Names)
				{
					IList<XDocument> inputs = options.Directories.Select(x => XDocument.Load(Path.Combine(x, fileName))).ToList();
					XDocument mergedDocument = Merge(inputs);

					using (XmlTextWriter writer = new XmlTextWriter(Path.Combine(options.Output, fileName), Encoding.ASCII))
						WriteXml(mergedDocument, writer);
				}
			}
		}

		private static XDocument Merge(IEnumerable<XDocument> inputs)
		{
			XDocument original = inputs.First();
			XDocument mergedDocument = new XDocument(original);
			IEnumerable<XElement> animationElements = from x in inputs
													  where x != original
													  select x.Root.Element("animations").Element("animation");
			XElement animationRoot = mergedDocument.Root.Element("animations");
			foreach (XElement animationElement in animationElements)
				animationRoot.Add(new XElement(animationElement));

			return mergedDocument;
		}

		private static void WriteXml(XDocument doc, XmlTextWriter writer)
		{
			writer.Formatting = Formatting.Indented;
			doc.WriteTo(writer);
			writer.Flush();
		}
	}
}
