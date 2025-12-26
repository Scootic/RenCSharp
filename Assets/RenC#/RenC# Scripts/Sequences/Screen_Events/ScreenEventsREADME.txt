------------------------------------------------------------------------------------------------------------------
Screen_Events are some serious meat and potatoes. With them, you can enact logic whenever the Script Manager
enters a specific screen. All Screen_Events fire before the text displaying coroutine begins, and do not care
whether or not the Script Manager is currently paused. If you want to learn more about that timing, check the
ProgressToNextScreen() method within the Script Manager at line 157.

When adding Screen_Events to a screen, you will notice that they do not seem to have any useful information, just
a dropdown menu. This dropdown menu will allow to select which type of Screen_Event you want, which will change
the selected Screen_Event to match that type.
------------------------------------------------------------------------------------------------------------------
Creating Custom Screen Events:

Whenever you create a custom Screen_Event, make sure that it is both a child of the Screen_Event class, and that
it is within the Screen_Events folder, or a subfolder. When inheriting from Screen_Event, you must create a public
override method for DoShit() that will be fired by the Script Manager.

It is important to include your custom script somewhere in this folder because of how the dropdown menu operates.
It can only give options that are within the Assembly housing Screen_Event, which is Sequences. Additionally,
include an override for ToString(), because that string is what the dropdown menu displays.

By default, your custom Screen_Event will use the same property drawer as other Screen_Events, but you may need
to create your own property drawer if you're doing something criminally insane. (See an example with Actor
_Expression, and Spawn_Actor.)
------------------------------------------------------------------------------------------------------------------
Actor Actions:

Spawn_Actor: Spawns the prefab of the passed in actor, feeding the UI_Element's images sprites based on the
string values you pass in. Will fade in over time.

Actor_Expression: Takes an actor, and alters the existing prefab's UI_Element images based on the string values
you pass in.

Remove_Actor: Despawns the prefab of the passed in actor. Will fade out over time.

Simple_Actor_Motion: Plays an animation that can loop on screen. This animation will lerp between two positional
values: the position when the event is first fired, and that position + the relative offset you set. You can
(and probably should) set the animation curves to make these motions feel juicy and not stinky.
------------------------------------------------------------------------------------------------------------------
Audio Actions:

Play_BGM: Plays a song, fading in based on the time you pass in. If you want to fade out of a song completely,
play an empty sound.

Play_SFX: Plays a sound effect. By default it is a 2D SFX, but it will become a 3D SFX if you change the position
from Vector3.zero. This position will be relative to the main camera. Can also loop.

Stop_SFX: Stops a sound effect if you really don't want to keep playing, especially if you previously looped it.
------------------------------------------------------------------------------------------------------------------
Scene Actions:

Fade_Transition: Goes from one BG sprite to another, playing an animation within the scene to facilitate that.
There are specific requirements for how this fader is found and set, so consult the example scene if the behavior
is not working as you would hope.

Set_Overlay: Sets the Overlay object's image. If you include more than one image, it will be animated. Will fade
out previous overlay before fading in the new one.
------------------------------------------------------------------------------------------------------------------
Set_Flag: Sets a flag to a specific value, or increments it by the passed in value. Handy for branching paths.

------------------------------------------------------------------------------------------------------------------