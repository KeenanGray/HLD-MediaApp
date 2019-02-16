/*!
 * \page HowToGuides How To Guides
 * 
 * <h1>How-To Guides for all Occasions</h1>
 * This page provides How-To guides and solution for common problems and questions.
 * 
 \tableofcontents
 * <h2>Table of Contents</h2>
 * [NGUI Support](@ref NGUI) <br>
 * [Select Focus Item](@ref SelectFocus) <br>
 * [Custom Navigation](@ref CustomNavigation) <br>
 * [Using Native Dialogs](@ref NativeDialogs) <br>
 * [Using Swipes in Gameplay](@ref INeedSwipes) <br>
 * [Grid based Puzzle Games](@ref GridNavigation) <br>
 * [Icons instead of Text](@ref ReferenceLabels) <br>
 * [Custom UI Element Order](@ref PositionOrder) <br>
 * [Localization](@ref Localization) <br>
 * [Using Text-To-Speech](@ref UseAsTTS) <br>
 * [Custom Text-to-Speech plugin](@ref ExchangeTheTTS) <br>
 * [Callbacks](@ref Callbacks) <br>
 * [3D UI Elements](@ref ThreeDee) <br>
 * <br>
 * 
 \section NGUI Enable NGUI Support
 * The plugin supports NGUI, but support must be manually enabled.<br>
 * Please read <a href="NGUI.html"><b>this page</b></a> to learn how to set up UAP to work correctly with NGUI.
 * <br><Br>
 * 
 \section SelectFocus Select Focus Item
 * When opening any kind of menus, the plugin will automatically select the first 
 * element in the UI Group with the highest priority (if there is more than one UI group).
 * From there the user changes the focus by swiping.<br>
 * But you can manually set which item should be focused and provide a much smoother experience 
 * for the user by sensibly setting the focus point to a UI element.<br>
 * 
 * You have two options to do this:
 * 1. All accessible UI elements (Label, Button, etc..) derive from the same base class (UAP_BaseElement) and all of them have a function called ::UAP_BaseElement::SelectItem().
 * If you already have a pointer to your accessible element (AccessibleLabel or similar), or want to get it, then the code would look like this:<br>
 * YourGameObject.GetComponent<UAP_BaseElement>().SelectItem();
 * 2. Alternatively, you can just pass a GameObject to the plugin and ask it to select it. It will find the accessible component on it automatically.<br>
 * In that case, the code would look like this:<br>
 * UAP_AccessibilityManager.SelectElement(YourGameObject);
 * 
 \section CustomNavigation Custom Navigation
 * If your game implements a custom method of selecting UI elements and you don't want the plugin to use swipes 
 * or keyboard input and select UI elements automatically, you will probably only want to use the plugin to voice
 * the currently selected element.<br>
 * You can do so by disabling the automatic UI navigation in the plugin's settings, by disabling the 'Handle UI' checkbox.<br>
 * <table border="0"><tr><td><img src="images/HandleUISettings.jpg"></td></tr></table>
 * You can then select your own UI elements like this: [Select Focus Item](@ref SelectFocus)<br>
 * 
 \section NativeDialogs Using Native Dialog
 * When opening a native dialog or system dialog, you will need to tell the accessibility plugin that is shouldn't react to touch input while the dialog is active. 
 * Otherwise the user will accidentally activate buttons and other UI elements while navigating the native dialog.<br>
 * Call <b>UAP_AccessibilityManager.BlockInput(true)</b> when opening the native dialog, and call it again with a false parameter when it closes.<br>
 * On Android you will need to tell your users to resume TalkBack. This is not needed on iOS, since VoiceOver works fine with Unity.
 * <br><br>
 * 
 \section INeedSwipes Using Swipes in Gameplay
 * With the accessibility enabled, swipes are used to navigate the UI. But what if your gameplay needs to use swipes as well?<br>
 * Instead of disabling accessibility completely, you can opt to just pause it instead. This will allow touch input directly through 
 * to your app instead - but it will still keep the TTS and the Magic Gestures intact.<br>
 * Please see the documentation for ::UAP_AccessibilityManager::PauseAccessibility() for more information.
 * <br><br>
 * 
 \section GridNavigation Grid based Puzzle Games
 * If your gameplay is based on a 2-dimensional grid, you might want to offer your players an alternative way of navigating the playing field.<br>
 * Please take a look at the <a href="DemoProject.html#Navigation"><b>Match 3 Example</b></a> and the documentation on how to set up 2D Navigation for your UI.
 * <br><br>
 * 
 \section ReferenceLabels Icons instead of Text
 * Especially in mobile applications the amount of actual text is usually kept to a minimum. Instead, apps rely heavily on the use of icons.<br>
 * This reduces the amount of localization that needs to be done. It also uses less screen space and it makes the app usable by younger children who can't read yet.
 * 
 * Here are some examples:
 * <table border="0"><tr><td><img src="images/IconInsteadOfText.png"></td></tr></table>
 * To make images like these accessible, you can use the <i>Accessible Label</i> component. But since these images have no text labels for the Accessibility Plugin to read, you need to provide a name for them manually.<br>
 * Please see here on how to do this: <a href="UIElements.html#Name"><b>UI Element Names</b></a>
 * <br><br>
 * Specifically for lives, virtual currencies, boosts and statistics, you can use the comfortable prefix function to add context to your displayed numbers without touching your code.<br>
 * See <a href="UIElements.html#Prefix"><b>Name Prefixes</b></a> for more information.
 * <table border="0">
 * <tr><td><img src="images/CompositeLabel_Example.png"></td></tr>
 * </table>
 * <br>
 * 
 \section PositionOrder Custom UI Element Order
 * Sometimes the automatic traversal order is not what feels natural, or provides the best usability. Sometimes the dimensions of a UI element cause the wrong order. 
 * In any of these scenarios you can provide a manual traversal order instead.<Br>
 * Read here on how to set that up: <a href="UIElements.html#Traversal"><b>Manual Traversal Order</b></a>
 * <br><br>
 * 
 \section Localization Localization 
 * There's several parts to localization:
 * - Localization of the plugin's internal text phrases
 * - Localization of the voice that's used to read out the UI elements
 * - Localization of text in labels, buttons etc
 *
 \subsection UAP_Localization Localization of the plugin
 * On app start, the plugin tries to detect the system's set language and sets it automatically.<br>
 * To set the language for the plugin manually, call UAP_AccessibilityManager.SetLanguage(string language)<br>
 * If the language is not supported, the plugin will fall back to English (or whatever language is first in the 
 * localization table - by default, this is English)<br>
 * All the plugin's internal text phrases (such as 'double tap to select') are read from a localization table. This table contains English and German 
 * localization out-of-the-box and can be easily extended with more languages if desired.<br>
 * The table can be found here:<br><code>UAP/Resources/UAP_InternalLocalization.txt</code><br>
 * This is a tab-separated spreadsheet, which can be renamed to TSV and imported into Excel or Google Sheets. When you are 
 * done adding or changing languages, simply export as a TSV again, and rename the file extension to TXT.
 *
 \subsection Voice_Localization Voice Selection
 * UAP currently uses the default system voice for Windows SAPI, Android, MacOS and iOS.<br>
 * The included Google TTS system currently always uses an en-US voice and does not support localization yet.<br>
 * On iOS, if VoiceOver is used for speech output, the currently active VoiceOver voice is used.<br>
 * On Windows, if NVDA is used, the currently active NVDA voice is used.<br>
 * The voices used can not be chosen by the app.
 *
 \subsection External_Localization Localization of labels, buttons, etc
 * All Accessible UI components offer the option to treat manually entered text as localization lookup keys.<br>
 * This means the text will not be spoken directly as entered, but used as a lookup key to get a localized string back, 
 * which will then be read aloud.<br>
 * <table border="0"><tr><td><img src="images/LocalizationCheckbox.jpg"></td></tr></table>
 * 
 * When this option is checked, the plugin will call its internal function UAP_AccessibilityManager::Localize() before 
 * reading out the text.<br>
 * <b>NGUI</b>: If you are using NGUI, the plugin will automatically use NGUI's internal localization system.<Br>
 * <b>Unity UI</b>: Unity doesn't have its own localization system, but you can hook the appropriate call to whatever 
 * plugin or system you are using on the function directly and return the localized result. This function is used for 
 * all localization calls, so you won't have to change any other code.
 * 
 * If everything is set up correctly, a preview of the translated text is displayed below the checkbox.<br>
 * Note that this option is not available if the name text is read from a different label, as any such text would obviously
 * already be localized.<br>
 * 
 * <h3>Alternative Solution</h3>
 * You can always choose to fill in text into the name field of an accessibility component directly from the code.<br>
 * That allows you to construct the content at runtime, and also to run it through your own localization system.<Br>
 * To do that, you can use the base class for all Accessible UI components: UAP_BaseElement.<br>
 * Each class deriving from this has a "text" member which you can use to fill in the name text: UAP_BaseElement::m_Text
 * <br><br>
 * 
 \section UseAsTTS Using Text-To-Speech
 * Sometimes you want to offer blind users additional information, since they can't grasp the entire 
 * view of the screen like a sighted user. You might want to announce the new total score after the 
 * player earned some points, or read out the remaining moves, or announce how many other cards of the same color are in the players hand.<br>
 * Little efforts like these will make your game stand out in terms of accessibility!
 * 
 * You can use the Accessibility Plugin's Text-To-Speech system to read out this extra information for you. 
 * This will also make sure that the audio won't clash with any UI elements that are being read at the same time.
 * Use UAP_AccessibilityManager::Say() to speak your additional feedback. Depending on the setting, you can have your output 
 * interrupt the current speech (if any), or wait and be queued. You can also determine whether you allow your 
 * text to be interrupted by the user (if he wants to skip it, for example).
 * <br><br>
 * 
 \section ExchangeTheTTS Custom Text-to-Speech plugin
 * The UAP plugin ships with native TTS for Android and iOS, and supports SAPI and NVDA 
 * on Windows computers, and VoiceOver on Mac.<br>
 * On iOS the plugin can also use VoiceOver for speech output if it is running. On WebGL the plugin can
 * connect to the Google Cloud TTS API if you provide an API key. See <a href="WebGL.html">WebGL</a> for more information.
 * <br><br>
 * If you prefer to use a different TTS system, or use a different plugin for TTS, you can easily connect the 
 * plugin to use a custom TTS. <br>
 * 
 * \subsection ImplementCustomTTS Implement the Interface
 * You need to implement a TTS interface class to connect your TTS plugin with UAP. This is very easy and usually 
 * takes less than 30 minutes if you already know how to use your TTS plugin.
 * <br>
 * 1. Create a copy of the file UAP/Plugins/CustomTTS/Custom_TTS_Template.cs and rename the copy. You need to rename both the filename and the class name.<br>
 * 2. Implement the five functions inside the file. All functions are commented with detailed explanations.<br>
 * - void UAP_CustomTTS::Initialize() <br>
 * - TTSInitializationState UAP_CustomTTS::GetInitializationStatus() <br>
 * - void UAP_CustomTTS::SpeakText(string textToSay, float speakRate) <br>
 * - void UAP_CustomTTS::StopSpeaking() <br>
 * - bool UAP_CustomTTS::IsCurrentlySpeaking() <br>
 * 
 * \subsection EnableCustomTTS Enable your TTS
 * Open the file UAP_AudioQueue.cs and navigate to the function UAP_AudioQueue::InitializeCustomTTS().
 * <br>
 * 1. Comment in the line UAP_CustomTTS.InitializeCustomTTS<YOUR_CLASS_NAME>(); <br>
 * 2. Replace 'YOUR_CLASS_NAME' with the name of your new TTS class that you created in the previous step.
 * <table border="0"><tr><td><img src="images/CustomTTS_Code.jpg"></td></tr></table>
 *
 \section Callbacks Callbacks
 * Apps can register for various callbacks with UAP, to be notified of important events.<br>
 * These events include the starting and stopping of the plugin, or special multi-finger gestures.<br>
 * Please see the page on <a href="MagicGestures.html#SupportedGestures"><b>Magic Gestures</b></a> for a complete list of events and how to register for them.
 * 
 \section ThreeDee 3D UI Elements
 * Please see the demo scene '2D UI Example' inside the plugin's Examples folder.
 * 
*/