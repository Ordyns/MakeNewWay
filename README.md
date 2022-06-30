# MakeNewWay
Make New Way - is beautiful mobile puzzle. You need to move/rotate islands to create correct path.  
The game has 24 different handmade levels

<img src="https://user-images.githubusercontent.com/88380021/176698542-fbb6d25c-3743-461d-929e-7a8eb8fcdf94.gif" width="360" height="640">

<p>
  <a href='https://play.google.com/store/apps/details?id=com.OrdynsStudio.MakeNewWay&pcampaignid=pcampaignidMKT-Other-global-all-co-prtnr-py-PartBadge-Mar2515-1'>
    <img alt='Get it on Google Play' src='https://user-images.githubusercontent.com/88380021/176532928-a273e99c-2559-4799-9c90-84832a6be452.png' width="160"/>
  </a>
</p>

# Editor
For the islands has been written a custom editor to make it easier to create levels  

<img src="https://user-images.githubusercontent.com/88380021/176703225-bf4419e9-6603-4836-829c-2a316e2e7e7b.gif" width="660" height="700">

# Analytics
For analytics the project uses Firebase and Unity Analytics. 
> "Analytics.cs" has commented out code, because the Firebase package was removed to reduce the size of the project and keep some of my data private

The application sends logs to Firebase when:
- player completes the tutorial  
- player completes the level  
- when an error occurs while ad loading

# Ads
Ads are showing starting from level 8 on every fourth level (levels 12, 16, 20 etc)
```C#
public static bool AdLevel(int levelNumber) => levelNumber > 8 && levelNumber % 4 == 0;
```

Also, the player can watch advertising to get a hint

# Localization
The game is localized into two languages: `English (en_US)` and `Russian (ru_RU)`.  
Localization files (.json) are stored in the `Assets/Resources/Localizations/`.

# Performance
| CPU                     | GPU        |   Avg. FPS (ms)   |   1% low  |
| ----------------------- | ---------- | ----------------- | --------- |
| Qualcom Snapdragon 730  | Adreno 618 |  60 FPS (16.5 ms) | 45-50 FPS |
| MediaTek Helio X20      | Mali-T880  |  43 FPS (25 ms)   |   20 FPS  |

Default render scale is 0.75

*Without post-processing the game shows up to 60 FPS on amlost all devices*

# Unity
Used Unity version: 2020.3.11f1

The project is based on URP

# Packages
• [DOTween](http://dotween.demigiant.com/)  
• [NaughtyAttributes](https://github.com/dbrizov/NaughtyAttributes)  
• [Firebase Analytics](https://firebase.google.com/docs/analytics/) *(the package was removed, see "Analytics" section)*  
• UnityAds  
• [GooglePlayCore](https://developer.android.com/guide/playcore)