Guh.

------------------------------------------------------------------------------------------------------------------
TagParser:

The TagParser is the man who interprets and tells tag classes to do things. It is a static class, that is mostly
referenced by Textbox_String. It includes a Parsing method, which compares a tag string given to it (anything
under the <tag> format in Textbox_String), and fires the method by that name, assuming there's a class that
contains it. For implementing custom tags, you must have your classes inherit from Base_Tag, for assembly 
constraint reasons.
------------------------------------------------------------------------------------------------------------------
TagRoutineHandler:

Only exists so tags can fire coroutines, even though they themselves are not monobehaviors. Basically just a
singleton gameobject that exists somewhere in your scene.
------------------------------------------------------------------------------------------------------------------
Base_Tag:

An empty abstract class that only exists to make sure the TagParser doesn't check through every single script
looking for corresponding methods.
------------------------------------------------------------------------------------------------------------------
Tag_Speed:

Changes the speed of the Textbox_String's rate of displaying characters. Only takes a single float argument. This
argument will be the seconds it takes to display the next character, 0 meaning it displays a new char every frame.
The end tag (</Speed>) will reset the Textbox_String's speed back to the previous stored value, one assigned by
settings.
------------------------------------------------------------------------------------------------------------------
Tag_Sine:

Makes characters wibble along a sine/cosine curve. There's only one float argument, which corresponds to frequency
of oscillation.
------------------------------------------------------------------------------------------------------------------
Tag_Noise:

Makes characters jostle around randomly. Takes in two float arguments: the max distance a character can go from
its origin point, and how close it must be to a previous rolled position. (That second one might do nothing???)