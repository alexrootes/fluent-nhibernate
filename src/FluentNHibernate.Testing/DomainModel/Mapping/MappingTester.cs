using System;
using System.Xml;
using FluentNHibernate.Mapping;
using NUnit.Framework;

namespace FluentNHibernate.Testing.DomainModel.Mapping
{
    public class MappingTester<T>
    {
        protected XmlElement currentElement;
        protected XmlDocument document;
        protected IMappingVisitor _visitor = new MappingVisitor();

        public virtual MappingTester<T> RootElement
        {
            get
            {
                currentElement = document.DocumentElement;
                return this;
            }
        }

        public virtual MappingTester<T> UsingVisitor(IMappingVisitor visitor)
        {
            _visitor = visitor;
            return this;
        }

        public virtual MappingTester<T> ForMapping(Action<ClassMap<T>> mappingAction)
        {
            var classMap = new ClassMap<T>();
            mappingAction(classMap);

            return ForMapping(classMap);
        }

        public virtual MappingTester<T> ForMapping(ClassMap<T> classMap)
        {
            document = classMap.CreateMapping(_visitor);
            currentElement = document.DocumentElement;

            return this;
        }

        public virtual MappingTester<T> Element(string elementPath)
        {
            currentElement = (XmlElement)document.DocumentElement.SelectSingleNode(elementPath);

            return this;
        }

        public virtual MappingTester<T> HasThisManyChildNodes(int expected)
        {
            currentElement.ChildNodeCountShouldEqual(expected);

            return this;
        }

        public virtual MappingTester<T> HasAttribute(string name, string value)
        {
            currentElement.AttributeShouldEqual(name, value);

            return this;
        }

        public virtual MappingTester<T> HasAttribute(string name, Func<string, bool> predicate)
        {
            currentElement.HasAttribute(name).ShouldBeTrue();

            predicate(currentElement.Attributes[name].Value).ShouldBeTrue();

            return this;
        }

        public virtual MappingTester<T> DoesntHaveAttribute(string name)
        {
            Assert.IsFalse(currentElement.HasAttribute(name), "Found attribute '" + name + "' on element.");

            return this;
        }

        public virtual MappingTester<T> Exists()
        {
            Assert.IsNotNull(currentElement);

            return this;
        }

        public virtual MappingTester<T> DoesntExist()
        {
            Assert.IsNull(currentElement);

            return this;
        }

        public virtual MappingTester<T> HasName(string name)
        {
            Assert.AreEqual(name, currentElement.Name, "Expected current element to have the name '" + name + "' but found '" + currentElement.Name + "'.");

            return this;
        }

        public virtual void OutputToConsole()
        {
        	Console.WriteLine(string.Empty);
        	Console.WriteLine(this.ToString());
        	Console.WriteLine(string.Empty);
        }

        public override string ToString()
        {
            var stringWriter = new System.IO.StringWriter();
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.Formatting = Formatting.Indented;
            this.document.WriteContentTo(xmlWriter);
            return stringWriter.ToString();
        }

        public MappingTester<T> ChildrenDontContainAttribute(string key, string value)
        {
            foreach (XmlElement node in currentElement.ChildNodes)
            {
                if (node.HasAttribute(key))
                    Assert.AreNotEqual(node.Attributes[key].Value, value);
            }
            return this;
        }

        public MappingTester<T> ValueEquals(string value)
        {
            Assert.That(currentElement.InnerXml, Is.EqualTo(value));

            return this;
        }
    }
}
