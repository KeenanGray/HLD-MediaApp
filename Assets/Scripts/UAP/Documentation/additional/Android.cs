/*!
 * \page AndroidDevelopment Android Development
 * 
 * \section Android Android
 * 
 * The UAP plugin needs a minimum API level of 14.<br>
 * This is only relevant if using an older version of Unity, since the lowest API level Unity 2018 supports is Android 4.1 (API level 16).
 * 
 * \section TalkBack Google TalkBack
 * 
 * The default screen reader on Android is called Google TalkBack. When it is active, it blocks all touch input coming in and decides what to let through to the application. <br>
 * This regularly interferes with apps and games that need swipes and taps for their gameplay. There are a number of accessible apps on the 
 * play store that implement their own accessibility and require that TalkBack is suspended during play.<br>
 * Different from its counterpart VoiceOver on iOS, TalkBack does not offer developers the option to query direct touch input when it is active.
 * <table border="0"><tr><td><img src="images/GoogleTalkBack.png"></td></tr></table>
 * 
 * The reason for this is simple and understandable: If an app could circumvent TalkBack (essentially disabling it), it could lock a 
 * blind user in the app with no way of getting back out. Without TalkBack, it would be impossible to find the soft key home buttons 
 * at the bottom of the screen. iOS devices <i>all</i> feature a physical home button on the device, so a lock-in would be impossible. There is always a way 
 * for the user to return to the home screen.<br>
 * There is a blog post on the topic here: <a href="https://icodelikeagirl.com/2016/03/27/unity-accessibility-plugin-update-1-talkback-voiceover-and-unity/"><b>TalkBack, VoiceOver and Unity</b></a>
 * 
 * This is why <b>users must suspend Google TalkBack </b> when using an app created with Unity.<br>
 * The plugin will prompt users to do so when it detects TalkBack running. <br>
 * 
 * NOTE: If you want to try it out yourself, TalkBack can be disabled by holding down the volume up and down keys at the same time. <br>
 * Visually impaired users are usually familiar with this shortcut, as there are many apps that require TalkBack to be suspended (all Unity apps).
 * 
 * <h3>Advertisements, IAP, Facebook, Google Play etc</h3>
 * If you are using any native plugins on Android, for example to show advertisements or offer in-app purchases in your app, you might want to give an audio prompt to users 
 * to resume TalkBack to be able to interact with the native Google Play Store dialogs. This inconvenience is caused by the TalkBack and Android system architecture and cannot be circumvented.
 * 
 * \section TextInput Text Input
 * 
 * Text Edit boxes are only partially supported on Android, due to the above mentioned TalkBack issue.<br>
 * The onscreen keyboard in Android is a native overlay, which means, blind users can only use it when 
 * they re-enable TalkBack. Apart from this inconvenience, edit boxes can be used normally.
*/