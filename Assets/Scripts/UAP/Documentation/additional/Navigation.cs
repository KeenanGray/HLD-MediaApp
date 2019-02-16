/*!
 * \page Navigation Navigation
 * 
 * \section HowItWorks How It Works
 * This page explains to seeing developers how blind users can navigate your app and your menus in accessibility mode.<br> 
 * The navigation was modeled as closely to the accessibility functions on iOS (VoiceOver) and Android (TalkBack) as possible.
 * 
 * In addition to the navigation input listed on this page the plugin handles <a href="MagicGestures.html"><b>Magic Gestures</b></a> for action such as <i>Back</i> and <i>Pause</i>.
 * 
 * DEBUGGING: In Editor, holding the ALT key down while tapping the left mouse button will emulate a second finger. This allows debugging and testing of gestures requiring more than one finger during mobile development. By default, only mouse taps/clicks are registered. If developing for mobile, you can turn on "Mouse Swiping" in the Settings to enable swipe simulation with the mouse as well.
 * 
 * \subsection ExploreByTouch Explore By Touch
 * 
 * Users can simply swipe over the screen and the accessibility manager will read out what UI element is under the finger. 
 * Explore By Touch is a fast way of finding buttons if the user already has a rough idea where they are located. 
 * Don't be tricked into thinking that visually impaired gamers don't remember where buttons are located.
 * 
 * <i>Explore By Touch</i> is meant for iOS and Android and is disabled on Window by default.<br>
 * You can enable it manually (for testing or if you want to go crazy) in the Accessibility Manager settings.
 * 
 * \subsection Swipes Swipes
 * 
 * Swiping left and right will jump from one UI element to the next. On Windows, the up and down arrow keys are mapped to this feature (and these keys are customizable).
 * Swiping up and down will jump to the next UI group, if there are any (= group of UI elements, see <a href="MakeYourUIAccessible.html"><b>Plugin Basics</b></a>).
 * 
 * Containers can also opt to use 3D grid navigation instead. This is useful for grid based puzzle games (for example Match 3 games). See the <a href="HowToGuides.html"><b>HowTo</b></a> section for more info.
 * 
 * \subsection CyclicMenus Cyclic Menus
 * By default, all screens are treated as non-cyclic menus, meaning, when the bottom of the screen is reached, 
 * swiping again will not change the focus and a notification sound will play, indicating that the end of the screen was reached. This can be changed in the Settings using the 
 * "Cyclic Menus" check box. If this is turned on, then swiping right at the bottom of the screen will jump back up to the first element.\n
 * Do not turn this on without reason. The default is off, because this is how VoiceOver handles the screen borders, and the feedback from blind beta testers has been that this is the preferred way of handling it, too.
 * 
 * \subsection ReadFromTop Read From Top
 * 
 * Swiping down from the top of the screen with two fingers will trigger <i>Read From Top</i>. This makes the accessibility manager read out all 
 * accessible UI elements on the screen in sequential order. It is very useful to get an overview over the screen, and to make sure no buttons etc are missed.
 * 
 * <i>Read From Top</i> is available on iOS and Android only.
 * 
 * \subsection ButtonsAndToggles Buttons and Toggles
 * 
 * Buttons and Toggles are activated with a double tap, after they have been selected. A single tap on a button, toggle or any other UI element will only select it 
 * (via <i>Explore By Touch</i>, but not trigger it. On Windows, the <i>Enter</i> will activate the element. This key is customizable.<br>
 * 
 * \subsection Sliders Sliders
 * 
 * After a slider is selected, users can double tap it to change the value (<i>Enter</i> on Windows). Swiping up and down will change the value up or down (you can adjust by how much). 
 * On Windows you can use the up and down arrow keys to change the value. 
 * Double tapping (or <i>Enter</i>) again will end the interaction and let the user continue navigating as usual.
 * 
 * \subsection ScrollLists Scroll Lists
 * 
 * Scroll lists and scroll views can be navigated using regular left and right swipes (<i>Up and Down Arrow Keys</i> on Windows).<br>
 * The plugin will traverse all elements inside the scroll view as normal, and automatically move the viewport so that the current element is visible.<br>
 * This works for two dimensional scroll views as well. Take a look at the Navigation Example scene to see this in action with a 2D scrollable map.
 * 
 * \subsection SelectionFrame Selection Frame
 * 
 * A high contrast black frame (you can change the color) will be laid around the currently selected UI element.<br>
 * This helps user that are not completely blind determine what is currently selected, and it tremendously helps 
 * developers see what's going on. <br>
 * This is based on a similar feature from the screen readers on iOS and Android. 
*/