------------------------------------------------------------------------------------------------------------------
Sequences are where the bulk of the logic and information for a VN lies.

Sequences themselves are Scriptable Objects, containing an array of Screens, an array of Player_Choices, and an
AssetReference used by SaveLoad to store which sequence the player is currently on. (Please set the AssetReference
to be itself.)

Screens are structs that contain the individual dialog boxes, an actor that will be speaking that dialog, and an 
array of Screen_Events (which get their own README).
------------------------------------------------------------------------------------------------------------------
Script_Manager:

The Script Manager does a lot.

It handles progressing through and loading new sequences. It handles taking in and applying the data passed
through SaveLoad. It creates the buttons that the player uses to make choices. It handles the color appearance
of the dialog box. The built-in project should take away the bulk of the setup, leaving any cosmetic alterations
up to you.
------------------------------------------------------------------------------------------------------------------
UI_Element:

The UI Element is a helper class that just stores Images, Buttons, and TMProTexts so that other scripts don't
have to do to many .GetComponent() calls. Basically, you set up a prefab, assign the elements in UI_Element, and
then you can safely assume that you can set whatever you want based on an index. See Actor_Expression for an
example of how this works.
------------------------------------------------------------------------------------------------------------------
TagParser:

The TagParser is a way to implement custom rich tags into your dialog. (If you want <b>Bold Text</b>, you do that)
The Script Manager communicates with the TagParser heavily when displaying dialog, to make sure that effects fire
when they are supposed to.

Only one custom tag is included currently: <Speed=#>.

For creating new custom tags, they will have to be within the format of:

protected static void MethodName(arg){}

and including a:

protected static void EndMethodName{}
------------------------------------------------------------------------------------------------------------------