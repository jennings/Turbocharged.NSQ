Turbocharged.NSQ
================

[![NuGet](https://img.shields.io/nuget/v/Turbocharged.NSQ.svg)](http://www.nuget.org/packages/Turbocharged.NSQ/)

An [NSQ][nsq] .NET client library filled with `async` happiness.


Usage
-----

Do the normal thing:

    PM> Install-Package Turbocharged.NSQ

### Producing Messages

    var prod = new NsqProducer("nsqd=localhost:4161");
    await prod.PublishAsync("my-topic", new byte[] { 1, 2, 3 });
    
A connection string looks like this:

    nsqd=localhost:4150:4151;

### Consuming Messages

    var cons = NsqConsumer.Create("nsqd=server1:4150; topic=my-topic; channel=my-channel");
    await cons.ConnectAndWaitAsync(Handler);

    static async Task Handler(Message message, IMessageFinalizer finalizer)
    {
        Console.WriteLine("Received: Topic={0}, Channel={1}, MsgLength={2}",
            message.Topic, message.Channel, Message.Data.Length));
        await finalizer.Finish();
    }


Connection string values
------------------------

A connection string looks like this:

    nsqd=localhost:4150;

Or, to use nsqlookupd:

    lookup1:4161; lookup2:4161;

A connection string must specify _either_ an `nsqd` endpoint _or_ `nsqlookupd` endpoints, but not both.

| Setting               | Description                                                                                           |
| --------------------- | ----------------------------------------------------------------------------------------------------- |
| lookupd={endpoints}   | List of `nsqlookupd` servers in the form `hostname:httpport`, e.g., `lookup1:4161;lookup2:4161`       |
| nsqd={endpoints}      | A _single_ `nsqd` servers in the form `hostname:tcpport`, e.g., `nsqd=server1:4150;nsqd=server2:4150` |
| clientId={string}     | A string identifying this client to the `nsqd` server                                                 |
| hostname={string}     | The hostname to identify as (defaults to Environment.MachineName)                                     |
| maxInFlight={int}     | The maximum number of messages this client wants to receive without completion                        |


License
-------

The MIT License. See `LICENSE.md`.


[nsq]: http://nsq.io/
