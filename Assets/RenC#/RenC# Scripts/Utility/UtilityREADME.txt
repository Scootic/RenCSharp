------------------------------------------------------------------------------------------------------------------
Included in the Utilites folder are various... utilites.

The Animated_Image_Handler serves two purposes: it handles the animations within the scene (for example, having
your overlay go through multiple different sprites for a pulsing effect, or a noisy grain effect). Not only that,
it also makes sure that SaveLoad can know that there's multiple frames of animation to store.

The Animation_Event_Delegates gives an array of actions that can have methods assigned to them, that are then
fired by a Unity Animation Event based on the event's passed in index. It is currently used by Fade_Transition
screen events to change the background image whenever it is applicable in the animation.

BGM_On_Start plays a song on Start(). Nice.

Persistent_Flags_On_Awake is deceptively useful, it loads in the persistent flags that need to be used on Awake().
I highly recommend manually going into the execution order and putting this bad boy higher up, if you haven't
already.

Set_Persistent_Flag sort of exists only to set a PFlag through a Unity_Event.

Simple_Scene_Loader loads a scene based on build index, and includes a method to exit the game.

String_Data_From_Menu is a garbage hack fraud script that only exists to maintain a string value through
a scene load transition (from main menu to game), that will destroy itself once a valid event exists to pass
the string through. Is currently used to input a custom player name when starting a new game.

Unity_Event_Read_Persistent_Flag fires a Unity Event based on whether or not a persistent flag meets the
conditions set by a designer. The checking method is public, so anyone could fire it, but it also includes
a bool to see if it should fire OnEnable().
------------------------------------------------------------------------------------------------------------------