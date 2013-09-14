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
	/// 
	/// Tracks without position, rotation, or scale will be removed in
	/// order to support concurrent animations with the same entity.
	/// </summary>
	class Program
	{
		private static Dictionary<string, HashSet<string>> FilterMapper = new Dictionary<string, HashSet<string>>();
		private static Dictionary<string, string> FilterNameMapper = new Dictionary<string, string>();

		static void Main(string[] args)
		{
			Options options = new Options();
			if (Parser.Default.ParseArguments(args, options))
			{
				InitializeFilter(options);
				foreach (string fileName in options.Names)
				{
					IList<XDocument> inputs = options.Directories.Select(x => XDocument.Load(Path.Combine(x, fileName))).ToList();
					XDocument mergedDocument = Merge(inputs);

					using (XmlTextWriter writer = new XmlTextWriter(Path.Combine(options.Output, fileName), Encoding.ASCII))
						WriteXml(mergedDocument, writer);
				}
			}
		}

		private static bool CloseTo(float original, float compareTo, float threshold)
		{
			return Math.Abs(original - compareTo) < threshold;
		}

		private static bool CloseTo(float x, float y, float z, float compareTo, float threshold)
		{
			return CloseTo(x, compareTo, threshold) && CloseTo(y, compareTo, threshold) && CloseTo(z, compareTo, threshold);
		}

		private static bool HasTransformation(XElement trackElement)
		{
			foreach (XElement keyframeElement in trackElement.Element("keyframes").Elements("keyframe"))
			{
				XElement translateElement = keyframeElement.Element("translate");
				if (translateElement != null && !CloseTo((float)translateElement.Attribute("x"), (float)translateElement.Attribute("y"), (float)translateElement.Attribute("z"), 0, 0.001f))
					return true;

				XElement rotateElement = keyframeElement.Element("rotate");
				if (rotateElement != null)
				{
					if (!CloseTo((float)rotateElement.Attribute("angle"), 0, 0.001f))
						return true;
					XElement axisElement = rotateElement.Element("axis");
					if (axisElement != null && !CloseTo((float)axisElement.Attribute("x"), (float)axisElement.Attribute("y"), (float)axisElement.Attribute("z"), 0, 0.001f))
						return true;
				}

				XElement scaleElement = keyframeElement.Element("scale");
				if (scaleElement != null && !CloseTo((float)scaleElement.Attribute("x"), (float)scaleElement.Attribute("y"), (float)scaleElement.Attribute("z"), 1, 0.001f))
					return true;
			}

			return false;
		}
		
		private static void InitializeFilter(Options options)
		{
			XDocument doc = XDocument.Load("Filters.xml");
			foreach (XElement filterElement in doc.Root.Elements("Filter"))
			{
				string name = filterElement.Attribute("name").Value;
				HashSet<string> bones = new HashSet<string>(filterElement.Elements("Bone").Select(x => x.Attribute("name").Value));
				FilterMapper.Add(name, bones);
			}

			foreach (string value in options.Filters)
			{
				string[] filterArgs = value.Split(':');
				if (!FilterMapper.ContainsKey(filterArgs[1]))
					throw new ArgumentException(filterArgs[1] + " is not a filter.");
				FilterNameMapper.Add(filterArgs[0], filterArgs[1]);
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
			{
				XElement copy = new XElement(animationElement);
				animationRoot.Add(copy);
			}

			foreach (XElement animationElement in mergedDocument.Root.Element("animations").Elements("animation"))
			{
				IEnumerable<XElement> tracks = animationElement.Element("tracks").Elements("track");
				List<XElement> toBeRemoved = new List<XElement>();
				string animationName = animationElement.Attribute("name").Value;
				string filterName = FilterNameMapper.ContainsKey(animationName) ? FilterNameMapper[animationName] : "";
				if (FilterMapper.ContainsKey(filterName))
				{
					toBeRemoved.AddRange(tracks.Where(x => !FilterMapper[filterName].Contains(x.Attribute("bone").Value)));
					toBeRemoved.ForEach(x => x.Remove());
					toBeRemoved.Clear();
				}

				toBeRemoved.AddRange(tracks.Where(x => !HasTransformation(x)));
				toBeRemoved.ForEach(x => x.Remove());
			}

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
