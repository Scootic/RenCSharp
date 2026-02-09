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