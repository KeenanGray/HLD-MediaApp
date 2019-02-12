/*!
 * \page QuickStart Quick Start Guide
 * 
 * Here's how to get your menus converted quickly - without unnecessary explanations or fine-tuning.
 * 
 * <h3>Step 1</h3>
 * Add the Accessibility Manager prefab to the first scene of your project.<br>
 * You can play around with the Settings later. For most applications, the default settings 
 * will be fine.
 * <table border="0"><tr><td><img src="images/ManagerPrefab.jpg"></td></tr></table>
 * 
 * <h3>Step 2</h3>
 * Drag one of your menu screens into the scene.<br>
 * Every UI element that needs to be accessible to the user will get an additional component attached to it in the next step.
 * To read in more detail about these components and what they do, head over to <a href="MakeYourUIAccessible.html"><b>Plugin Basics</b></a>.
 * <table border="0"><tr><td><img src="images/AddYourMenuScreen.png"></td></tr></table>
 * 
 * <h3>Step 3</h3>
 * Add an <i>Accessible UI Group Root</i> component to the root of your menu screen<br>
 * Hint: The <i>Root</i> is usually a Canvas or a Panel, or simply the root of your prefab.<br>
 * <table border="0"><tr><td><img src="images/AddRootComponent.png"></td></tr></table>
 * 
 * If your prefab/screen/dialog is a popup, you need to check the <b><i>Popup</i> check box</b>.
 * <table border="0"><tr><td><img src="images/UIGroupPop.jpg"></td></tr></table>
 * 
 * <h3>Step 4</h3>
 * Select a GameObject in your menu screen hierarchy that has a UI element on it that needs to be accessible.<br>
 * These are <i>Labels, Buttons, Toggles, DropDown Lists, Sliders, Images and Input Edit Fields</i><br>
 * 
 * <h3>Step 5</h3>
 * Add the appropriate accessibility component to the GameObject<br>
 * Example: Add the <i>Accessible Button</i> component to the GameObject containing the uGUI Button component (or UIButton in case of NGUI)
 * <table border="0"><tr><td><img src="images/PlayButtonAccessible.png"></td></tr></table>
 * 
 * You do not need to add label components to the labels of buttons (or toggles). The <i>Accessible Button</i> component will find the label automatically if it is a child of the button and reference it internally.
 * <table border="0"><tr><td><img src="images/AutomaticLabeling.png"></td></tr></table>
 * 
 * <em>Hints:</em>
 * <li>Use accessible labels to add text to images</li><br>
 * <li>Write custom text into the 'Name' field of the accessibility component if the automatic text isn't appropriate.</li><br>
 * <li>You can also construct your label's text automatically:</li>
 * <table border="0"><tr><td><img src="images/CompositeLabel_Setup.png"></td></tr></table>

 * 
 * <h3>Step 6</h3>
 * Repeat the steps 2 through 5 for all UI elements in all your menus until everything is marked up.
 * 
 * <h3>Step 7</h3>
 * Select the Accessibility Manager in your scene and enable Editor Test Mode in the <i>Testing and Debugging</i> rollout.<br>
 * You can now jump into game mode and test your accessibility.
 * <table border="0"><tr><td><img src="images/EditorDebug.jpg"></td></tr></table>
 * 
 * <br>
 * It is <b>strongly</b> recommended that you read this page on <a href="UIElements.html"><b>UI Elements</b></a> next.<br><br>
 * Check out the <a href="HowToGuides.html"><b>How To Guides</b></a> to find answers to the most common questions and use cases.<br>
 * To make your app's accessibility shine, take a look at <a href="BestPractices.html"><b>Best Practices</b></a>.<br>
 * To learn more about how the plugin works in detail, go to <a href="MakeYourUIAccessible.html"><b>Plugin Basics</b></a>.
 * 
 * <h2>Notes</h2>
 * <b>IMPORTANT</b><br>
 * If you are testing on Windows and do not have an actual screen reader installed, the plugin will use Window Speech Synthesis as a fallback for debugging purposes. This system works well, but speech is generated with a very noticeable <b>delay</b><br>
 * This delay is caused by Microsoft SAPI and not the plugin. It will not appear on mobile devices or when testing with NVDA under Windows.<br>
 * Since this can make testing cumbersome, it's recommended that you download NVDA, a free and very popular screen reader.<br>
 * It can be downloaded here:<br>
 * <a href="https://www.nvaccess.org/"><b>NVDA - NonVisual Desktop Access</b></a>
 * 
 * <b>IMPORTANT</b><br>
 * Keyboard navigation on a desktop computer is different from swipe navigation on mobile devices. Use the up and down arrows to navigate.<br>
 * See <a href="Navigation.html"><b>Navigation</b></a> for more details.
 * 
 * <b>IMPORTANT</b><br>
 * By default, the accessibility features will be disabled.<br>
 * The plugin will turn itself on if an accessibility service (such as TalkBack or VoiceOver) is detected.<br>
 * Alternatively, the user of the app can enable them either through a magic gesture, or through a menu (if you offer one).
 * <br><br>
*/