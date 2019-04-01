# ClippySharp
Add Clippy to any xam.mac application for instant nostalgia.

<img src="https://raw.githubusercontent.com/netonjm/ClippySharp/master/images/clippysharp.gif" width="300">

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
* The awesome [Cinnamon Software](http://www.cinnamonsoftware.com/) for developing [Double Agent](http://doubleagent.sourceforge.net/)
the program we used to unpack Clippy and his friends!
* Smore Inc, for creating [clippy.js](https://github.com/smore-inc/clippy.js) :)
* Microsoft, for creating clippy :)
