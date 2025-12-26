------------------------------------------------------------------------------------------------------------------
Tools!

The Sequence_Editor script includes the logic for how Screen_Event property drawers will render and function.
There is a large amount of obsolete and commented code in there, read at your own peril.

Save_Data_From_Main_Menu is used to override the Script Manager trying to load its default sequence whenever
loading from the main menu.

Player_Choice is a struct used by Sequences to define what a player can choose at the end of a sequence. They
include optional conditions, which will result in the player choice not being displayed if the conditions aren't
met. If you just want a sequence to automatically load directly into another, without giving the player a choice,
you can leave the choiceText of the first choice completely blank.

FlagCondition is a struct that references if a certain flag meets a certain condition, based on the selected
operation (==, !=, >=, <=, etc.) and whether or not that operation should be bitwise or not. Bitwise operations
only support the: "==" and "!=" operators. Whenever you use a bitwise operation, remember to check values that
match specifically with binary. (1, 2, 4, 8, 16, 32, etc.) Since these flags are stored in ints, you only
have so many numbers you can work with.
------------------------------------------------------------------------------------------------------------------