using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;

namespace BlenderExporterFix
{
	internal class Options
	{
		[OptionList('d', "directory", Separator = '|', HelpText = "Input XML directories.", Required = true)]
		public IList<string> Directories { get; set; }

		[OptionList('n', "names", Separator = '|', HelpText = "Input XML file names.", Required = true)]
		public IList<string> Names { get; set; }

		[Option('o', "output", HelpText = "Output XML directory.", Required = true)]
		public string Output { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("This tool merges animations from specified skeleton XMLs to one skeleton XML.");
			sb.AppendLine("-d    Specify a list of directories of input XMLs delimited by '|'.");
			sb.AppendLine("-n    Specify a list of names of input XMLs delimited by '|'.");
			sb.AppendLine("-o    Specify output directory of output XMLs.");
			return sb.ToString();
		}
	}
}
