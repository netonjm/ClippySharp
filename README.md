# ClippySharp

Add Clippy to any Xamarin.Mac (in the future it will support other UI toolkits) application for instant nostalgia.

| Xam.Mac API | VS4Mac
|---|---|
|  <img src="https://raw.githubusercontent.com/netonjm/ClippySharp/master/images/clippysharp.gif" width="300"> | <img src="https://raw.githubusercontent.com/netonjm/ClippySharp/master/images/clippy-monodevelop.gif" width="300"> |


Usage: Setup
------------
Add this code to you to your application to enable ClippySharp.

```csharp
var agent = new Agent("clippy");
var agentView = new AgentView(agent);
mainWindow.ContentView = agentView;
```

Usage: Actions
--------------
All the agent actions are queued and executed by order, so you could stack them.

```csharp
// play a given animation
agent.Play("Searching");

// play a random animation
agent.Animate();

// get a list of all the animations
agent.GetAnimations ();
// => ["MoveLeft", "Congratulate", "Hide", "Pleased", "Acknowledge", ...]

// Show text balloon
agent.Speak("When all else fails, bind some paper together. My name is Clippy.");

// move to the given point, use animation if available
agent.MoveTo(100,100);

// gesture at a given point (if gesture animation is available)
agent.GestureAt(200,200);

// stop the current action in the queue
agent.StopCurrent();

// stop all actions in the queue and go back to idle mode
agent.Stop();
```

Special Thanks
--------------
* ClippySharp it's a port of [clippy.js](https://github.com/smore-inc/clippy.js) for C#. Thanks to Smore Inc and all the team for creating this awesome assistant. 
* The awesome [Cinnamon Software](http://www.cinnamonsoftware.com/) for developing [Double Agent](http://doubleagent.sourceforge.net/)
the program used to unpack Clippy and his friends!
* Microsoft, for creating clippy :)
