﻿using System.Linq;
using NextLevelSeven.Core;
using NextLevelSeven.Parsing;
using NextLevelSeven.Test.Testing;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NextLevelSeven.Test.Parsing
{
    [TestFixture]
    public class SubcomponentParserFunctionalTests : ParsingTestFixture
    {
        [Test]
        public void Subcomponent_CanGetKey()
        {
            var message = Message.Parse(ExampleMessages.Minimum);
            var element = message[1][3][1][1][1];
            Assert.AreEqual("MSH1.3.1.1.1", element.Key);
        }

        [Test]
        public void Subcomponent_UsesSameEncodingAsMessage()
        {
            var message = Message.Parse(ExampleMessages.Minimum);
            var element = message[1][3][1][1][1];
            Assert.AreSame(message.Encoding, element.Encoding);
        }

        [Test]
        public void Subcomponent_CanGetMessage()
        {
            var message = Message.Parse(ExampleMessages.Minimum);
            var element = message[1][3][1][1][1];
            Assert.AreSame(message, element.Message);
        }

        [Test]
        public void Subcomponent_CloneHasNoMessage()
        {
            var message = Message.Parse(ExampleMessages.Minimum);
            var element = message[1][3][1][1][1].Clone();
            Assert.IsNull(element.Message);
        }

        [Test]
        public void Subcomponent_HasNoDescendants()
        {
            var element = Message.Parse(ExampleMessages.Minimum)[1][3][1][1][1];
            Assert.AreEqual(0, element.Descendants.Count());
        }

        [Test]
        public void Subcomponent_HasComponentAncestor()
        {
            var element = Message.Parse(ExampleMessages.Minimum)[1][3][1][1][1];
            Assert.IsNotNull(element.Ancestor);
        }

        [Test]
        public void Subcomponent_HasGenericComponentAncestor()
        {
            var element = Message.Parse(ExampleMessages.Minimum)[1][3][1][1][1] as ISubcomponent;
            Assert.IsNotNull(element.Ancestor);
        }

        [Test]
        public void Subcomponent_HasGenericAncestor()
        {
            var element = Message.Parse(ExampleMessages.Minimum)[1][3][1][1][1] as IElementParser;
            Assert.IsNotNull(element.Ancestor as IComponent);
        }

        [Test]
        public void Subcomponent_HasOneValue()
        {
            var element = Message.Parse(ExampleMessages.Minimum)[1][3][1][1][1];
            var val0 = MockFactory.String();
            element.Value = val0;
            Assert.AreEqual(1, element.ValueCount);
            Assert.AreEqual(element.Value, val0);
            Assert.AreEqual(1, element.Values.Count());
        }

        [Test]
        public void Subcomponent_ThrowsWhenMovingElements()
        {
            var element = Message.Parse(ExampleMessages.Minimum)[1][3][1][1][1];
            element.Value = MockFactory.String();
            var newMessage = element.Clone();
            AssertAction.Throws<ParserException>(() => newMessage[2].Move(3));
            Assert.AreEqual(element.Value, newMessage.Value);
        }

        [Test]
        public void Subcomponent_Throws_WhenIndexed()
        {
            var element = Message.Parse(ExampleMessages.Standard)[1][3][1][1][1];
            string value = null;
            AssertAction.Throws<ParserException>(() => { value = element[1].Value; });
            Assert.IsNull(value);
        }

        [Test]
        public void Subcomponent_CanBeCloned()
        {
            var subcomponent = Message.Parse(ExampleMessages.Standard)[1][3][1][1][1];
            var clone = subcomponent.Clone();
            Assert.AreNotSame(subcomponent, clone, "Cloned subcomponent is the same referenced object.");
            Assert.AreEqual(subcomponent.Value, clone.Value, "Cloned subcomponent has different contents.");
        }

        [Test]
        public void Subcomponent_CanBeClonedGenerically()
        {
            var subcomponent = (IElement)Message.Parse(ExampleMessages.Standard)[1][3][1][1][1];
            var clone = subcomponent.Clone();
            Assert.AreNotSame(subcomponent, clone, "Cloned subcomponent is the same referenced object.");
            Assert.AreEqual(subcomponent.Value, clone.Value, "Cloned subcomponent has different contents.");
        }

        [Test]
        public void Subcomponent_CanAddDescendantsAtEnd()
        {
            var subcomponent = Message.Parse(ExampleMessages.Standard)[2][3][4][1];
            var count = subcomponent.ValueCount;
            var id = MockFactory.String();
            subcomponent[count + 1].Value = id;
            Assert.AreEqual(count + 1, subcomponent.ValueCount,
                @"Number of elements after appending at the end of a subcomponent is incorrect.");
        }

        [Test]
        public void Subcomponent_CanWriteStringValue()
        {
            var subcomponent = Message.Parse(ExampleMessages.Standard)[1][3][1][1][1];
            var value = MockFactory.String();
            subcomponent.Value = value;
            Assert.AreEqual(value, subcomponent.Value, "Value mismatch after write.");
        }

        [Test]
        public void Subcomponent_CanWriteNullValue()
        {
            var subcomponent = Message.Parse(ExampleMessages.Standard)[1][3][1][1][1];
            var value = MockFactory.String();
            subcomponent.Value = value;
            subcomponent.Value = null;
            Assert.IsNull(subcomponent.Value, "Value mismatch after write.");
        }
    }
}