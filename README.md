# UnityFlow

Unity Flow is a project that aims to make RUDP multiplayer more accesible in unity.

It streamlines the process into 1 Unity project and provides actions to communicate betweent he server and clients.
There is also an editor script that launches multiple clients for you to test the functionality.

This system is intended to be used for fast prototyping and exploration.
If your prototype is complete and you want to take the project to a more serious level, you should split the projects into server and client.
This can be done by creating 2 git submodules for (Project/Client) and (Project/Server).
For building the client you would discard all changes in (Project/Server) and vice versa.

# Settings (Assets/Core/Flow/FlowSettings)
Here you can set the global settings needed to run a dedicated server.

# Actions (Assets/FlowActions)
To create new actions you can use the asset menu (rightclick -> Flow -> Add FlowAction).
Actions are automatically indexed by the script, so no need for registering them.

An action consists of 4 components:

## FlowClientPackage
Package definition for data sent by the client

## FlowServerPackage
Package definition for data sent by the server

## FlowClientAction
Handles data sent by the server
And my sent data back to the server

## FlowServerAction
Handles data sent by the client
And my sent data back to the client/s


# RUDP
Since this project is based on RUDP the data sending of each Action has several transport methods:
- Flow.Shared.FlowAction.SendMethod.ReliableUnordered
- Flow.Shared.FlowAction.SendMethod.ReliableOrdered
- Flow.Shared.FlowAction.SendMethod.ReliableSequenced
- Flow.Shared.FlowAction.SendMethod.Sequenced
- Flow.Shared.FlowAction.SendMethod.Unreliable

When you send data in an action, it's mandatory to define one of those transprot methods:
- Flow.Shared.FlowAction.ActionSender.Send
- Flow.Shared.FlowAction.ActionSender.SendAll
- Flow.Shared.FlowAction.ActionSender.SendExcept
- Flow.Shared.FlowAction.ActionSender.SendExclusively

If you have any further questions or suggestions, feel free to hop into my discord:
https://discord.gg/YqAEJvJaxz
