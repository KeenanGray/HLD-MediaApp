/*!
 * \page Platforms Supported Platforms
 * 
 * \section SPlatforms Platforms
 * 
  * <h3>iOS</h3>
 * This platform is fully supported.<br>
 * If VoiceOver is detected, the plugin will activate itself.<br>
 * 
 * <h3>Android</h3>
 * This platform is fully supported.<br>
 * If TalkBack is detected, the plugin will activate itself.<br>
 * TalkBack will need to be suspended during play (the plugin will notify the user to do so automatically).<br>
 * To enter text into input fields, Talkback needs to be unsuspended to make the on-screen keyboard accessible.
 * 
 * <h3>Windows</h3>
 * This platform is fully supported for 64 Bit and 32 Bit targets.<br>
 * The plugin will activate itself when it detects a screen reader, and use it for voice output. As a fallback, Windows SAPI is used as voice output.<br>
 * Currently supported screen readers: NVDA, Window SAPI<br>
 * Jaws, Windows Eye, and others are on the road map.
 * 
 * <h3>Mac OS</h3>
 * Mac OS is supported starting with version v1.0.3 using MacOS VoiceOver.<br>
 * 
 * <h3>Linux</h3>
 * This platform is untested, but the general input should work.<br>
 * It will need a third-party TTS solution (for example <a href="https://assetstore.unity.com/packages/tools/audio/rt-voice-48394"><b>RTVoice</b></a>).<br>
 * Linux support is on the road map, but with a low priority.
 * 
 * <h3>Tizen</h3>
 * This platform is untested, but the general input should work.<br>
 * It will need a TTS solution.<br>
 * Tizen support is currently not on the road map.
 * 
 * <h3>WebGL</h3>
 * Full support for WebGL has been added in UAP v1.0.5, but requires developer to choose a TTS option.<br>
 * WebGL voice output can be done via the integrated Google Cloud TTS (English only) or via a third-party TTS plugin.<br>
 * Please see the <a href="WebGL.html"><b>WebGL documentation</b></a> for detailed setup instructions.<br>
 * Fully integrated, independent TTS is on the road map for a future release.
 * 
 * <h3>Windows Phone</h3>
 * This platform is untested, but the general input should work.<br>
 * It will need a TTS solution.<br>
 * Windows phone support is currently not on the road map.
 * 
 * <h3>XBox One</h3>
 * Currently not supported.<br>
 * Support is planned for a later version, using the XBox Narrator. No release date yet.
 * 
 * <h3>Playstation</h3>
 * Currently not supported. Needed is a completely different input scheme and a TTS solution.<br>
 * Currently not on the roadmap.
 *  
 * <h3>Nintendo Switch</h3>
 * Currently not supported. Needed is a completely different input scheme and a TTS solution.<br>
 * Currently not on the roadmap.
 *  
 * \section Compatibility Compatibility
 *
 * <h3>Unity GUI</h3>
 * The new Unity UI system is fully supported. 
 * 
 * <h3>NGUI</h3>
 * NGUI support is currently active work in progress.<br>
 * Please see the dedicated page on this topic: <a href="NGUI.html"><b>NGUI Support</b></a><br>
*/