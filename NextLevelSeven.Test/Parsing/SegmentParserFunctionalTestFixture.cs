﻿using System.Linq;
using FluentAssertions;
using NextLevelSeven.Core;
using NextLevelSeven.Parsing;
using NextLevelSeven.Test.Testing;
using NUnit.Framework;

namespace NextLevelSeven.Test.Parsing
{
    [TestFixture]
    public class SegmentParserFunctionalTestFixture : ParsingBaseTestFixture
    {
        [Test]
        public void Segment_CloneCanGetDelimiter()
        {
            var element = Message.Parse(ExampleMessageRepository.Standard)[2].Clone();
            Assert.AreEqual('|', element.Delimiter);
        }

        [Test]
        public void Segment_CanGetAndSetType()
        {
            var element = Message.Parse(ExampleMessageRepository.Standard)[2];
            var newType = Any.StringCaps(3);
            element.Type = newType;
            Assert.AreEqual(newType, element.Type);
            Assert.AreEqual(newType, element[0].Value);
        }

        [Test]
        public void Segment_CanMoveMshFields()
        {
            var element = Message.Parse(ExampleMessageRepository.Minimum)[1];
            element[3].Value = Any.String();
            element[4].Value = Any.String();
            element[5].Value = Any.String();
            var newMessage = element.Clone();
            newMessage[3].Move(4);
            Assert.AreEqual(element[3].Value, newMessage[4].Value);
        }

        [Test]
        public void Segment_CanMoveFields()
        {
            var element = Message.Parse(Any.Message())[2];
            element[3].Value = Any.String();
            element[4].Value = Any.String();
            element[5].Value = Any.String();
            var newMessage = element.Clone();
            newMessage[3].Move(4);
            Assert.AreEqual(element[3].Value, newMessage[4].Value);
        }

        [Test]
        public void Segment_CanSetValues()
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            var type = Any.StringCaps(3);
            var data = Any.String();
            message[2].Values = new[] { type, data };
            Assert.AreEqual(string.Format("{0}|{1}", type, data), message[2].Value);
        }

