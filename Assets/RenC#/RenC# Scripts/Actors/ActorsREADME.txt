------------------------------------------------------------------------------------------------------------------
Actor SOs include:

A name that will be displayed in game by the Script Manager.
A prefab that can be spawned, which will be the object that's actually in your scene representing the actor.
A color that the Script Manager will use to change the appearance of the dialog box depending on who's speaking.
An AssetReference, which you should set to be the SO itself, to be used during Save/Load.
All visual information will be stored in an array of SpriteArrays.
------------------------------------------------------------------------------------------------------------------
SpriteArrays:

SpriteArrays are how visual information is stored. These SpriteArrays are composed of an array of sprites (duh)
and a corresponding array of strings. This array of strings are used to set in-game prefabs in a method similar
to dictionaries.

For example: The Change_Actor_Expression event takes in an array of strings (one for each sprite you can change)
and use the input strings to output the corresponding sprites in the SpriteArrays. These output sprites are then
applied to the spawned prefab of the targetted actor.
------------------------------------------------------------------------------------------------------------------