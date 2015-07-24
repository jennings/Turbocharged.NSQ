using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Turbocharged.NSQ.Tests
{
    public class MessageConversionFacts
    {
        static string SAMPLE_TEXT_FILE = System.IO.Path.Combine(Environment.CurrentDirectory, "UTF-8-sample-text.txt");

        public static TheoryData<string> SampleTextAsString()
        {
            return new TheoryData<string> { System.IO.File.ReadAllText(SAMPLE_TEXT_FILE, Encoding.UTF8) };
        }

        public static TheoryData<byte[]> SampleTextAsByteArray()
        {
            return new TheoryData<byte[]> { System.IO.File.ReadAllBytes(SAMPLE_TEXT_FILE) };
        }

        [Theory]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6 })]
        [MemberData("SampleTextAsByteArray")]
        public void CanConvertToAndFromByteArray(byte[] testValue)
        {
            MessageBody converted = testValue;
            byte[] result = converted;

            Assert.True(testValue.SequenceEqual(result));
        }

        [Theory]
        [InlineData("The quick brown fox jumps over the lazy dog")]
        [MemberData("SampleTextAsString")]
        public void CanConvertToAndFromString(string testValue)
        {
            MessageBody converted = testValue;
            string result = converted;

            Assert.Equal(testValue, result);
        }

        [Theory]
        [InlineData("The quick brown fox jumps over the lazy dog")]
        [MemberData("SampleTextAsString")]
        public void CanConvertToAStringWhenCreatedWithAUTF8ByteArray(string testValue)
        {
            byte[] testValueAsBytes = Encoding.UTF8.GetBytes(testValue);
            MessageBody converted = testValue;
            string result = converted;

            Assert.Equal(testValue, result);
        }

        [Fact]
        public void ConvertingToNullWorks()
        {
            byte[] testValue = null;
            MessageBody converted = testValue;
            byte[] result = converted;

            Assert.Null(result);
            Assert.True(converted.IsNull);
        }

        [Fact]
        public void ANonNullValueDoesNotSayItIsNull()
        {
            byte[] testValue = new byte[] { 1, 2, 3 };
            MessageBody converted = testValue;

            Assert.False(converted.IsNull);
        }
    }
}
