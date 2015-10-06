using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Turbocharged.NSQ.Tests
{
    public class ConsumerOptionFacts
    {
        [Theory]
        [InlineData("foo:123; Channel=ABC; Topic=123; Clientid=HelloWorld")]
        [InlineData("foo:123; channel= ABC  ; topic=123;clientid=HelloWorld;")]
        [InlineData("foo:123; cHaNnEl=   ABC;ToPiC=123 ; cLiEnTiD = HelloWorld;")]
        public void ParsingIsCaseSpaceAndSemiColonInsensitive(string connectionString)
        {
            var options = ConsumerOptions.Parse(connectionString);
            Assert.Equal("ABC", options.Channel);
            Assert.Equal("123", options.Topic);
            Assert.Equal("HelloWorld", options.ClientId);
        }

        [Fact]
        public void ParsingGetsAllFields()
        {
            var connectionString = "foo:123; channel=abc; clientId=def; hostname=ghi; maxInFlight=123; reconnectionDelay=55; reconnectionMaxDelay=66; topic=foobar";
            var options = ConsumerOptions.Parse(connectionString);
            Assert.Equal("abc", options.Channel);
            Assert.Equal("def", options.ClientId);
            Assert.Equal("ghi", options.HostName);
            Assert.Equal(123, options.MaxInFlight);
            Assert.Equal(TimeSpan.FromSeconds(55), options.ReconnectionDelay);
            Assert.Equal(TimeSpan.FromSeconds(66), options.ReconnectionMaxDelay);
            Assert.Equal("foobar", options.Topic);
        }

        [Fact]
        public void ParsingTakesTheLastOfRepeatedFields()
        {
            var connectionString = "foo:123; clientid = FIRST; clientid = SECOND; clientid = THIRD";
            var options = ConsumerOptions.Parse(connectionString);
            Assert.Equal("THIRD", options.ClientId);
        }

        [Theory]
        [InlineData("foo:123;bar:456;baz:789;clientid=HelloWorld")]
        [InlineData("foo:123; bar:456; baz:789; clientid=HelloWorld;")]
        [InlineData("foo:123 ; bar:456 ; lookupd= baz:789 ; clientid = HelloWorld;")]
        [InlineData("lookupd = foo:123 ; lookupd= bar:456 ; lookupd=baz:789 ; clientid = HelloWorld;")]
        public void AllLookupInstancesAreUsed(string connectionString)
        {
            var options = ConsumerOptions.Parse(connectionString);

            var expectedEndPoints = new DnsEndPoint[] {
                new DnsEndPoint("foo", 123),
                new DnsEndPoint("bar", 456),
                new DnsEndPoint("baz", 789),
            };

            Assert.Equal("HelloWorld", options.ClientId);
            Assert.Equal(expectedEndPoints.Length, options.LookupEndPoints.Count);
            Assert.All(options.LookupEndPoints,
                ep => Assert.True(expectedEndPoints.Contains(ep)));
        }

        [Theory]
        [InlineData("nsqd = foo:123")]
        [InlineData("nsqd = bar:456; nsqd = foo:123;")]
        [InlineData("nsqd = bar:456; nsqd = baz:789; nsqd = foo:123")]
        public void OnlyTheLastNsqdInstanceIsUsed(string connectionString)
        {
            var options = ConsumerOptions.Parse(connectionString);
            Assert.Equal(0, options.LookupEndPoints.Count);
            Assert.Equal(new DnsEndPoint("foo", 123), options.NsqEndPoint);
        }

        [Fact]
        public void NsqdInstancesAreIgnoredIfLookupdIsProvided()
        {
            var connectionString = "foo:123; lookupd = bar:456; nsqd = baz:789; clientid=HelloWorld;";
            var options = ConsumerOptions.Parse(connectionString);
            Assert.Null(options.NsqEndPoint);
            Assert.Equal(2, options.LookupEndPoints.Count);
        }
    }
}
