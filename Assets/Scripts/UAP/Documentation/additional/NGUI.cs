/*!
 * \page NGUI NGUI
 * 
 * <table border="0"><tr><td><img src="images/logoNGUI.png"></td></tr></table>
 * NGUI is one of the most popular UI plugins for Unity. It's being developed by <a href="http://www.tasharen.com/"><b>Tasharen Entertainment</b></a> and can be found in the Unity Asset Store <a href="https://www.assetstore.unity3d.com/en/#!/content/2413"><b>here</b></a>.<br>
 * UAP is compatible with UIs created with NGUI, after enabling support in the Settings.
 *
 * \section NGUIAndUAP NGUI
 * 
 * This page will explain how to set up UAP to work with NGUI.<br>
 * \li [Setting up UAP for NGUI](@ref EnablingNGUISupport) <br>
 * \li [NGUI Demo Scene](@ref DemoSceneNGUI) <br>
 * \li [Trouble Shooting](@ref TroubleShooting) <br>
 * <br>
 * 
 * \section EnablingNGUISupport Setting up UAP for NGUI
 * 
 * Because NGUI is a separate plugin, support must be explicitly enabled to avoid compile errors.<br>
 * You can either do this manually, or have the plugin enable support for you.<br>
 * Make sure you have NGUI added to your project before enabling the plugin support, or the compiler will throw errors, because it cannot find NGUI's classes.<br>
 * 
 * \subsection PreProcessorDefine Enabling NGUI Support
 * <table border="0"><tr><td><img src="images/AutomatiNGUIDetection.jpg"></td></tr></table>
 * If NGUI is already present in the project, you can simply enable NGUI support in the settings in the Accessibility Manager prefab in your scene.<br>
 * The plugin will then set the scripting defines for you (for Android, iOS and Standalone) automatically.
 * 
 * If you want to do it manually, you need to add the string <i>"ACCESS_NGUI"</i> to your Scripting Define Symbols.
 * <table border="0"><tr><td><img src="images/ScriptingDefine.jpg"></td></tr></table>
 * 
 * \section DemoSceneNGUI NGUI Demo Scene
 * 
 * The Examples folder contains a navigation demo scene using NGUI.<br>
 * This scene will only work if you have NGUI installed and setup as described on this page.<br>
 * <br>
 * 
 * \section TroubleShooting Trouble Shooting
 * 
 * \subsection Version NGUI Version
 * The current version of UAP was tested with NGUI version 3.11.2<br>
 * Depending on which version of NGUI you are using the interfaces can change and become incompatible with the existing code.<br>
 * If you are using a different version and encounter compilation errors, please check the support forums for updated code snippets or post with a support request.
 * 
 * \subsection TouchBlocker Touch Blocker and Focus Frame
 * When active, the plugin needs to prevent blind users from accidentally pressing buttons on the screen. Read <a href="MakeYourUIAccessible.html#ScreenOverlay"><b>this</b></a> for more details.<br>
 * For NGUI the touch blocker panel is created dynamically and added to the UI root in the scene after level load. It's depth is set to 11000. Similarly the item frame is place on a panel with depth 12000.<Br>
 * If you have panels with a higher depth, or need to modify this for any other reason, you can adjust the code in the functions UAP_AccessibilityManager::CreateNGUITouchBlocker() and UAP_AccessibilityManager::CreateNGUIItemFrame() respectively.
 * 
 * \subsection CompileErrors Compile Error: The type or namespace name `UIWidget' could not be found
 * This error only happens if NGUI support is enabled, but NGUI is not present in the project.<br>
 * Please double check that NGUI is imported.<br>
 * 
 * \subsection TraversalOrder The elements are traversed in the wrong/random order
 * Because of the way NGUI anchors UI elements and uses its pixel perfect mode, the position of individual UI elements can shift up or down by one pixel 
 * during actual gameplay. This can have the undesired effect that elements that are in a row next to each other are traversed in what appears to be a random order.<br>
 * There are two possible solutions - you can either specify a manual traversal order, or work with dummy widgets and set up reference Targets.<br>
 * These options are documented here: <a href="UIElements.html#Traversal"><b>Manual Traversal Order</b></a> and  <a href="UIElements.html#Target"><b>Reference Targets</b></a>
 * 
*/