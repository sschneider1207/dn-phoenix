#
a c# port of the phoenix framework js client (at 1.1.1, commit c09e98e) funded by @livehelpnow

the intention is to follow the js api and c#ly as possible

### dependencies

- newtonsoft.json (7.0.1)
- Websocket4Net (0.14)

#
an example i used to test (C# 6)

    private static void Main(string[] args)
    {
      //let socket = new Socket("/ws", {params: {userToken: "123"}})
      //socket.connect()
      var p = new JObject();
      p["userToken"] = "123";
      var options = new SocketOptions()
      {
        LogCallback = Logger,
        Params = p,
      };
      var socket = new Socket("ws://phoenix.local:4000/socket", options);
      socket.Connect();

      //let channel = socket.channel("rooms:123", { token: roomToken})
      //channel.on("new_msg", msg => console.log("Got message", msg) )
      var data = new JObject();
      var roomToken = "foo";
      data["token"] = roomToken;
      var channel = socket.Channel("rooms:lobby", data);
      channel.On("new_msg", (jo, x) => Console.WriteLine($"new_msg { jo.ToString() }"));

      //channel.join()
      //  .receive("ok", ({messages}) => console.log("catching up", messages) )
      //  .receive("error", ({reason}) => console.log("failed join", reason) )
      //  .receive("timeout", () => console.log("Networking issue. Still waiting...") )
      channel.Join()
        .Receive("ok", (jo) => Console.WriteLine("ok"))
        .Receive("error", (jo) => Console.WriteLine("error"))
        .Receive("timeout", (jo) => Console.WriteLine("timeout"))
        ;

      ReadLoop(channel);
    }
    private static void ReadLoop(Channel channel)
    {
      var input = Console.ReadLine().Trim();
      if (input.Length < 1)
      {
        Console.WriteLine("trying to send");
        var data = new JObject();
        data["body"] = "from dotnet";
        channel.Push("new_msg", data).Receive("ok", (jo) => Console.WriteLine($"PUSH new_msg ok { jo.ToString() }"));
        ReadLoop(channel);
      }
    }
    private static void Logger(string kind, string msg, JObject data = null)
    {
      Console.WriteLine($"{kind} - {msg}");
    }