        [Test]
        public void Segment_CanSetValuesOnMsh()
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            var data = Any.String();
            var delimiter = "$";
            message[1].Values = new[] { "MSH", delimiter, data };
            Assert.AreEqual(string.Format("MSH{0}{1}", delimiter, data), message[1].Value);
        }

        [Test]
        public void Segment_CanInsertFields()
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            var segment = message[2];
            var data = Any.String();
            segment.Insert(1, data);
            Assert.AreEqual(data, segment[1].Value);
        }

        [Test]
        public void Segment_CanInsertFieldElement()
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            var segment = message[2];
            var data = Message.Parse(Any.Message())[2][1];
            segment.Insert(1, data);
            Assert.AreEqual(data.Value, segment[1].Value);
        }

        [Test]
        [ExpectedException(typeof(ElementException))]
        public void Segment_CanNotInsertMshElement()
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            var segment = message[1];
            var data = Message.Parse(Any.Message())[2][1];
            segment.Insert(1, data);
        }

        [Test]
        public void Segment_GetsNextIndex()
        {
            var message = Message.Parse(Any.Message());
            var segment = message[2];
            Assert.AreEqual(segment.ValueCount, segment.NextIndex);
        }

        [Test]
        public void Segment_CanInsertFieldsInMsh()
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            var segment = message[1];
            var data = Any.String();
            segment.Insert(3, data);
            Assert.AreEqual(data, segment[3].Value);
        }

        [Test]
        [ExpectedException(typeof(ElementException))]
        public void Segment_Throws_WhenIndexedBelowZero()
        {
            var element = Message.Parse(ExampleMessageRepository.Standard)[1];
            element[-1].Value.Should().BeNull();
        }

        [Test]
        public void Segment_WithIdenticalValueToAnotherSegment_IsEquivalent()
        {
            var segment1 = Message.Parse(ExampleMessageRepository.Standard)[1];
            var segment2 = Message.Parse(ExampleMessageRepository.Standard)[1];
            Assert.AreEqual(segment1.Value, segment2.Value);
        }

        [Test]
        public void Segment_ReportsCorrectType()
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            Assert.AreEqual("MSH", message[1].Type, @"Segment didn't report correct type.");
        }

        [Test]
        public void Segment_CanBeCloned()
        {
            var segment = Message.Parse(ExampleMessageRepository.Standard)[1];
            var clone = segment.Clone();
            Assert.AreNotSame(segment, clone, "Cloned segment is the same referenced object.");
            Assert.AreEqual(segment.Value, clone.Value, "Cloned segment has different contents.");
        }

        [Test]
        public void Segment_CanBeClonedGenerically()
        {
            var segment = (IElement) Message.Parse(ExampleMessageRepository.Standard)[1];
            var clone = segment.Clone();
            Assert.AreNotSame(segment, clone, "Cloned segment is the same referenced object.");
            Assert.AreEqual(segment.Value, clone.Value, "Cloned segment has different contents.");
        }

        [Test]
        public void Segment_HasMessageAncestor()
        {
            var element = Message.Parse(ExampleMessageRepository.Minimum)[1];
            Assert.IsNotNull(element.Ancestor);
        }

        [Test]
        public void Segment_HasGenericMessageAncestor()
        {
            var element = Message.Parse(ExampleMessageRepository.Minimum)[1] as ISegment;
            Assert.IsNotNull(element.Ancestor);
        }

        [Test]
        public void Segment_HasGenericAncestor()
        {
            var element = Message.Parse(ExampleMessageRepository.Minimum)[1] as IElementParser;
            Assert.IsNotNull(element.Ancestor as IMessage);
        }

        [Test]
        public void Segment_HasClonedEncoding()
        {
            var segment = Message.Parse(ExampleMessageRepository.Standard)[1];
            var clone = segment.Clone();
            Assert.AreSame(segment.Encoding, clone.Encoding, "Cloned segment is the same referenced object.");
        }

        [Test]
        public void Segment_CanAddDescendantsAtEnd()
        {
            var segment = Message.Parse(ExampleMessageRepository.Standard)[2];
            var fieldCount = segment.ValueCount;
            var id = Any.String();
            segment[fieldCount].Value = id;
            Assert.AreEqual(fieldCount + 1, segment.ValueCount,
                @"Number of elements after appending at the end is incorrect.");
        }

        [Test]
        public void Segment_CanGetFields()
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            var segment = message[2];
            Assert.AreEqual(7, segment.Fields.Count());
        }

        [Test]
        public void Segment_CanGetFieldsGenerically()
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            ISegment segment = message[2];
            Assert.AreEqual(7, segment.Fields.Count());
        }

        [Test]
        public void Segment_CanGetFieldsByIndexer()
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            var segment = message[2];
            var field = segment[2];
            Assert.AreEqual(@"20130528073829", field.Value);
        }

        [Test]
        public void Segment_CanDeleteField()
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            var segment = message[1];
            var field3 = segment[3].Value;
            var field5 = segment[5].Value;
            var field6 = segment[6].Value;
            ElementExtensions.Delete(segment, 4);
            Assert.AreEqual(field3, segment[3].Value, @"Expected segment[3] to remain the same after delete.");
            Assert.AreEqual(field5, segment[4].Value, @"Expected segment[5] to become segment[4].");
            Assert.AreEqual(field6, segment[5].Value, @"Expected segment[6] to become segment[5].");
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [ExpectedException(typeof(ElementException))]
        public void Segment_CanNotDeleteMshEncodingFields(int index)
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            var segment = message[1];
            ElementExtensions.Delete(segment, index);
        }

        [Test]
        public void Segment_CanDeleteNonMshField()
        {
            var message = Message.Parse(ExampleMessageRepository.Standard);
            var segment = message[2];
            var field3 = segment[3].Value;
            var field5 = segment[5].Value;
            var field6 = segment[6].Value;
            ElementExtensions.Delete(segment, 4);
            Assert.AreEqual(field3, segment[3].Value, @"Expected segment[3] to remain the same after delete.");
            Assert.AreEqual(field5, segment[4].Value, @"Expected segment[5] to become segment[4].");
            Assert.AreEqual(field6, segment[5].Value, @"Expected segment[6] to become segment[5].");
        }

        [Test]
        public void Segment_CanDeleteFieldsViaLinq()
        {
            var message = Message.Parse("MSH|^~\\&|1|2|3|4|5");
            var segment = message[1];
            segment.Descendants.Skip(2).Where(i => i.Converter.AsInt%2 == 0).Delete();
            Assert.AreEqual("MSH|^~\\&|1|3|5", message.Value, @"Message was modified unexpectedly.");
        }

        [Test]
        public void Segment_WillPointToCorrectFieldValue_WhenFieldsChange()
        {
            var message = Message.Parse();
            var msh3 = message[1][3];
            var msh4 = message[1][4];
            var expected = Any.String().Substring(0, 5);

            msh4.Value = expected;
            msh3.Value = Any.String();
            Assert.AreEqual(msh4.Value, expected);
        }

        [Test]
        public void Segment_WithSignificantDescendants_ShouldClaimToHaveSignificantDescendants()
        {
            var message = Message.Parse();
            Assert.IsTrue(message[1].HasSignificantDescendants(),
                @"Segment claims to not have descendants when it should.");
        }

        [Test]
        public void Segment_WillConsiderNonPresentValuesToNotExist()
        {
            var message = Message.Parse();
            Assert.IsFalse(message[2].Exists, @"Nonexistant segment is marked as existing.");
        }

        [Test]
        public void Segment_CanWriteStringValue()
        {
            var segment = Message.Parse(ExampleMessageRepository.Standard)[2];
            var value = Any.String();
            segment.Value = value;
            Assert.AreEqual(value, segment.Value, "Value mismatch after write.");
        }

        [Test]
        public void Segment_CanWriteNullValue()
        {
            var segment = Message.Parse(ExampleMessageRepository.Standard)[2];
            var value = Any.String();
            segment.Value = value;
            segment.Value = null;
            Assert.IsNull(segment.Value, "Value mismatch after write.");
        }
    }
}