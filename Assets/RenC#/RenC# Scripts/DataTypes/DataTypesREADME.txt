------------------------------------------------------------------------------------------------------------------
Included in the DataTypes folder are various structs, an enum, and two scriptable objects.

The enum, ConditionalOperators, are mainly used by the flags system to determine what sort of operation the 
designer wants to be performed when checking a flag. (For example, if you want a player choice to only be
available if a flag is equal to, or below a certain value.)

The two scriptable objects are databases used by the Script Manager to handle loading the correct background(s),
overlay(s), and BGM whenever loading a save. If you're having problems with SaveLoad, it is likely that you
forgot to include a specific asset in one of these databases, or have forgetten to assign a database entirely.
There should be bountiful Debug.LogWarning()s that should help make things clear whenever something goes wrong.

The structs are all used within the SaveLoad system, to store key information that should be preserved. Be
careful when editing them, as it may cause existing save files to become corrupted or otherwise unusable.
------------------------------------------------------------------------------------------------------------------ 