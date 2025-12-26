------------------------------------------------------------------------------------------------------------------
Menus, whether that be a pause menu or a main menu, are handled by the Menu Manager, which will automatically
display and hide the appropriate content, based on an index. When implementing, you make your menu opening UI
(usually a button) fire the OpenMenu function, passing in the integer that correlates with the index of the 
menu script assigned to the Menu Manager.

Menus already included are histories (which display previous text boxes), save/load (which should be obvious),
and simple menus that don't have any custom functionality (only existing to display static content, or nothing
that needs unique behavior).
------------------------------------------------------------------------------------------------------------------
Creating Custom Menus:

All menus, if you want them to be handled by the Menu Manager, must be children of the Menu_Base class.
Included in Menu_Base are two abstract methods: OnMenuOpen, and OnMenuClose.

I like to think that they are self explanatory: OnMenuOpen is fired when the specific menu you chose is being
opened, while OnMenuClose is fired by the currently open menu that will be replaced by a new one. If you
desperately need to learn more about the timing, look into the Menu Manager script.
------------------------------------------------------------------------------------------------------------------