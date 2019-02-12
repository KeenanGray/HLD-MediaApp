/*!
 * \page MakeYourUIAccessible Plugin Basics
 * 
 * This page will cover how the plugin works in more detail.<br>
 * If you just need a quick jump start, check out the <a href="QuickStart.html"><b>Quick Start Guide</b></a>.
 * 
 \section Introduction Introduction
 * The plugin works by attaching an accessibility component to all relevant UI elements in the scene.<br>
 * These components allow the Accessibility Manager to keep track of every component active on the screen. The plugin can then handle navigation between the corresponding UI elements using Swipes (on mobile) or keys (on desktop).
 * It will read out names, types and hints and forward interaction events to the UI elements.<br>
 * Any UI element that has no accessibility component is invisible to the plugin (and to the blind user).
 * 
 \section ScreenOverlay Screen Overlay
 * To make sure that the user doesn't accidentally press any buttons or other UI elements, the Accessibility Manager puts a full screen panel over everything, to catch clicks and touches.<br>
 * This panel is transparent, it won't interferes with your graphics. You can change the color of the panel and make it opaque, if you want.<br>
 * The panel needs to lay on top of everything else. To ensure that, the Canvas it is on has been given a Sort Order of 1000. If your app uses higher sort orders than that, please adjust that number upwards so that it is the highest in your entire app. For NGUI the touch blocker panel is created dynamically and added to the UI root in the scene after level load. It's depth is set to 11000.<br>
 * The panel will not block any input if the Accessibility Manager is disabled, paused or set to not handle UI inputs.
 * 
 \section AccessibilityComponents Accessibility Components
 * The plugin supports <i>Labels</i>, <i>Buttons</i>, <i>Sliders</i>, <i>Toggles</i>, <i>Dropdowns</i> and <i>Text Edit Fields</i>.<Br> 
 * There is also a special toggle version of the called <i>Plugin Toggle</i>, which can be used in the game settings to 
 * enable/disable accessibility without having to write any additional code.<Br>
 * Please take a look at the demo Navigation Scene to find examples of the most common UI elements and use cases.
 * 
 * The various Settings of each component are explained in detail on this page: <a href="UIElements.html"><b>UI Elements</b></a>
 * 
 * <h2>General - Name, Value, Type, Hint</h2>
 * Each accessible UI element has a name, a value, a type and a hint. These are all read out by the accessibility plugin when the element is selected. 
 * The information is ordered by importance - so that the user can quickly abort and move on. Rarely will blind users wait for everything to be spoken.<br>
 * 
 * Example:<br>
 * <table border="0">
 * <tr><td><img src="images/DropDown.jpg"></td></tr>
 * <tr><td><center>Plugin will announce: "Difficulty" - "Easy" - "Dropdown" - "Double Tap to change."</center></td></tr>
 * </table>
 * Note that not all UI elements have a <i>value</i> - only Sliders, Dropdowns and Toggle Buttons do. <br>
 * 
 * <h2>Disabled or Inactive UI elements</h2>
 * When an interactive UI element is set to inactive, but is <b>still visible</b>, the plugin will move the 
 * focus cursor onto the element normally, but read out that it is disabled. It will not allow interaction with the object.
 * <table border="0"><tr><td><img src="images/DisabledElements.png"></td></tr>
 * <tr><td><center>Disabled but visible elements can be focused, but cannot be interacted with</center></td></tr>
 * </table>
 * 
 * Any UI element that is completely hidden will be skipped by the plugin and treated as if it isn't there.
 * 
 * <h2>Automatic and Manual Labeling</h2>
 * Generally, all you need to do is give the element a name. The plugin will take care of value, type and hint.<br>
 * By default, when an accessibility component is added to a UI element like a label or button, it will try to read the label directly. For all other UI elements, or when no Label could be found, the component will use the name of the GameObject as a name.<br>
 * You can also provide a name manually, or point to a label that should be read instead.
 * <table border="0"><tr><td><img src="images/AutomaticLabeling.png"></td></tr></table>
 * <br>
 * 
 * \section UIRoot UI Group Root
 * <table border="0">
 * <tr>
 * <td width=650 valign=top>
 * All accessible UI elements on a screen are grouped together under an Accessible UI Root.<br>
 * There needs to be at least one UI Group Root in any hierarchy containing accessible UI elements (you can have more than one). <br>
 * If there is not, the plugin will let you know about this with an error in the console.
 * <img src="images/UIGroupRoot.jpg">
 * Elements inside a group are ordered by their position, from <em>left to right</em> and <em>top to bottom</em>.<br>
 * The elements can then be cycled through by swiping left/right or using the up/down arrow keys.
 * 
 * You do not have to use more than one UI Group Root per screen - it is completely <i>optional</i>.<br>
 * You can place <em>all UI elements</em> of your screen in the <em>same group</em> - and that is in fact 
 * the fastest way to make your app accessible. If this is all you need for now, you can skip over to the next section<br>
 * </td>
 * <td><table border="0"><tr><td><img src="images/positionOrder.jpg" height=370></td></tr>
 * <tr><td><center>Traversal Order is determined by position</center></td></tr>
 * </table>
 * </td>
 * </tr>
 * </table>
 * 
 * <table border="0">
 * <tr>
 * <td width=650 valign=top>
 * <h3>Speed Up Navigation By Grouping</h3>
 * You can use UI Group Roots to sensibly group UI elements together.<br>
 * Their purpose is to allow blind users quick navigation between groups of UI elements. 
 * Swiping up or down (on mobile devices) within a menu will jump from one group to the next, skipping the UI elements in the current one.
 * 
 * This can tremendously speed up navigation.
 * 
 * Example:<br>
 * Imagine a level selection screen that has 20 level buttons on screen, and a Back and button at the bottom. The user is forced to <b>swipe right 20 times</b> to get to that button.<br>
 * 
 * If the level buttons and the back button were in two separate UI Groups, one single swipe down will have the user jump directly to the Back button.
 * </td>
 * <td><table border="0"><tr><td><img src="images/NeedsContainers.jpg" height=370></td></tr>
 * <tr><td><center>This screen needs grouping for faster navigation!</center></td></tr>
 * </table>
 * </td>
 * </tr>
 * </table>
 * 
 * <table border="0">
 * <tr>
 * <td width=450 valign=top>
 * <table border="0"><tr><td width=400 align=right><img src="images/Container_Navigation_01.png" height=370></td></tr>
 * <tr><td><center>Groups are traversed after one another.</center></td></tr>
 * </table>
 * <Br>
 * It is important to understand that all elements within one container will be navigated through <b>before the next container</b> is entered.<br>
 * This is relevant because groups can overlap one another visually on screen, and groups can contain other groups further down in the scene hierarchy.<br>
 * Accessibility elements will always belong to the first UI Group Root they find upwards from their own position in the hierarchy.<br>
 * The example setup one to the right demonstrates the navigation rather well.
 * </td>
 * <td width=450 valign=top>
 * <br><br><br>
 * The setup on the left shows a menu screen with two UI Group Roots, each containing several (orange) UI elements. 
 * The numbers inside the element represent their position in the navigation list, meaning, 
 * if the user started from the top, he would have to swipe to the next element (minus one) that many times to reach it.<br>
 * <br>
 * 
 * <table border="0"><tr><td width=400 align=right><img src="images/Container_Navigation_02.png" height=370></td></tr>
 * <tr><td><center>Groups are always traversed in order</center></td></tr>
 * </table>
 * </td>
 * </tr>
 * </table>
 * <br>
 * 
 \section TraversalOrder Traversal Order
 * Different from UI elements, UI Groups are <b>not ordered by position.</b><br>
 * UI Groups will be traversed in the order that they (and the GameObjects they are placed on) are created.<br>
 * This is usually equivalent to their order in the hierarchy.<Br>
 * <table border="0"><tr><td><img src="images/UIGRoupPriority.jpg"></td></tr></table>
 * You can overrule this by setting a manual traversal order, using the <i>Priority</i> property of the UI Group Root component.
 * The UI Groups are then traversed in order of priority, from highest to lowest.
 * 
 * When setting this up, it's advisable to use large numbers and leave room between the groups, 
 * so that you have wiggle room to easily add in more groups later.<br>
 * For example 10, 20, 30... instead of 1, 2, 3... will allow you more flexibility later.
 * 
 * 
 */