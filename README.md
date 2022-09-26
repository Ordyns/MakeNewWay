# MakeNewWay
**Make New Way** is a beautiful mobile puzzle. You need to move/rotate islands to create correct path.  
The game has 24 different handmade levels

<img src="https://user-images.githubusercontent.com/88380021/176698542-fbb6d25c-3743-461d-929e-7a8eb8fcdf94.gif" width="360" height="640">

<p>
  <a href='https://play.google.com/store/apps/details?id=com.OrdynsStudio.MakeNewWay&pcampaignid=pcampaignidMKT-Other-global-all-co-prtnr-py-PartBadge-Mar2515-1'>
    <img alt='Get it on Google Play' src='https://user-images.githubusercontent.com/88380021/176532928-a273e99c-2559-4799-9c90-84832a6be452.png' width="160"/>
  </a>
</p>

# Table of contents
- [How to create level](#how-to-create-level)
  - [Rules](#rules)
  - [First setup](#first-setup)
  - [Level creation](#level-creation)
  - [When the level is finished](#when-the-level-is-finished)
- [Localization](#localization)
  - [How to add new localization](#how-to-add-new-localization)
- [Performance](#performance)
- [Analytics](#analytics)
- [Ads](#ads)
- [Unity](#unity)
- [Packages](#packages)

# How to create level
### **Rules**  
**If you want to create a new level, you must follow several rules:**
- **Preferably**, use **LevelTemplate** (`Assets/Scenes/Level Template`). Make a copy and change it's name.  
- The level name must be in a special format: **Level N**, where **N** is the level number (**N > 0**).  
- Each level must have a **unique number**.  
- Level numbers must be in **ascending order**.   
   ✅ 1, 2, 3, 4 ...  
   ❌ 1, **1**, 3, 4 ...  
   ❌ 1, 3, 4, 5 ...  

🟡 Currently, the game has 24 levels, so if you add a new level, it's ***preferably*** to choose a number greater than 24

### **First setup**   
If you are using the **level template**, you don't need to setup a level.  

### **Level creation**  
All islands are stored in `Assets/Islands/Prefabs/`.  
  
Every island (except Start and Finish) has an **input** and an **output** (*from which side the energy must go*).   
***Note**: **Start** has only **output**, **Finish** has only **input***  

**You can easily configure the islands through the inspector:**  

<img src="https://user-images.githubusercontent.com/88380021/176703225-bf4419e9-6603-4836-829c-2a316e2e7e7b.gif" width="495" height="525">  

You can modify **ComplexIsland** by adding or deactivating child islands of the same type (Movable/Rotatable).

❗ **All islands must be child objects of *"Islands"***. 

### **When the level is finished**
You need to **specify the number of steps** in the **LevelSettings** (*GameObject in the level hierarchy*) that the player will have to complete the level.   
If the level can be completed in two ways, you can specify the number of steps for the alternative solution (**StepsForBonus** in LevelSettings).   

*Also, you can provide a hint (***HintSteps > Steps***).*   

**Then you need to change the levels count**. Find **ProjectContext.prefab** in `Assets/Resources/`, then select **LevelsInfoProvider** (a child GameObject of ProjectContext) and modify **LevelsCount**. If your level **has a bonus** (alternate solution), add its level number to **NumbersOfLevelsWithBonus**.

# Localization
The game is localized into two languages: `English (en_US)` and `Russian (ru_RU)`.  
Localization files (.json) are stored in the `Assets/Resources/Localizations/`.

### How to add new localization
Each localization must have the same keys, so **don't change or delete them**❗     

You can make a copy, for example, of `en_US` and change the values. You should name your localization file in the same format (**language_COUNTRY**).   


After your localization is complete, you need to find **ProjectContext.prefab** in `Assets/Resources/`, then select **Localization** (a child GameObject of ProjectContext) and add a new element to "Languages".  

**Specify**:  
- **Language code** (*it is equal to the name of the localization file*),   
- **Displaying name** (*this name will be displayed in the menu settings, so it **must be about 2-4 letters***)   
- **System language** (*"this localization will be loaded automatically, if the user’s language is **{System language}**"*).    

 

# Performance
| CPU                     | GPU        |   Avg. FPS (ms)   |   1% low  |
| ----------------------- | ---------- | ----------------- | --------- |
| Qualcom Snapdragon 730  | Adreno 618 |  60 FPS (16.5 ms) | 45-50 FPS |
| MediaTek Helio X20      | Mali-T880  |  43 FPS (25 ms)   |   20 FPS  |

*Default render scale is 0.75*

*Without post-processing the game shows up to 60 FPS on amlost all devices*

# Analytics
For analytics the project uses Firebase and Unity Analytics. 
> "Analytics.cs" has commented out code, because the Firebase package was removed to reduce the size of the project and keep some of my data private

The application sends logs to Firebase when:
- player completes the tutorial  
- player completes the level  
- when an error occurs while ad loading

# Ads
~~Ads are showing starting from level 8 on every fourth level (levels 12, 16, 20 etc)~~ (**temporarily removed**)

Also, the player can watch advertising to get a hint

# Unity
Used Unity version: 2020.3.11f1

The project is based on URP

# Packages
• [Extenject (Zenject)](https://github.com/Mathijs-Bakker/Extenject)  
• [DOTween](http://dotween.demigiant.com/)  
• [NaughtyAttributes](https://github.com/dbrizov/NaughtyAttributes)  
• [Firebase Analytics](https://firebase.google.com/docs/analytics/) *(the package was removed, see [Analytics](#analytics))*  
• [GooglePlayCore](https://developer.android.com/guide/playcore)  
• UnityAds  
