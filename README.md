# UnityFlow

Unity Flow is a Project based on the networking tutorial provided by https://github.com/tom-weiland:
https://github.com/tom-weiland/tcp-udp-networking

It streamlines the process into 1 Unity project and provides actions to communicate betweent he server and clients.
There is also an editor script that launches multiple clients for you to test the functionality.

# Settings (Scripts/Flow/FlowSettings)
here you can set the global Settings needed to run a dedicated server.
Also this is the location where you register your actions

# Actions (Scripts/Flow/Actions/*)
Actions must have a postfix "FlowAction" and be located under the namespace Flow.
An action combines all the communication ways between a client and a server.

Actions can be accessed via:

```c#
AweseomeFlowAction action = FlowActions.Get("Aweseome") as AweseomeFlowAction;
action.ToServer("myMessage");
```


## FromClient
  Handle Package received on __server__ *from* the __client__.

## ToServer
  Send Package from __client__ *to* the __server__.

## FromServer
  Handle Package received on the __client__ *from* the __server__.
  
## ToClient
  Send Package from __server__ *to* the __client(s)__.

