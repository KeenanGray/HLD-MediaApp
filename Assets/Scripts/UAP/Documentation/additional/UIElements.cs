/*!
 * \page UIElements UI Elements
 * 
 * <h1>Accessible UI Elements</h1>
 * 
 * UAP supports the following UI elements: Labels, Buttons, Toggles (2 variations), Sliders, Text Edit Fields and Dropdown Lists.<br>
 * Most of these elements come with their own set of Settings, which will be discussed here.
 * 
 * \section Settings Common Settings
 * 
 * There are a number of settings that are common to all UI elements, regardless of their individual type.
 * <table border="0">
 * <tr><td><img src="images/CommonSettings.jpg"></td></tr>
 * </table><br>
 * 
 * \subsection Name Name
 * The name of a UI element is the first thing that the plugin will read when the element receives focus.<br>
 * For labels, this will be the text inside the label. For a button, this would be the text on the button. Etc.<br>
 * <table border="0">
 * <tr><td><img src="images/NameSection.jpg"></td></tr>
 * </table>
 * You can provide a name manually, or point the component to a label in the scene that contains the 
 * element's name or description.
 * <table border="0">
 * <tr><td><b>Option 1:</b><br>You can manually provide a name for the element.</td></tr>
 * <tr><td><img src="images/ManualLabelingElement.jpg" height="100"></td></tr>
 * <tr><td><b>Option 2:</b><br>You can point to a text label in the scene.</td></tr>
 * <tr><td><img src="images/ReferenceLabel.jpg" height="100"></td></tr>
 * </table><br>
 * 
 * \subsection Prefix Name Prefix
 * <i><b>NOTE:</b></i> <i>Prefix</i> has been renamed to <i>Combine String</i>. Apart from the name, these instruction remain the same.<br><br>
 * You can have the plugin automatically read out a combination of your own text and the text of a label by setting 
 * a prefix text. This is only available if you are using a text label, obviously.<br>
 * <br>
 * This option is especially useful for virtual currencies, statistics and timers, as these are usually displayed 
 * with a graphic icon and a number. The number is usually set from the game's code.<br>
 * Blind users need some form of context to understand what value the number represents. <br>
 * Here's an example of how to use a prefix to build a composite name:
 * <table border="0">
 * <tr><td><img src="images/CompositeLabel_Lives.png"></td></tr>
 * <tr><td><img src="images/CompositeLabel_Setup.png"></td></tr>
 * </table>
 * 
 * \subsection CombineLabels Combine Labels
 * UAP can automatically combine multiple labels into one accessible label. This reduces the amount of 
 * times the user has to swipe to get the same information as sighted user can see in one single look.
 * This is useful if your UI layout requires using multiple labels, but the information all belongs together.
 * <br><br>
 * In the example below, all stats of an item could be combined into one accessible label, allowing the user 
 * to swipe from one item to the next quickly (instead of having to traverse each stat individually to get to 
 * the next):
 * <table border="0">
 * <tr><td><img src="images/CombinationLabel_Example.png"></td></tr>
 * </table>
 * The <b>UI Navigation</b> sample scene contains an example for an accessible label that combines the
 * information of multiple UI labels into one.
 * <table border="0">
 * <tr><td><img src="images/CombinationLabel_Setup01.png"></td></tr>
 * </table>
 * When navigating, the "Start Items" section of the UI will read out all information in one go/<br>
 * <i>"Start Items: 3 hearts and 3 stars"<br></i>
 * This information is filled in from a total of three UI labels in the scene:
 * <table border="0">
 * <tr><td><img src="images/CombinationLabel_Setup02.png"></td></tr>
 * </table>
 * Only the first label has an Accessible Label component attached to it.<br>
 * In the "Combine Labels" section, the other two labels (the ones containing the 
 * amount of hears and stars) are merely referenced.<br>
 * The Combination string references the labels using placeholders like this: {0}, {1}, {2}, etc<br>
 * In this example, the combination string<br>
 * <i>"{0} {1} hearts and {2} stars"</i><Br>
 * is automatically filled and becomes<br>
 * <i>"Start Items: 3 hearts and 3 stars"<br></i>
 * <table border="0">
 * <tr><td><img src="images/CombinationLabel_Setup03.png"></td></tr>
 * </table>
 * Combination labels also support localization.<br>
 * You will need to include the placeholders {0} etc in your translated texts.<br>
 * The plugin will translate your provided key and fill in the placeholders with the text from the referenced labels.
 * 
 * 
 * 
 * \subsection Target Target
 * <table border="0">
 * <tr><td><img src="images/TargetObject.jpg"></td></tr>
 * </table>
 * The Accessible UI component does not need to be placed on the same GameObject as the 
 * UI element it is supposed to make accessible.<br>
 * This setting is optional.<br>
 * <table border="0">
 * <tr><td><img src="images/ReferencingPosition.jpg"></td></tr>
 * </table>
 * This can be useful to ensure a certain traversal order, because the order is determined
 * from the positions of the GameObject that holds the Accessible component.<br>
 * You can place the Accessible UI component on a different object in you 
 * scene and point to the target UI element here. 
 * <table border="0">
 * <tr><td><img src="images/Highlighting.jpg"></td></tr>
 * </table>
 * The UI highlighting frame will always be shown around the actual target object, not 
 * around the object holding the Accessibility component.
 * <br><br>
 * 
 * \subsection Traversal Traversal Order
 * You can manually set the order in which elements are traversed for individual 
 * UI elements.<br>
 * You can however only change their position order inside their respective UI Group Root. 
 * The order of UI Groups is not influenced by the settings in this section. 
 * <table border="0">
 * <tr><td><img src="images/TraversalOrder.jpg"></td></tr>
 * </table>
 * To manually specify the order in which a number of UI elements are traversed, 
 * you need to specify a sort order and a parent object. You need to supply both,
 * or the setting will be ignored.<br>
 * 
 * <b>Order Index: </b>This is the order (ascending) in which the elements are traversed. 
 * You can leave gaps in the numbering, to allow for easier changes later, for example 
 * 10, 20, 30, and so on.<br>
 * <b>Order Parent: </b>This is an object in your UI that serves as a base for the position 
 * calculation. You might choose to only manually specify the order of some of your UI elements. 
 * The system uses the position of this parent object to determine where to place all of the 
 * manually sorted UI elements in relation to the rest of the UI elements. This does <i>not need 
 * to be an actual parent</i> of any of the UI elements that are manually ordered.
 * <table border="0">
 * <tr><td><img src="images/TraversalSettings.jpg"></td></tr>
 * </table><br>
 * 
 * \subsection Speech Speech Output
 * This section contains settings that apply only when this element receives focus.
 * <table border="0">
 * <tr><td><img src="images/SpeechSection.jpg"></td></tr>
 * </table>
 * <b>Allow VoiceOver</b><br>
 * On iOS you can prevent this element from being read aloud with VoiceOver and read it with the regular speech synthesizer instead.<br>
 * In general this setting should be left at its default state of <i>on</i>, because it allows your users to navigate your app using their chosen voice and speech rate.<br>
 * Disabling this setting could be useful if you need a strict distinction between your regular speech output and this specific element. This could be the case for special notifications and popup windows.<br>
 * 
 * <b>Read Type</b><br>
 * Got all interactive elements, such as buttons, the plugin will read the element type after a short delay after announcing the element name.<br>
 * Example: "Play" (Pause) "Button" (Pause) "Double tap to select"<br>
 * In some special circumstances you might want to prevent the type from being read, for example in puzzle grids, when each grid tile is a button and the constant repetition isn't necessary, but instead annoying and immersion breaking.<br>
 * 
 * <b>Custom Hint Text</b><br>
 * The automatic hint text is picked automatically by the element type and platform.<br>
 * Under certain circumstances you might want to provide a custom hint text here. This can be useful if you want to give more information on what you control does.<br>
 * Example: "Activate to reset all tutorials in the game"<br>
 * You shouldn't rely on users listening to this text however, as often times blind users only listen to the name of a control and then move on.
 * 
 * 
 * \section Sliders Sliders
 * <table border="0">
 * <tr><td><img src="images/SliderSetup.jpg"></td></tr>
 * </table>
 * 
 * \section Toggles Toggles
 * <table border="0">
 * <tr><td><img src="images/ToggleStates.jpg"></td></tr>
 * </table>
 * 
 * \section PluginToggle Plugin Toggle
 * The Accessible Plugin Toggle is a specialized version of a regular toggle designed specifically 
 * to be placed on a toggle in the application's settings screen that controls the accessibility mode. Using this you will not need any additional code to handle this toggle.<br>
 * This type of toggle cannot be used anywhere else.<br>
 * <table border="0">
 * <tr><td><img src="images/PluginToggle.jpg"></td></tr>
 * </table>
 * This helpful script initializes the associated UI toggle with the correct state. Depending on whether or not the plugin is active, this toggle will be set or unset.<br>
 * This is especially helpful because the plugin might be turned on from a number of sources (VoiceOver/TalkBack was detected, 
 * the plugin was active during the last session or the user used the magic gesture to turn accessibility on.).<br>
 * The plugin might also activate/deactivate while the UI toggle is visible on screen (via a gesture for example). This component will ensure that the toggle 
 * state is always correctly updated.
 * <table border="0">
 * <tr><td><img src="images/PluginToggleSettings.jpg"></td></tr>
 * </table>
 * The toggle can also activate/deactivate the plugin when the toggle is flipped in the UI.<br>
 * This behavior is optional (but enabled by default). You can turn it off if you want to show your 
 * users an additional confirmation dialog before enabling accessibility (and drastically changing the controls).<br>
 * In this case you need to call UAP_AccessibilityManager::EnableAccessibility() from your code.
 */