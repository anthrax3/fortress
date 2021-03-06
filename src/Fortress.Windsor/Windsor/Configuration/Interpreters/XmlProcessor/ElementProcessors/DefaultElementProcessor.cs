// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


using System.Xml;

namespace Castle.Windsor.Configuration.Interpreters.XmlProcessor.ElementProcessors
{
	public class DefaultElementProcessor : AbstractXmlNodeProcessor
	{
		private const string IncludeAttrName = "includeUri";
		private static readonly IncludeElementProcessor includeProcessor = new IncludeElementProcessor();
		private static readonly DefaultTextNodeProcessor textProcessor = new DefaultTextNodeProcessor();

		public override string Name
		{
			get { return ""; }
		}

		public override void Process(IXmlProcessorNodeList nodeList, IXmlProcessorEngine engine)
		{
			var element = (XmlElement) nodeList.Current;

			ProcessAttributes(element, engine);

			engine.DispatchProcessAll(new DefaultXmlProcessorNodeList(element.ChildNodes));
		}

		private static void ProcessAttributes(XmlElement element, IXmlProcessorEngine engine)
		{
			ProcessIncludeAttribute(element, engine);

			foreach (XmlAttribute att in element.Attributes)
				textProcessor.ProcessString(att, att.Value, engine);
		}

		private static void ProcessIncludeAttribute(XmlElement element, IXmlProcessorEngine engine)
		{
			var include = element.Attributes[IncludeAttrName];

			if (include == null)
				return;
			// removing the include attribute from the element
			element.Attributes.RemoveNamedItem(IncludeAttrName);

			var includeContent = includeProcessor.ProcessInclude(element, include.Value, engine);

			if (includeContent != null)
				element.PrependChild(includeContent);
		}
	}
}