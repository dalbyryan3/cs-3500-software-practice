
# TankWars - Ryan Dalby and Luke Ludlow (CS 3500 Fall 2019)
---

# Server

## Design overview

### Server view 
Our server's main view is the console application. 
It merely prints messages for the user to read and waits for user input
to kill the server.

The server view also spawns the server web view that runs its own event loop.

The server view also instantiates the controller objects needed and starts the other thread for the main game loop. 

### Server controllers
Our server has separate controllers to handle the responsibility for 
networking and controlling the game.

### Server model
All of the game logic is in the server model. This includes game physics like calculating collisions
and game rules like respawning.


## Database

### Table structure

We implemented a full relational database that follows ACID principles.
This is the "proper" option presented in class, where we have the tables 
Games, Players, and GamesPlayed.

Here are the exact SQL create table statements that we used to create these tables:

```sql
CREATE TABLE `Games` (
  `gID` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Duration` int(11) unsigned NOT NULL,
  PRIMARY KEY (`gID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1
```

```sql
CREATE TABLE `Players` (
  `pID` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) NOT NULL,
  PRIMARY KEY (`pID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1
```

```sql
CREATE TABLE `GamesPlayed` (
  `gID` int(11) unsigned NOT NULL,
  `pID` int(11) unsigned NOT NULL,
  `Score` int(11) unsigned NOT NULL,
  `Accuracy` int(11) unsigned NOT NULL,
  PRIMARY KEY (`gID`,`pID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1
```



### Saving games

Players with the same tank name are treated as the same player.
This means that when a game ends, if there are several players who 
all named themselves "Danny", then when we save both of them under that name with unique playerIDs.
Accessing by a player name may result in multiple of the same player for a game, but the stats for each player are unique.



Game duration is calculated by the game controller and given to the 
database controller when it's time to save the game.
It calculates and tracks the amount of seconds that have passed since the game started.
This is calculated by incrementing a frame count every time Update is called and
multiplying it by the milliseconds per frame in GameSettings.



## Known issues
- Sometimes if you drive into the corner of an open map, the teleportation gets weird 
(this is literally a corner case, haha). The teleportation jitters back and forth a bit 
and then settles you in a location. 
Wrapping around the world bounds works fine in the x and y axis, and corner cases work 
fine if there are some walls around,
so we decided this was a very small bug that we don't need to fix. 

- When the server accepts the first exit key it does not actually stop the game. This is because CancelAsync doesn't actually abort the game loop thread, 
it sends a message to the worker thread that work should be cancelled, since the work is always busy it never gets a chance to actually get an updated status for if a cancellation is pending.
This could be fixed but we do not have enough time. 





---

# Client

## Design overview

### Model
Our model project contains the "world" and all of the objects in it. The model classes are basically "POCOs" - Plain Old
Csharp Objects. 
The game world model is only changed by the controller. The view also references the model,
 but the view
is only allowed to read the game world objects so it can draw them.
These model classes will also be used by the server in PS9.


### View

Our view project primarily contains a Windows Form. On top of this Windows Form we have our
general GUI elements that were inserted using the Visual Studio Designer. We also manually 
inserted our own drawing panel. This drawing panel handles the drawing aspect of our game 
and contains drawer objects for drawing all of the objects that comprise the game.

Our animation code is a bit weird. We finally found a way that works, but we had to use dependency injection to invoke a
call on the drawing panel, and other hacks like that. 
The explosions look great, so we're happy with the results of our animation code, but 
if we had more time we would refactor it to create a special Animator class in the view.

To draw our beam, we created a very long gif (50x2000 pixels).
 We tested to make sure it's long enough by having both of
us drive to opposite corners
of the map and then shoot a beam, and the beam animation extended all the way to the edge of the view  
(past the black part outside of the map) for both players, so it works perfectly!
However, it will probably break on larger maps sizes, but it works great for the tankwars public server. 


### Controller
The controller's job is to handle user input, 
communicate with the server, 
update the game world model, 
and tell the view to draw the next frame.

Our GameController class does this pretty well, so we're happy with our implementation.

We handle keyboard inputs using a state machine. A C# List was chosen specifically because
 order was needed. A Stack was
not used because it can be necessary to remove a specfic object within the list.
A SortedSet was not used since it does not have an easy way to access element-wise.
In order to not get duplicates the input into the List is checked. 
This List input check is not expensive since there can be at most 4 items in the list holding the state of keyboard inputs.

The controls are pretty tight, so you can try to strafe dodge bullets.





## Successes

### What works
Our game client is fully functional! It performs as well as the given TankWars executable.
Our client can handle small details like mashing the WASD keys and holding the mouse click down.
Our view assigns unique colors to the first 8 players, and so on.

### Fun features
- Our custom explosion animation is huge, which makes killing tanks very satsifying!
- Powerups are mario super stars.


## Room for improvement

### Known issues
- Sometimes on very rare occasions, the explosion doesn't animate when you die.
- Sometimes the beam animation is slow, so it kind of looks like a still image, but it's still moving. 
- If you open the help menu while playing, then your keyboard inputs stay on top of the movement instruction stack,
which can be weird. However, the given solution TankWars client does the exact same thing so this isn't really an issue.

### What we would do with more time

There is some logic in the view and long term it would be smart to refactor in order to move this logic to the Controller.

This in turn would allow for the logic to be unit tested, which long term would be something that we would perform on the Controller:
 for both future maintainability and to diagonose any possible bugs.

The Controller would then be refactored into other classes to seperate some of the logic (separate handling key presses,

mouse clicks, networking handshaking and any other logic stuff)

In retrospect, we see some better ways to improve the organization of our model, view, and controller.

We would create a "Common" module that contains all the POCOs (Plain Old Csharp Objects),
and then the Client's Model would be its own self-contained World class.
 Similarly, the future Server would reference the POCOs in the Common module
but would
maintain its own "model" (depending on the server's implementation).

Some things in the view might fit better in the model or controller, such as the PlayerColorManager.
Additionally, some of the logic for deciding when to draw things 
(e.g. when we should not draw a tank because its HP is 0)
 would fit better
as part of the controller.

Unfortunately, we don't have enough time to make our code perfectly organized.
We decided to focus on functionality, because it's more important for the 
game client to play smoothly and look good.
