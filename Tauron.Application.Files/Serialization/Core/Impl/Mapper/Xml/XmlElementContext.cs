﻿using System;
using System.Xml;
using System.Xml.Linq;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal class XmlElementContext : ContextImplBase
    {
        private XContainer _currentElement;

        public XmlElementContext([NotNull] SerializationContext original, [CanBeNull] XDeclaration declaration,
                                 [CanBeNull] XNamespace xNamespace, [NotNull] string rootName) : base(original)
        {
            XDocument doc = null;
            XElement ele = null;

            switch (original.SerializerMode)
            {
                case SerializerMode.Deserialize:
                    doc = XDocument.Load(TextReader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
                    break;
                case SerializerMode.Serialize:
                    if (declaration != null)
                    {
                        doc = new XDocument(declaration, new XElement(xNamespace + rootName));
                        _currentElement = doc;
                    }
                    else
                    {
                        ele = new XElement(rootName);
                        _currentElement = ele;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }

            XElement = ele ?? doc.Root;
        }

        [NotNull]
        public XElement XElement { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (Original.SerializerMode != SerializerMode.Serialize) return;

            using (var writer = XmlWriter.Create(TextWriter,
                new XmlWriterSettings { Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates }))
            {
                _currentElement.WriteTo(writer);
                _currentElement = null;
            }

            base.Dispose(disposing);
        }
    }
}